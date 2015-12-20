using System;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.OptionsModel;
using ToDoApp.Models;

namespace ToDoApp.Services
{
    public class DocumentDbContext : IDocumentDbContext
    {
        public DocumentDbContext(IOptions<DocumentDbOptions> options)
        {
            this.Options = options;
        }

        public DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    Uri endpointUri = new Uri(Options.Value.EndPoint);
                    client = new DocumentClient(endpointUri, Options.Value.AuthKey, new ConnectionPolicy {
                      ConnectionMode = ConnectionMode.Direct,
                      ConnectionProtocol = Protocol.Tcp
                    });
                }

                return client;
            }
        }

        public DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        public string CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(collectionId))
                {

                    collectionId = Options.Value.Collection;
                }

                return collectionId;
            }
        }

        public Database Database
        {
            get
            {
                if (database == null)
                {
                    database = ReadOrCreateDatabase();
                }
                return database;
            }
        }

        public string DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(databaseId))
                {
                    databaseId = Options.Value.Database;
                }
                return databaseId;
            }
        }

        private Database ReadOrCreateDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();
            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }
            return db;
        }

        private DocumentCollection ReadOrCreateCollection(string databaseLink)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();
            if (col == null)
            {
                col = Client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = CollectionId }).Result;
            }
            return col;
        }

        IOptions<DocumentDbOptions> Options { get; set; }

        private DocumentClient client;
        private string collectionId;
        private DocumentCollection collection;
        private string databaseId;
        private Database database;
    }
}
