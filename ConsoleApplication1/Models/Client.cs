namespace Project53.New_arhtech
{
	public class Client
	{
		public Client()
		{
		}
		
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public decimal Balance { get; set; }

		public Client(decimal balance, string email)
		{
			Balance = balance;
			Email = email;
		}
	}
}