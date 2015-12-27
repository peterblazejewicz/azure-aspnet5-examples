using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableApp.Models
{
    public class CustomerEntity: TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }
	
		public CustomerEntity() { }
	
		public string Email { get; set; }
	
		public string PhoneNumber { get; set; }
    }
}
