#nullable disable
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers.Helpers;
using WebApplication1.DataModel;

namespace WebApplication1.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class UserController : ControllerBase
	{
			
		private readonly IUserService _userService;

		public UserController(IUserService userService) =>
			(_userService) = (userService);

		[HttpGet]
		public async Task<ActionResult<UserInfoVm>> Get()
		{
			return new UserInfoVm("username@gmail.com", "Andrew", "Moryakov", 10m);

			
			
			
			return BadRequest();
		}
	}
}