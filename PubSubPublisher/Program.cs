using System;
using System.Threading.Tasks;
using PubSubRestLib;

namespace PubSubPublisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet run service_account.json");
                return;
            }

            var credential = new Credential(args[0]);
            var assertion = credential.ToAssertion();

            var accessTokenClient = new AccessTokenClient();
            var accessToken = await accessTokenClient.GetAccessToken(assertion);
            Console.WriteLine($"accessToken={accessToken}");
        }
    }
}