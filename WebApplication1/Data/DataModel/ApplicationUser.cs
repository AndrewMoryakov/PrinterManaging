using Microsoft.AspNetCore.Identity;

namespace WebApplication1.DataModel
{
	public class ApplicationUser : IdentityUser
	{
		public string  FirstName { get; set; }
		public string  LastName { get; set; }
		public override string Email { get; set; }
		public IdentityUserRole<string> Role { get; set; }
	}
}