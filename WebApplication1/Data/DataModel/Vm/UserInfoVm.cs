namespace WebApplication1.DataModel
{
	public class UserInfoVm
	{
		public UserInfoVm(string email, string firstName, string lastName, decimal balance)
		{
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Balance = balance;
		}
		
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public decimal Balance { get; set; }
	}
}