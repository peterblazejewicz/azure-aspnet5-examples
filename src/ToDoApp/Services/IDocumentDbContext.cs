using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ToDoApp.Services
{
    public interface IDocumentDbContext
    {
        String DatabaseId { get; }

        String CollectionId { get; }

        Database Database { get; }

        DocumentCollection Collection { get; }

        DocumentClient Client { get; }
    }
}
