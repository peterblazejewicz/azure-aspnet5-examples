using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using AzureQueueApp.Models;
using GenFu;
using System.Threading.Tasks;
using System.Threading;

namespace AzureQueueApp
{
    /*
		Common interface for this application
	*/
    public interface IApplication
    {
        Task<IApplication> InitializeAsync();
        // change content of message in the queue
        Task ChangeMessage();
        // insert a single message into queue
        // gets lengths of the queue
        Task GetLength();
        Task InsertMessage();
        // peek a single message from the queue 
        Task PeekMessage();
        // Removes message from the queue
        Task RemoveMessage();
    }

    public class Application : IApplication
    {
        public static Task<IApplication> CreateAsync(AzureStorageOptions options)
        {
            var app = new Application(options);
            return app.InitializeAsync();
        }
        Application(AzureStorageOptions options)
        {
            this.options = options;
            storageCredentials = new StorageCredentials(options.AccountName, options.AccountKey);
            Logger.Get().LogInformation("Azure configuration");
            Logger.Get().LogInformation($"account: {options.AccountName} key: {options.AccountKey} queue: {options.QueueName}");
        }
        public async Task<IApplication> InitializeAsync()
        {
            bool useHttps = true;
            // Retrieve storage account from credentials
            storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();
            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference(options.QueueName);
            // Create the queue if it doesn't already exist
            bool created = await queue.CreateIfNotExistsAsync();
            Logger.Get().LogInformation($"Queue {queue.Name} CreateIfNotExists={created}");
            return this;
        }
        public async Task ChangeMessage()
        {
            Logger.Get().LogInformation("ChangeMessage");
            // Peek at the next message
            CloudQueueMessage message = await queue.GetMessageAsync();
            if (message != null)
            {
                TicketRequest ticket = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<TicketRequest>(message.AsString));
                LogTicketRequest(ticket);
                // add a free ticket :)
                ticket.NumberOfTickets = ticket.NumberOfTickets + 1;

                string json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(ticket));
                message.SetMessageContent(json);
                await queue.UpdateMessageAsync(message,
                    TimeSpan.FromSeconds(60.0),
                    MessageUpdateFields.Content | MessageUpdateFields.Visibility);
                // log change ticket
                LogTicketRequest(ticket);
            }
            else
            {
                Logger.Get().LogWarning($"The {queue.Name} appears to be empty");
            }
        }
        /*
            https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-queues/
            You can get an estimate of the number of messages in a queue. The FetchAttributes method asks the Queue service to retrieve the queue attributes, including the message count. The ApproximateMessageCount property returns the last value retrieved by the FetchAttributes method, without calling the Queue service.
        */
        public async Task GetLength()
        {
            Logger.Get().LogInformation("GetLength");
            // Fetch the queue attributes.
            await queue.FetchAttributesAsync();
            // Retrieve the cached approximate message count.
            int? cachedMessageCount = queue.ApproximateMessageCount;
            Logger.Get().LogInformation($"Number of messages in {queue.Name} queue: {cachedMessageCount}");
        }
        public async Task InsertMessage()
        {
            Logger.Get().LogInformation("InsertMessage");
            // GenFu configuration specific for our example
            A.Configure<TicketRequest>()
                .Fill(t => t.OrderDate)
                .AsFutureDate();
            A.Configure<TicketRequest>()
                .Fill(t => t.NumberOfTickets)
                .WithinRange(1, 10);
            A.Configure<TicketRequest>()
                .Fill(t => t.Email)
                .AsEmailAddressForDomain("example.com");
            TicketRequest ticket = A.New<TicketRequest>();
            string json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(ticket));
            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(json);
            // Async enqueue the message
            await Task.Factory.StartNew(() => queue.AddMessage(message));
            // @see https://github.com/Azure/azure-storage-net/issues/220
            // await queue.AddMessageAsync(message);
            LogTicketRequest(ticket);
        }

        public async Task PeekMessage()
        {
            Logger.Get().LogInformation("PeekMessage");
            // Peek at the next message
            CloudQueueMessage msg = await queue.PeekMessageAsync();
            if (msg != null)
            {
                TicketRequest ticket = await Task.Factory.StartNew(() =>
                    JsonConvert.DeserializeObject<TicketRequest>(msg.AsString)
                );
                LogTicketRequest(ticket);
            }
            else
            {
                Logger.Get().LogWarning($"The {queue.Name} appears to be empty");
            }
        }
        /*
            https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-queues/
            Your code de-queues a message from a queue in two steps. When you call GetMessage, you get the next message in a queue. A message returned from GetMessage becomes invisible to any other code reading messages from this queue. By default, this message stays invisible for 30 seconds. To finish removing the message from the queue, you must also call DeleteMessage. This two-step process of removing a message assures that if your code fails to process a message due to hardware or software failure, another instance of your code can get the same message and try again. Your code calls DeleteMessage right after the message has been processed.
        */
        public async Task RemoveMessage()
        {
            Logger.Get().LogInformation("RemoveMessage");
            // Get the next message
            CloudQueueMessage message = await queue.GetMessageAsync();
            if (message != null)
            {
                TicketRequest ticket = await Task.Factory.StartNew(() =>
                    JsonConvert.DeserializeObject<TicketRequest>(message.AsString)
                );
                LogTicketRequest(ticket);
                Logger.Get().LogInformation("Processing ticket");
                await Task.Factory.StartNew(() => Thread.Sleep(1000));
                Logger.Get().LogInformation("Finished processing ticket");
                await queue.DeleteMessageAsync(message);
                Logger.Get().LogInformation("Message removed");
            }
            else
            {
                Logger.Get().LogWarning($"The {queue.Name} appears to be empty");
            }
        }

        private void LogTicketRequest(TicketRequest ticket)
        {
            if (ticket == null)
            {
                Logger.Get().LogWarning("Failed to deserialize ticket");
                return;
            };
            Logger.Get().LogInformation($"Ticket: #{ticket.TicketId} for: {ticket.Email} date: {ticket.OrderDate} total: {ticket.NumberOfTickets}");
        }
        private AzureStorageOptions options;
        private StorageCredentials storageCredentials;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue queue;
    }
}
