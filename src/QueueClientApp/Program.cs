using System;
using Microsoft.ServiceBus.Messaging;

namespace QueueClientApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
			//
			string connectionString = "Endpoint=sb://blazejewicz-queue.servicebus.windows.net/;SharedAccessKeyName=SendAndListenPolicy;SharedAccessKey=iPfTVwoYbJimLW8qtCDcgmi5mUIcay3gzN1TkhlxGmM=";
			QueueClient client = QueueClient.CreateFromConnectionString(connectionString);
			Console.Read();
			Environment.Exit(0);
			// Configure the callback options
			OnMessageOptions options = new OnMessageOptions {
				AutoComplete = false,
				AutoRenewTimeout = TimeSpan.FromMinutes(1)
			};
			client.OnMessage((message) => {
				try {
					// process message
					Console.WriteLine("Body: {0}", message.GetBody<string>());
					Console.WriteLine("MessageID: {0}", message.MessageId);
					Console.WriteLine("TestProperty: {0}", message.Properties["TestProperty"]);
					// remove from queue
					message.Complete();
				}
				catch(Exception ex)
				{
					// Indicates a problem, unlock message in queue
					message.Abandon();
					Console.WriteLine("Error: {0}", ex.Message);
				}
			}, options);
            Console.Read();
        }
    }
}
