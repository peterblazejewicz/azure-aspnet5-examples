using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace QueueGettingStarted
{
    public class Program
    {
        public static void Main(string[] args)
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
            Console.WriteLine("Client created");
            Console.WriteLine($"queue: {Options.DemoQueue}");
            Console.ReadKey();
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
