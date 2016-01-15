using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Extensions.Configuration;

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
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("");
            CloudQueueClient client = storageAccount.CreateCloudQueueClient();
            if(client != null){
                Console.WriteLine("Client created");
            } else {
                Console.WriteLine("Error creating client");
            }
            Console.ReadKey();
        }
        
        static IConfiguration Configuration { get; set; }
        static AzureStorageOptions Options {get; set; } = new AzureStorageOptions();
    }
    
    class AzureStorageOptions {
        public string ConnectionString { get; set; }
    }
}
