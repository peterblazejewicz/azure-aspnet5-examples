var express = require('express');
var router = express.Router();

var azure = require('azure-storage');
var nconf = require('nconf');
nconf.env()
  .file({ file: 'config.json', search: true });
  
var tableName = nconf.get("TABLE_NAME");
var partitionKey = nconf.get("PARTITION_KEY");
var accountName = nconf.get("STORAGE_NAME");
var accountKey = nconf.get("STORAGE_KEY");

/* GET home page. */
router.get('/', function (req, res, next) {
  res.render('index', { title: 'Express' });
});

module.exports = router;
