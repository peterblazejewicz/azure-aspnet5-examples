# Azure ASP.NET5 Examples

Various examples for MS Azure using ASP.NET 5.

## Examples

### [Azure Storage Queue](/src/AzureQueueApp)

Example ASP.NET5 console application demonstrates following topics:
- Azure Storage Queue operations
- Async/Await patterns
- Configuration
- User Secrets
- commandline arguments

The example takes some code ideas and parts from:
- [How to use Queue storage from .NET](https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-queues/#leverage-additional-options-for-de-queuing-messages)
- [Microsoft Azure Storage Queues Part 2: Hands on Working with Queues](http://justazure.com/microsoft-azure-storage-queues-part-2-working-queues/)
- [Azure Queues 103 -  Batch Processing with Mark Simms](https://channel9.msdn.com/Shows/Azure-Friday/Azure-Queues-103-Batch-Processing-with-Mark-Simms)


### :construction: [DocumentDb ToDoApp](/src/ToDoApp) [WIP]

This is ASP.NET5 MVC6 web application that uses Azure DocumentDb as backing store.
At the moment the application is broken as not all parts of Azure Document Db SDK are x-plat:
https://social.msdn.microsoft.com/Forums/azure/en-US/a4a80fde-5282-480a-b981-2bf5bb5f64c9/missing-bcryptdll-out-of-the-box-on-net-5-mono-linux?forum=AzureDocumentDB

The code is based on [Web application development with ASP.NET MVC using DocumentDB](https://azure.microsoft.com/en-us/documentation/articles/documentdb-dotnet-application/)

This project is marked as WIP due to x-plat problems.

## Author

@peterblazejewicz
