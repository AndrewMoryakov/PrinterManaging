using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
	public class ConfirmEmailController : ComponentBase
	{
		public async Task<ActionResult<bool>> Login(string code, string email)
		{
			if ((code != null || email != null)
			    &&
			    (code != "" || email != "")
			)

				return true;

			return false;
		}
	}
}