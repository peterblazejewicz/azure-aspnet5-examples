using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QueueGettingStarted
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AsyncTask(args)
                .ContinueWith((task) =>
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }).Wait();
        }

        public async static Task AsyncTask(string[] args)
        {
            // configuration
            var builder = new ConfigurationBuilder()
                .AddJsonFile("./appsettings.json")
                .AddUserSecrets()
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            // options
            ConfigurationBinder.Bind(Configuration.GetSection("Azure:Storage"), Options);
            Console.WriteLine("Queue encryption sample");
            Console.WriteLine($"Configuration for ConnectionString: {Options.ConnectionString}");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Options.ConnectionString);
            CloudQueueClient client = storageAccount.CreateCloudQueueClient();
            Debug.Assert(client != null, "Client created");
            string hash = Guid.NewGuid().ToString("N");
            CloudQueue queue = client.GetQueueReference($"{Options.DemoQueue}{hash}");
            try
            {
                await queue.CreateAsync();
                string messageStr = Guid.NewGuid().ToString();
                CloudQueueMessage message = new CloudQueueMessage(messageStr);
                // Add message
                Console.WriteLine($"Inserting the message: {message}");
                await queue.AddMessageAsync(message, null, null, null, null);
                // Retrieve message
                Console.WriteLine("Retrieving the message.");
                CloudQueueMessage retrMessage = await queue.GetMessageAsync();
                // Update message
                Console.WriteLine("Updating the message.");
                string updatedMessage = Guid.NewGuid().ToString("N");
                retrMessage.SetMessageContent(updatedMessage);
                await queue.UpdateMessageAsync(retrMessage, TimeSpan.FromSeconds(0), MessageUpdateFields.Content | MessageUpdateFields.Visibility, null, null);
                // Retrieve updated message
                Console.WriteLine("Retrieving the updated encrypted message.");
                retrMessage = await queue.GetMessageAsync();
            }
            finally
            {
                bool deleted = await queue.DeleteIfExistsAsync();
                Console.WriteLine($"Queue deleted: {deleted}");
            }
        }

        static IConfiguration Configuration { get; set; }
        static AzureStorageOptions Options { get; set; } = new AzureStorageOptions();
    }

    class AzureStorageOptions
    {
        public string ConnectionString { get; set; }
        public string DemoQueue { get; set; }
    }
}
