using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
	public class NativeController : ControllerBase
	{
		public void Execute(HttpRequest requestContext)
		{

			string key = "axJe2mNmoKetQpgroIoLB+CE";
			dynamic paramss = requestContext.HttpContext.Request.Body;
			//string email = "test";
			//for (int i = 0; i < requestContext.HttpContext.Request.Params.Count; i++)
			//{

			//	LogManager.GetCurrentClassLogger().Log(LogLevel.Info, paramss.GetKey(i) + " " + paramss[i]);
			//	if (paramss.GetKey(i) == "label")
			//	{
			//		LogManager.GetCurrentClassLogger().Log(LogLevel.Info, $"Before trim {paramss[i]}");
			//		email = paramss[i].ToLower().Trim();
			//		LogManager.GetCurrentClassLogger().Log(LogLevel.Info, $"After trim {email}");
			//		break;
			//	}
			//}


			string paramString = String.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}&{7}&{8}",
	   paramss["notification_type"], paramss["operation_id"], paramss["amount"], paramss["currency"],
	   paramss["datetime"], paramss["sender"],
	   paramss["codepro"].ToString().ToLower(), key, paramss["label"]);

			string paramStringHash1 = GetHash(paramString);

			// LogManager.GetCurrentClassLogger().Log(LogLevel.Info, "Yandex hash:" + paramStringHash1);
			// LogManager.GetCurrentClassLogger().Log(LogLevel.Info, "Server hash:" + paramss["sha1_hash"]);

			StringComparer comparer = StringComparer.OrdinalIgnoreCase;
			if (comparer.Compare(paramStringHash1, (string)paramss["sha1_hash"]) == 0)
			{
				var email = paramss["label"].ToLower().Trim();
				var amount = paramss["amount"];
				// LogManager.GetCurrentClassLogger().Log(LogLevel.Info, $" Fund for { email} on {amount}");
				// using (var db = new ApplicationDbContext())
				// {
				// 	var userId = db.Users.FirstOrDefault(el => el.Email == email);
				// 	// UserHelper.ChangeInvoice(new FundAccountBindingModel
				// 	// {
				// 	// 	Email = userId?.Email,
				// 	// 	Amount = amount,
				// 	// 	IdOfCurrentUser = userId?.Id
				// 	// }, db);
				// }
			}
			// for (int i = 0; i < requestContext.HttpContext.Request.Params.Count; i++)
			// {
			// 	LogManager.GetCurrentClassLogger().Log(LogLevel.Info, "New 1");
			// 	LogManager.GetCurrentClassLogger().Log(LogLevel.Info, paramss.GetKey(i) + paramss[i]);
			// }
		}

		private string GetHash(string val)
		{
			SHA1 sha = new SHA1CryptoServiceProvider();
			byte[] data = sha.ComputeHash(Encoding.Default.GetBytes(val));

			StringBuilder sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}
	}
}