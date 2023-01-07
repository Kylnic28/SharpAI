using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SharpAI.API
{
    public class Image
    {
        /// <summary>
        /// A text description of the desired image(s). The maximum length is 1000 characters.
        /// </summary>
        [JsonPropertyName("prompt")]
        public string? Prompt { get; set; }

        /// <summary>
        /// The number of images to generate. Must be between 1 and 10.
        /// </summary>

        [JsonPropertyName("n")]
        [Range(1, 10)]
        public uint Number { get; set; } = 1;

        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>

        [JsonPropertyName("size")]
        public string? Size { get; set; } = "1024x1024";

        /// <summary>
        /// The format in which the generated images are returned. Must be one of url or b64_json.
        /// </summary>

        [JsonPropertyName("response_format")]
        public string? ResponseFormat { get; set; } = "url";


        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// </summary>

        [JsonPropertyName("user")]
        public string? User { get; set; } = "SharpAI";
    }

    public class ImageResponse 
    {
        [JsonPropertyName("created")]
        public uint Created { get; set; }

        [JsonPropertyName("data")]
        public List<ImageData>? Images { get; set; }
    }
    public class ImageData
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }


        public override string ToString()
        {
            return Url;
        }
    }
}
