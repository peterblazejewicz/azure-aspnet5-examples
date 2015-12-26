using System;

namespace AzureQueueApp.Models
{
    public class TicketRequest
    {
        public int TicketId { get; set; }
        public string Email { get; set; }
        public int NumberOfTickets { get; set; }
        public DateTime OrderDate { get; set; }
		
    }
}
