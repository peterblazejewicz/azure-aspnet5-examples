var azure = require("azure-storage");
//
var nconf = require("nconf");
nconf.env()
  .file({
    file: "config.json",
    search: true
  });
var QueueName = "my-queue";
var storageName = nconf.get("StorageName");
var storageKey = nconf.get("StorageKey");
var dev = nconf.get("NODE_ENV");
//
var retryOperations = new azure.ExponentialRetryPolicyFilter();
var queueSvc = azure.createQueueService(storageName,
  storageKey).withFilter(retryOperations);
if(queueSvc) {
  queueSvc.createQueueIfNotExists(QueueName, function(error, results, response) {
    if(error) {
      return;
    }
    var created = results;
    if(created) {
      console.log("created new queue");
    } else {
      console.log("queue already exists");
    }
    var ticket = {
      EventId:  4711,
      Email: "peter@example.com",
      NumberOfTickets: 2,
      OrderDate: Date.UTC
    };
    var msg = JSON.stringify(ticket);
    queueSvc.createMessage(QueueName, msg, function(error, result, response) {
      if(error) {
        return;
      }
      queueSvc.peekMessages(QueueName, {
        numOfMessages: 32
      }, function(error, result, response){
        if(!error){
          // Message text is in messages[0].messagetext
        }
      });
    });
  });
}