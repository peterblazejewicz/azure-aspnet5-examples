using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace QueueGettingStarted
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
    }
}
