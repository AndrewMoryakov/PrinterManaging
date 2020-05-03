using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Serilog.Core;

namespace Project53.New_arhtech.Http.Server
{
	public static class RestFullServer
	{
		public static async Task<IDisposable> StartServer()
		{
			//Startup startup = new Startup();
			//string baseAddress = "http://localhost:9011/";

			//IDisposable webApplication = WebApp.Start(baseAddress, startup.Configuration);
			//Console.ReadLine();

			//return webApplication;
			//string baseAddress = "http://localhost:9011/";

			//using (WebApp.Start<Startup>(url: baseAddress))
			//{
			//	var handler = new HttpClientHandler
			//	{
			//		UseDefaultCredentials = true
			//	};
			//	using (var client = new HttpClient(handler))
			//	{
			//		client.BaseAddress = new Uri(baseAddress);
			//		var response = await client.GetAsync("/api/Values");

			//		Console.ReadLine();
			//	}
			//}

			//return null;

			IDisposable disp = null;
			try
			{
				// Logger _logger = LogManager.GetCurrentClassLogger();

				return await Task<IDisposable>.Run(() =>
				{
					var handler = new HttpClientHandler
					{
						UseDefaultCredentials = true
					};
					string baseAddress = "http://localhost:9011/";

					disp = WebApp.Start<Startup>(url: baseAddress);
					var r = new HttpClient(handler).GetAsync(baseAddress + "api/values/get").Result;
					// _logger.Info(r.ToString());
					//Console.ReadLine();

					return disp;
				});
			}
			catch (Exception ex)
			{
				// LogManager.GetCurrentClassLogger().WriteException(ex);
				return disp;
			}
		}
	}
}