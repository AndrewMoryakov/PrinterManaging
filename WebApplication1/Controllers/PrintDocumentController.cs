using System;
using System.Linq;
using System.Threading.Tasks;
using DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.Controllers.Helpers;
using WebApplication1.DataModel;

namespace WebApplication1.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    // [Authorize]
    [ApiController]
    public class PrintDocument : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _appDbContext;

        public PrintDocument(
            IUserService userService,
            ApplicationDbContext appDbCntxt,
            IConfiguration cnfg)
            => (_appDbContext, _configuration, _userService) 
                = (appDbCntxt, cnfg, userService);
        
        [HttpPost]
        public async Task<ActionResult> Post(PrintedDocument printedDocument)
        {
            var costOfOnePage = Convert.ToDecimal(
                _configuration
                .GetSection("costOnePage").Value);

            var fullCost = costOfOnePage = printedDocument.AmountOfPages;
            var email = _userService.GetEmailCurrentUser();
            
            using (_appDbContext)
            {
                var user = _appDbContext.Users
                    .FirstOrDefault(el => el.Email == email);

                user.Balance -= fullCost;
                _appDbContext.Users.Update(user);
                await _appDbContext.SaveChangesAsync();
            }
            
            return new OkResult();
        }
    }
}