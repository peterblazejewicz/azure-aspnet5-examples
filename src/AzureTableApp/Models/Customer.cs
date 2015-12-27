using GenFu;

namespace AzureTableApp.Models
{
    public class Customer
    {
        public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }

        public static CustomerEntity CreateCustomerEntity()
		{
			A.Configure<Customer>()
				.Fill(c => c.Email)
				.AsEmailAddressForDomain("example.com")
				.Fill(c => c.FirstName)
				.AsFirstName()
				.Fill(c => c.LastName)
				.AsLastName()
				.Fill(c => c.PhoneNumber)
				.AsPhoneNumber();
			var customer = A.New<Customer>();
			var customerEntity = new CustomerEntity(customer.FirstName, customer.LastName)
			{
				PhoneNumber = customer.PhoneNumber,
				Email = customer.Email
			};
			return customerEntity;
		}
    }
}
