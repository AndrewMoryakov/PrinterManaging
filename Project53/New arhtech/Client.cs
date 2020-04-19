namespace Project53.New_arhtech
{
	public class Client
	{
		public decimal Balance { get; private set; }
		public string Email { get; private set; }

		public Client(decimal balance, string email)
		{
			Balance = balance;
			Email = email;
		}
	}
}