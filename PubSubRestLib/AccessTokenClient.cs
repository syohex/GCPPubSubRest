using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PubSubRestLib
{
    public class AccessTokenClient
    {
        private class AccessTokenResponse
        {
            [JsonPropertyName("access_token")] public string AccessToken { get; set; }
        }

        public async Task<string> GetAccessToken(string assertion)
        {
            var parameters = new Dictionary<string, string>
            {
                {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                {"assertion", assertion}
            };
            var body = new FormUrlEncodedContent(parameters);
            using var client = new HttpClient();
            var res = await client.PostAsync("https://www.googleapis.com/oauth2/v4/token", body);
            string responseBody = await res.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<AccessTokenResponse>(responseBody);
            if (data == null)
            {
                throw new Exception("Could not get access token");
            }

            return data.AccessToken;
        }
    }
}