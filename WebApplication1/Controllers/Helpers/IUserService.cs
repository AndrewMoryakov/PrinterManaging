using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApplication1.DataModel;

namespace WebApplication1.Controllers.Helpers
{
	public interface IUserService
	{
		string GetCurrentUserId();
		string GetEmailCurrentUser();
		Task<IdentityResult> CreateUser(ApplicationUser user, string password);
		Task<IList<Claim>> GetClaims(ApplicationUser user);
		Task<ApplicationUser> GetUserByUsernameOrEmailAsync(string username);
		//Task<ApplicationUser> GetUserByPublicId(string username);
		Task<ApplicationUser> GetUserById(string id);
		PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string password);
		ApplicationUser GetUser();
		ApplicationUser GetUser(string id);
		Task<IList<string>> GetRolesAsync();
		Task<IList<string>> GetRolesAsync(string userId);
		//IList<string> GetRoles(string id);
		//Task AddToTelegram(Telegram.Bot.Types.Message msg);
	}
}