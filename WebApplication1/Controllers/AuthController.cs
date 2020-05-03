using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
	[Produces("application/json")]
	[AllowAnonymous]
	[ApiController]
	public class AuthController : ControllerBase
	{
		[HttpPost("/token")]
		public async Task<ActionResult<string>> Token()
		{
			try
			{
				var username = Request.Form["username"];
				var password = Request.Form["password"];
			}
			catch (Exception ex)
			{

			}

			return new Guid().ToString();
		}
	}
}