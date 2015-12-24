var azure = require("azure-storage");
//
var nconf = require("nconf");
nconf.env()
  .file({
    file: "config.json",
    search: true
  });
var storageName = nconf.get("StorageName");
var storageKey = nconf.get("StorageKey");
var dev = nconf.get("NODE_ENV");
