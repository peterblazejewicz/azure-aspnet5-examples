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
        Task InsertMessage();
        // peek a single message from the queue 
        Task PeekMessage();
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
        public async Task InsertMessage()
        {
            Logger.Get().LogInformation("InsertMessage");
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
