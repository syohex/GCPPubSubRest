using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PubSubRestLib
{
    public class PubSubClient
    {
        private const string BaseUrl = "https://pubsub.googleapis.com/v1";

        public PubSubClient()
        {
        }

        private class PubSubMessages
        {
            [JsonPropertyName("messages")] public PubSubMessage[] Messages { get; set; }
        }

        private class PubSubMessage
        {
            [JsonPropertyName("data")] public string Data { get; set; }
            [JsonPropertyName("attributes")] public Dictionary<string, string> Attributes { get; set; }
        }

        private class PublishResponse
        {
            [JsonPropertyName("messageIds")] public string[] MessageIds { get; set; }
        }

        public async Task<string[]> PublishMessage(Credential credential, string topic, string message,
            Dictionary<string, string> attributes)
        {
            var accessTokenClient = new AccessTokenClient();
            var accessToken = await accessTokenClient.GetAccessToken(credential.ToAssertion());
            var endPoint = $"{BaseUrl}/projects/{credential.ServiceAccount.ProjectId}/topics/{topic}:publish";

            var msgBytes = Encoding.UTF8.GetBytes(message);
            var data = Convert.ToBase64String(msgBytes);

            var pubSubMessages = new PubSubMessages
            {
                Messages = new[]
                {
                    new PubSubMessage
                    {
                        Data = data,
                        Attributes = attributes
                    }
                }
            };

            var postBody = JsonSerializer.Serialize(pubSubMessages);
            var content = new StringContent(postBody, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, endPoint);
            request.Content = content;
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Headers.Add("Accept", "application/json; charset=utf-8");

            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Got unexpected status code: {response.StatusCode}");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            var publishResponse = JsonSerializer.Deserialize<PublishResponse>(responseBody);
            if (publishResponse == null)
            {
                throw new Exception($"Got unexpected response: {responseBody}");
            }

            return publishResponse.MessageIds;
        }
    }
}