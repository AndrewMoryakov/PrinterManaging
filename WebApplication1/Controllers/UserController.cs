#nullable disable
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
		private readonly ILogger _logger;
		private readonly ApplicationDbContext _appDbContext;

		public UserController(
			IUserService userService,
			ILogger<UserController> logger, ApplicationDbContext appDbCntxt) =>
			(_userService, _logger, _appDbContext) = (userService, logger, appDbCntxt);

		[HttpGet]
		public ActionResult<UserInfoVm> Get()
		{
			var user = _userService.GetUser();
			_logger.LogInformation($"user: {user?.Email??"null"}" );
			
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

		[HttpPut]
		public ActionResult Update(UserInfoBinding userBinding)
		{
			var email = _userService.GetEmailCurrentUser();

			using (var dbContext = new ApplicationDbContext())
			{
				var usr = dbContext.Users.FirstOrDefault(el=>el.Email == email);
				usr = userBinding.Adapt<ApplicationUser>();
				dbContext.Users.Update(usr);
				dbContext.SaveChanges();
			}

			return new OkResult();
		}
	}
}