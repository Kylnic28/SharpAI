using System.Text.Json.Serialization;

namespace SharpAI.API
{
    public class Models
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("data")]
        public List<ModelData>? Data { get; set; }

    }
    public class ModelData
    {
        [JsonPropertyName("id")]
        public string? Identifier { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("permission")]
        public List<ModelPermission>? Permission { get; set; }

        [JsonPropertyName("root")]
        public string? Root { get; set; }

        [JsonPropertyName("parent")]
        public string? Parent { get; set; }
    }

    public class ModelPermission
    {
        [JsonPropertyName("id")]
        public string? Identifier { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("created")]
        public uint Created { get; set; }

        [JsonPropertyName("allow_create_engine")]
        public bool AllowCreateEngine { get; set; }

        [JsonPropertyName("allow_sampling")]
        public bool AllowSampling { get; set; }

        [JsonPropertyName("allow_logprobs")]
        public bool AllowLogProbs { get; set; }

        [JsonPropertyName("allow_search_indices")]
        public bool AllowSearchIndices { get; set; }

        [JsonPropertyName("allow_view")]
        public bool AllowView { get; set; }

        [JsonPropertyName("allow_fine_tuning")]
        public bool AlllowFineTuning { get; set; }

        [JsonPropertyName("organization")]
        public string? Organization { get; set; }

        [JsonPropertyName("group")]
        public string? Group { get; set; }

        [JsonPropertyName("is_blocking")]
        public bool IsBlocking { get; set; }

    }
}

