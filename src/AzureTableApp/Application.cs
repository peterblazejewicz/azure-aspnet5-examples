using System;
using System.Threading.Tasks;
using AzureTableApp.Models;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AzureTableApp
{
    public interface IApplication
    {
        Task<IApplication> InitializeAsync();
		Task AddEntity();
    }
    public class Application : IApplication
    {
        public static Task<IApplication> CreateAsync(AzureStorageOptions options)
        {
            var app = new Application(options);
            return app.InitializeAsync();
        }

        Application(AzureStorageOptions options) { Options = options; }
        public async Task<IApplication> InitializeAsync()
        {
            Log.LogInformation("Initializing Table storage access");
            Log.LogInformation($"Azure options: AccountName:{Options.AccountName} AccessKey:{Options.AccessKey} TableName:{Options.TableName}");
            // storage credentials from IOptions
            StorageCredentials credentials = new StorageCredentials(Options.AccountName, Options.AccessKey);
            // Retrieve storage account from credentials
            CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, true);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Get a reference to a table
            peopleTable = tableClient.GetTableReference(Options.TableName);
            // Create the CloudTable if it does not exist
            bool created = await peopleTable.CreateIfNotExistsAsync();
            if (created == true)
            {
                Log.LogInformation($"Table {Options.TableName} created");
            }
            else
            {
                Log.LogInformation($"Table {Options.TableName} already exists");
            }
            return this;
        }
		/*
			https://azure.microsoft.com/en-us/documentation/articles/vs-storage-aspnet5-getting-started-tables/
			Table operations involving entities are done using the CloudTable object you created earlier in "Access tables in code." The TableOperation object represents the operation to be done. The following code example shows how to create a CloudTable object and a CustomerEntity object. To prepare the operation, a TableOperation is created to insert the customer entity into the table. Finally, the operation is executed by calling CloudTable.ExecuteAsync.
		*/
		public async Task AddEntity()
		{
			Log.LogInformation("AddEntity");
			// Create a new customer entity.
			CustomerEntity entity = Customer.CreateCustomerEntity();
			// Create the TableOperation that inserts the customer entity.
			TableOperation insertOperation = TableOperation.Insert(entity);
			// Execute the insert operation.
			var results = await peopleTable.ExecuteAsync(insertOperation);
			Log.LogInformation("Added: " + JsonConvert.SerializeObject(results.Result));	
		}
        //
        private AzureStorageOptions Options { get; }
        CloudTable peopleTable = null;
        private ILogger Log { get; } = Logger.Get();

    }
}
