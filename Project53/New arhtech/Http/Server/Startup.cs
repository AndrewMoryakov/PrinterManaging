using System;
using System.Web.Http;
using Owin;


namespace Project53.New_arhtech.Http.Server
{
	public class Startup
	{
		// This code configures Web API. The Startup class is specified as a type
		// parameter in the WebApp.Start method.
		public void Configuration(IAppBuilder appBuilder)
		{
			var config = new HttpConfiguration();
			
			try
			{
				config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{token}", new
				{
					token = RouteParameter.Optional
				});

				appBuilder.UseWebApi(config);

			}
			catch (Exception ex)
			{
				// LogManager.GetCurrentClassLogger().WriteException(ex);
				throw;
			}
		}
	}
}