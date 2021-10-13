using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using PubSubRestLib;

namespace PubSubSubscriber
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: dotnet run service_account.json subscription");
                return;
            }

            var subscription = args[1];

            var credential = new Credential(args[0]);
            var client = new PubSubClient();
            var messages = await client.PullMessages(credential, subscription, 1);
            if (messages.Count == 0)
            {
                Console.WriteLine("No Messages!!");
                return;
            }

            foreach (var msg in messages)
            {
                var attr = JsonSerializer.Serialize(msg.Attributes);
                Console.WriteLine($"Data: {msg.Data}, Attrs: {attr}");
            }
        }
    }
}