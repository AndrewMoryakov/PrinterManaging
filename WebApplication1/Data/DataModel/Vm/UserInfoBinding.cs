namespace WebApplication1.DataModel
{
	public class UserInfoBinding
	{
		public UserInfoBinding()
		{
		}
		
		public UserInfoBinding(string email, string firstName, string lastName)
		{
			Email = email;
			FirstName = firstName;
			LastName = lastName;
		}
		
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}