using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using AzureQueueApp.Models;
using GenFu;

namespace AzureQueueApp
{
    /*
		Common interface for this application
	*/
    public interface IApplication
    {
        // change content of message in the queue
        void ChangeMessage();
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
            storageCredentials = new StorageCredentials(options.AccountName, options.AccountKey);
            Logger.Get().LogInformation("Azure configuration");
            Logger.Get().LogInformation($"account: {options.AccountName} key: {options.AccountKey} queue: {options.QueueName}");
            bool useHttps = true;
            // Retrieve storage account from credentials
            storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();
            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference(options.QueueName);
            // Create the queue if it doesn't already exist
            bool created = queue.CreateIfNotExists();
            Logger.Get().LogInformation($"Queue {queue.Name} CreateIfNotExists={created}");
        }
        public void ChangeMessage()
        {
            Logger.Get().LogInformation("ChangeMessage");
            // Peek at the next message
            CloudQueueMessage message = queue.GetMessage();
            if (message != null)
            {
                TicketRequest ticket = JsonConvert.DeserializeObject<TicketRequest>(message.AsString);
                LogTicketRequest(ticket);
                // add a free ticket :)
                ticket.NumberOfTickets = ticket.NumberOfTickets + 1;

                string json = JsonConvert.SerializeObject(ticket);
                message.SetMessageContent(json);
                queue.UpdateMessage(message,
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
        public void InsertMessage()
        {
            Logger.Get().LogInformation("InsertMessage");
            TicketRequest ticket = A.New<TicketRequest>();
            string json = JsonConvert.SerializeObject(ticket);
            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(json);
            queue.AddMessage(message);
            LogTicketRequest(ticket);
        }

        public void PeekMessage()
        {
            Logger.Get().LogInformation("PeekMessage");
            // Peek at the next message
            CloudQueueMessage msg = queue.PeekMessage();
            if (msg != null)
            {
                TicketRequest ticket = JsonConvert.DeserializeObject<TicketRequest>(msg.AsString);
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
