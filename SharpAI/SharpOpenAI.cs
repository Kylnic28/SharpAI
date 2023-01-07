using SharpAI.API;
using SharpAI.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static SharpAI.API.Constants;
using static SharpAI.API.AIModules;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpAI
{
    public class SharpOpenAI
    {
        private static HttpClient? client;
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
        /// <param name="module">Module that will be used.</param>
        /// <param name="data">Data that will be sent.</param>
        /// <returns>Answer, images, etc.</returns>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<string> AskToAI<T>(AIModules module, T data) where T : class
        {            
            string output = string.Empty;

            switch (module)
            {
                case TextGenerationModule:
                    var completionResponse = await Post<T>(data);
                    var deserializedCompletionResponse = await JsonSerializer.DeserializeAsync<CompletionResponse>(completionResponse);

                    if (deserializedCompletionResponse is null)
                        throw new JsonException("Deserialized content is null.");

                    var tokens = deserializedCompletionResponse.Choices?.Select(x => x.LogProbs?.Tokens).First();

                    if (tokens is not null)
                    {
                        foreach (var token in tokens)
                            output += token.ToString();
                    }
                    else
                        return "EMPTY_REQUEST";

                    break;

                case ImageGenerationModule:
                    var imageResponse = await Post<T>(data);
                    var deserializedImageResponse = await JsonSerializer.DeserializeAsync<ImageResponse>(imageResponse);

                    if (deserializedImageResponse is null)
                        throw new JsonException("Deserialized content is null.");

                    var getUrls = deserializedImageResponse.Images;

                    if (getUrls is null)
                        throw new NullReferenceException(nameof(getUrls));

                    output = string.Join(",", getUrls);
                    break;
            }

            return output.Trim();
        }


    }
}

