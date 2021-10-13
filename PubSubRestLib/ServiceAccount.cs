using System.Text.Json.Serialization;

namespace PubSubRestLib
{
    public class ServiceAccount
    {
        [JsonPropertyName("private_key")] public string PrivateKey { get; set; }
        [JsonPropertyName("client_email")] public string ClientEmail { get; set; }
        [JsonPropertyName("project_id")] public string ProjectId { get; set; }
    }
}