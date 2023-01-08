using SharpAI.API;
using SharpAI.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using static SharpAI.API.Constants;
using static SharpAI.API.AIModules;
using System.Text.Json.Serialization;

namespace SharpAI
{
    public class SharpOpenAI
    {
        private HttpClient client;
        public HttpClient Client { get { return client; } set { client = value; } }
        public SharpOpenAI()
        {
            client = new HttpClient();
        }
        private HttpRequestMessage PrepareRequest<T>(string url, bool get = true, T? dataToSend = null) where T : class
        {
            if (!File.Exists(API_CONFIG_FILE))
                throw new FileNotFoundException($"Configuration file: ({API_CONFIG_FILE}) not found ! Aborting ...");

            APIConfiguration? configurationFile;
            HttpRequestMessage? httpRequest = new();

            try
            {
                configurationFile = JsonSerializer.Deserialize<APIConfiguration>(File.ReadAllText(API_CONFIG_FILE));

                if (configurationFile is not null)
                {
                    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", configurationFile.Key);

                    if (!string.IsNullOrEmpty(configurationFile.Organization))
                        httpRequest.Headers.Add("OpenAI-Organization", configurationFile.Organization);

                    httpRequest.RequestUri = new Uri(url);
                    httpRequest.Method = (get == true ? HttpMethod.Get : HttpMethod.Post);

                    if (httpRequest.Method == HttpMethod.Post && dataToSend != null)
                    {
                        // serialize data as json
                        var jsonData = JsonSerializer.Serialize(dataToSend);

                        httpRequest.Content = new StringContent(jsonData);
                        httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }


                }

                return httpRequest;

            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error during derserialization process. Cannot read API settings!");
                Console.WriteLine(ex.Message);
            }

            return httpRequest;
        }

        private async Task<Stream> Post<T>(T dataToSend) where T : class
        {
            if (dataToSend is null)
                throw new ArgumentNullException("Data cannot be null!");

            var type = dataToSend.GetType();
            var url = string.Empty;

            switch (type)
            {
                case Type _ when type == typeof(Completion):
                    url = TEXT_URL;

                    break;

                case Type _ when type == typeof(Image):
                    url = IMAGES_URL;
                    break;
            }

            using var requestMessage = PrepareRequest(url, false, dataToSend);
            var apiResponse = await client.SendAsync(requestMessage);
            var apiStream = await apiResponse.Content.ReadAsStreamAsync();

            if (!apiResponse.IsSuccessStatusCode)
            {
                var currentStatus = new StreamReader(apiStream).ReadToEnd();
                throw new HttpRequestException(currentStatus);
            }


            return apiStream;

        }

        public async Task<Models> GetAvailableModels()
        {
            using (client = new())
            {
                using (var requestMessage = PrepareRequest<object>(MODELS_URL, true))
                {

                    var response = await client.SendAsync(requestMessage);

                    var data = JsonSerializer.Deserialize<Models>(await response.Content.ReadAsStreamAsync());

                    if (data is null) // this should never happen
                        throw new Exception("Serialized data is null. Aborting ...");

                    return data;

                }
            }


        }

        /// <summary>
        /// Ask to AI
        /// </summary>
        /// <typeparam name="T">Data Type.</typeparam>
        /// <param name="data">Data that will be sent.</param>
        /// <returns>Answer, images, etc.</returns>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<string> AskToAI<T>(T data, bool saveToFile = false) where T : class
        {
            string output = string.Empty;
            Type t = typeof(T);

            switch (t)
            {
                case Type _ when t == typeof(Completion):

                    var d = data as Completion;
                    var completionResponse = await Post(data);

                    var deserializedCompletionResponse = await JsonSerializer.DeserializeAsync<CompletionResponse>(completionResponse, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

                    if (deserializedCompletionResponse is null)
                        throw new JsonException("Deserialized content is null.");

                    var tokens = deserializedCompletionResponse.Choices?.Select(x => x.LogProbs?.Tokens).First();

                    if (tokens is not null)
                    {
                        foreach(var token in tokens)
                        {
                            if (token.Equals("<|endoftext|>")) // if stop settings is set to null.
                                break;

                            output += token;
                        }
                    }
                    else
                        return "EMPTY_REQUEST";

                    break;

                case Type _ when t == typeof(Image):

                    var imageResponse = await Post(data);
                    var deserializedImageResponse = await JsonSerializer.DeserializeAsync<ImageResponse>(imageResponse);

                    if (deserializedImageResponse is null)
                        throw new JsonException("Deserialized content is null.");

                    var getUrls = deserializedImageResponse.Images;

                    if (getUrls is null)
                        throw new NullReferenceException(nameof(getUrls));

                    output = string.Join(",", getUrls);
                    break;

                default:
                    throw new NotImplementedException($"{nameof(t)} isn't implemented or supported yet.");
            }

            if (saveToFile)
                File.WriteAllText("output.txt", output);

            return output.Trim();
        }



    }
}

