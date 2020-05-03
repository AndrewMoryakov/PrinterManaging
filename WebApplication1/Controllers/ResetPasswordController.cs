using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
	public class ResetPasswordController : ControllerBase
	{
		// [HttpPost]
		// [AllowAnonymous]
		// [ValidateAntiForgeryToken]
		// public async Task<ActionResult> ResetPassword(string code, string email, string password)
		// {
		// 	// using (ApplicationDbContext db = new ApplicationDbContext())
		// 	// {
		// 	// 	var provider = new DpapiDataProtectionProvider("AppName");
		// 	//
		// 	// 	var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));// new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
		// 	// 	//var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
		// 	// 	userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
		// 	// 		provider.Create("PasswordReset"));
		// 	//
		// 	// 	var user = userManager.FindByEmail(email);
		// 	// 	if (user == null)
		// 	// 	{
		// 	// 		return RedirectToAction("Index", "Home");
		// 	// 	}
		// 	// 	code = code.Replace(" ", "+");
		// 	// 	var result = await userManager.ResetPasswordAsync(user.Id, code, password);
		// 	// 	if (result.Succeeded)
		// 	// 	{
		// 	// 		return RedirectToAction("Index", "Home");
		// 	// 	}
		// 	// 	return RedirectToAction("Index", "Home");
		// 	// }
		// }
	}
}