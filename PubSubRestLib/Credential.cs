using System;
using System.Buffers.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PubSubRestLib
{
    public class Credential
    {
        private readonly ServiceAccount _serviceAccount;
        private readonly RSA _privateKey;
        private const string PubSubScope = "https://www.googleapis.com/auth/pubsub";

        public Credential(string serviceAccountJson)
        {
            var jsonStr = File.ReadAllText(serviceAccountJson);
            _serviceAccount = JsonSerializer.Deserialize<ServiceAccount>(jsonStr);
            if (_serviceAccount == null)
            {
                throw new ArgumentException("invalid service_account.json");
            }
            
            _privateKey = ParsePrivateKey(_serviceAccount.PrivateKey);
        }

        private class JwtHeader
        {
            [JsonPropertyName("alg")] public string Algorithm { get; set; }

            [JsonPropertyName("typ")] public string Type { get; set; }
        }

        private class JwtClaim
        {
            [JsonPropertyName("iss")] public string Issuer { get; set; }
            [JsonPropertyName("scope")] public string Scope { get; set; }
            [JsonPropertyName("aud")] public string Audience { get; set; }
            [JsonPropertyName("exp")] public long Expiration { get; set; }
            [JsonPropertyName("iat")] public long IssuedAt { get; set; }
        }

        public string ToAssertion()
        {
            var header = new JwtHeader
            {
                Algorithm = "RS256",
                Type = "JWT",
            };

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var claim = new JwtClaim
            {
                Issuer = _serviceAccount.ClientEmail,
                Scope = PubSubScope,
                Audience = "https://www.googleapis.com/oauth2/v4/token",
                Expiration = now + 3600,
                IssuedAt = now,
            };

            string headerJson = JsonSerializer.Serialize(header);
            string claimJson = JsonSerializer.Serialize(claim);

            string request = $"{ToBase64WithoutPadding(headerJson)}.{ToBase64WithoutPadding(claimJson)}";
            byte[] requestBytes = Encoding.ASCII.GetBytes(request);

            var sigBytes = _privateKey.SignData(requestBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var signature = Convert.ToBase64String(sigBytes).Replace("=", "");
            return $"{request}.{signature}";
        }

        private static RSA ParsePrivateKey(string privateKey)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);
            return rsa;
        }

        private static String ToBase64WithoutPadding(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes).Replace("=", "");
        }
    }
}