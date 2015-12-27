using System;
using System.Threading.Tasks;
using AzureTableApp.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableApp
{
    public interface IApplication
    {
        Task<IApplication> InitializeAsync();
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
            // storage credentials from IOptions
            StorageCredentials credentials = new StorageCredentials(Options.AccountName, Options.AccessKey);
            // Retrieve storage account from credentials
            CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, true);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Get a reference to a table
            table = tableClient.GetTableReference(Options.TableName);
            // Create the CloudTable if it does not exist
            bool created = await table.CreateIfNotExistsAsync();
            return this;
        }
        //
        private AzureStorageOptions Options { get; }
        CloudTable table = null;

    }
}
