#nullable disable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers.Helpers;
using WebApplication1.DataModel;

namespace WebApplication1.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	// [Authorize]
	[ApiController]
	public class UserController : ControllerBase
	{
			
		private readonly IUserService _userService;

		public UserController(IUserService userService) =>
			(_userService) = (userService);

		[HttpGet]
		public ActionResult<UserInfoVm> Get()
		{
			var user = _userService.GetUser();
			if(user != null)
				return new UserInfoVm
				{
					Balance = user.Balance,
					Email = user.Email,
					FirstName = user.FirstName,
					LastName = user.LastName
				};
			
			return BadRequest();
		}
	}
}