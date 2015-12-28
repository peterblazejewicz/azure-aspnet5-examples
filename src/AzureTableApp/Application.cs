using System;
using System.Linq;
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
        Task AllEntities();
        Task InsertBatch();
        Task SingleEntity();
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
        /*
          https://azure.microsoft.com/en-us/documentation/articles/vs-storage-aspnet5-getting-started-tables/
          To query a table for all of the entities in a partition, use a TableQuery object. The following code example specifies a filter for entities where 'Smith' is the partition key. This example prints the fields of each entity in the query results to the console.
        */
        public async Task AllEntities()
        {
          // Construct the query operation for all customer entities where PartitionKey="Smith".
          TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>()
            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));
          // Print the fields for each customer.
          TableContinuationToken token = null;
          do
            {
                TableQuerySegment<CustomerEntity> resultSegment = await peopleTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;
                foreach (CustomerEntity entity in resultSegment.Results)
                {
                    Log.LogInformation($"{entity.PartitionKey}, {entity.RowKey}\t{entity.Email}\t{entity.PhoneNumber}");
                }
            } while (token != null);
        }
        /*
          https://azure.microsoft.com/en-us/documentation/articles/vs-storage-aspnet5-getting-started-tables/
          You can insert multiple entities into a table in a single write operation. The following code example creates two entity objects ("Jeff Smith" and "Ben Smith"), adds them to a TableBatchOperation object using the Insert method, and then starts the operation by calling CloudTable.ExecuteBatchAsync.
        */
        public async Task InsertBatch()
        {
            Log.LogInformation("InsertBatch");
            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();
            // Create a customer entity and add it to the table.
            CustomerEntity customer1 = Customer.CreateCustomerEntity();
            customer1.PartitionKey = "Smith";
            // Create another customer entity and add it to the table.
            CustomerEntity customer2 = Customer.CreateCustomerEntity();
            customer2.PartitionKey = "Smith";
            // Add both customer entities to the batch insert operation.
            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);
            // Execute the batch operation.
            var results = await peopleTable.ExecuteBatchAsync(batchOperation);
            if (results.Any())
            {
                Log.LogInformation("Inserted");
                foreach (var result in results)
                {
                    Log.LogInformation("Added: " + JsonConvert.SerializeObject(result.Result));
                }
            }
        }
        /*
          https://azure.microsoft.com/en-us/documentation/articles/vs-storage-aspnet5-getting-started-tables/
          You can write a query to get a single, specific entity. The following code uses a TableOperation object to specify a customer named 'Bennett Smith'. This method returns just one entity, rather than a collection, and the returned value in TableResult.Result is a CustomerEntity object. Specifying both partition and row keys in a query is the fastest way to retrieve a single entity from the Table service.
        */
        public async Task SingleEntity()
        {
          Log.LogInformation("SingleEntity");
          // Create a retrieve operation that takes a customer entity.
          TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Bennett");
          // Execute the retrieve operation.
          TableResult retrievedResult = await peopleTable.ExecuteAsync(retrieveOperation);
          // Print the phone number of the result.
          if (retrievedResult.Result != null) {
            CustomerEntity customer = (CustomerEntity)retrievedResult.Result;
            Log.LogInformation($"Retrieved phone number: {customer.PhoneNumber}");            
          } else 
          {
            Log.LogWarning("The phone number could not be retrieved."); 
          }
        }
        //
        private AzureStorageOptions Options { get; }
        CloudTable peopleTable = null;
        private ILogger Log { get; } = Logger.Get();

    }
}
