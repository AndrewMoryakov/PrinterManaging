using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApplication1.Controllers.Helpers;

namespace WebApplication1.Controllers
{
	[Produces("application/json")]
	[AllowAnonymous]
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		
		private readonly IUserService _userService;

		public AuthController(IUserService userService)
		{
			_userService = userService;
		}
		
		[HttpGet]
		public async Task<ActionResult<object>> Token()
		{
			string username;
			string pswrd;
			try
			{
				pswrd = Request.Headers["password"][0];
				username = Request.Headers["username"][0];
				var user = await _userService.GetUserByUsernameOrEmailAsync(username.ToUpper());
				if (user != null)
				{
					if (_userService.VerifyHashedPassword(user, pswrd) == PasswordVerificationResult.Success)
					{
						var roles = await (_userService as UserHelper.UserService).UserManager.GetRolesAsync(user);

						var creds = new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);

						var claimsIdentity = GetIdentity(user.UserName, user.Id, roles.First(), user.Email);
						var now = DateTime.UtcNow;
						var jwt = new JwtSecurityToken(
							notBefore: now,
							issuer: AuthOptions.ISSUER,
							audience: AuthOptions.AUDIENCE,
							claims: claimsIdentity.Claims,
							expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
							signingCredentials: creds
						);

						var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
						var response = new
						{
							access_token = encodedJwt
						};

						//await HttpContext.SignInAsync(scheme: JwtBearerDefaults.AuthenticationScheme, principal: new ClaimsPrincipal(claimsIdentity));
						
						Response.ContentType = "application/json";
						return response;

					}
				}
			}
			catch (Exception ex)
			{
				// _loger.LogError(ex.StackTrace);
			}
			
			return new StatusCodeResult(403);
		}
		
		private ClaimsIdentity GetIdentity(string user, string userId, string role, string email)
		{
			//Person person = people.FirstOrDefault(x => x.Login == username && x.Password == password);
			//if (person != null)
			{
				var claims = new[]
				{
					new Claim(ClaimsIdentity.DefaultNameClaimType, user),
					new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
					new Claim(ClaimsIdentity.DefaultIssuer, role),
					new Claim(ClaimTypes.Email, email),
					new Claim(ClaimTypes.NameIdentifier, userId),
				};

				ClaimsIdentity claimsIdentity =
					new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
						ClaimsIdentity.DefaultRoleClaimType);

				return claimsIdentity;
			}

			return null;
		}
	}
}