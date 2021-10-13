using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PubSubRestLib;

namespace PubSubPublisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: dotnet run service_account.json topic message");
                return;
            }

            var topic = args[1];
            var message = args[2];

            var attributes = new Dictionary<string, string>
            {
                {"name", "John"},
                {"age", "99"},
            };

            var credential = new Credential(args[0]);
            var client = new PubSubClient();
            var messageIds = await client.PublishMessage(credential, topic, message, attributes);
            Console.WriteLine($"accessToken={messageIds}");
        }
    }
}