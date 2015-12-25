using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace AzureQueueApp
{
    /*
		Common interface for this application
	*/
    public interface IApplication
    {
        // insert a single message into queue
        void InsertMessage();
        // peek a single message from the queue 
        void PeekMessage();
    }
    public class Application : IApplication
    {
        public Application(AzureStorageOptions options)
        {
            this.options = options;
            Console.WriteLine(JsonConvert.SerializeObject(options));
            storageCredentials = new StorageCredentials(options.AccountName, options.AccountKey);
            bool useHttps = true;
            // Retrieve storage account from credentials
            storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();
            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference(options.QueueName);
            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();
        }
        public void InsertMessage()
        {
            throw new NotImplementedException();
        }

        public void PeekMessage()
        {
            throw new NotImplementedException();
        }
        private AzureStorageOptions options;
        private StorageCredentials storageCredentials;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue queue;
    }
}
