# Azure Queue example application

An example console application based on topics discussed in [How to use Queue storage from .NET](https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-queues/).

## Implementation

The application showcases different operations on the Azure Storage Queue. You could use following command line switches to execute implemented operations:

- BatchRemove
```
dnx run --operation BatchRemove
```
This operations removes and processes a set of Tickets from the queue in batch operation (up to 20 tickets)

- ChangeMessage
```
dnx run --operation ChangeMessage
```
This operation modify a ticket from queue (we add a bonus, free ticket to the order)

- ClearMessages
```
dnx run --operation ClearMessages
```
This operations empties the queue removing all non-processed tickets.

- InsertMessage
```
dnx run --operation InsertMessage
```
This operation inserts a ticket request object into queue.

- GetLength
```
dnx run --operation GetLenght
```
This operation retrieves cached queue length

- PeekMessage
```
dnx run --operation PeekMessage
```
This operation peeks the first (FIFO) ticket request from queue

- RemoveMessage
```
dnx run --operation RemoveMessage
```
This operation gets a first (FIFO) ticket from queue, process it and delete from queue

## Example output from running application
```
dnx run --operation PeekMessage
info: AzureQueueApp.Program[0]
      starting
info: AzureQueueApp.Program[0]
      Azure configuration
info: AzureQueueApp.Program[0]
      account: {REDACTED} key: {REDACTED} queue: tickets-queue
info: AzureQueueApp.Program[0]
      Queue tickets-queue CreateIfNotExists=False
info: AzureQueueApp.Program[0]
      PeekMessage
info: AzureQueueApp.Program[0]
      Ticket: #1721701070 for: peter@example.com date: 12/26/2015 4:27:08 PM total: 1
Press a key to exit ...
```

### Configuration 

User Secrets and appsettings.json are used in configuration:

The application uses configuration to access Azure information in the following order:
- `appsettings.json`
- user secrets 
- environment variables

The configuration file or user secrets section-based keys:

```
user-secret list
info: Azure:Storage:AccountName = ...
info: Azure:Storage:AccountKey = ....
info: Azure:Storage:QueueName = queue-name
```

## Author
@peterblazejewicz