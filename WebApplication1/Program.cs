using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApplication1
{
	public class Program
	{
		private static IConfigurationBuilder _conf;
		
		public static async Task Main(string[] args)
		{
			var webHost = CreateHostBuilder(args).Build();
			var buildedConf = _conf.Build();
			await InitDb(webHost, buildedConf);
			
			await webHost.RunAsync();
		}

		public static IWebHostBuilder CreateHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseKestrel(opts => opts.AllowSynchronousIO = true)
				.UseContentRoot(Directory.GetCurrentDirectory())
				.ConfigureLogging((hostContext, logging) =>
					{
						logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
						logging.AddConsole();
						logging.AddDebug();
					}
				)
				.ConfigureAppConfiguration((builderContext, config) =>
				{
					var env = builderContext.HostingEnvironment;

					_conf = config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
						.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
				})
				.UseStartup<Startup>();
		
		private static async Task InitDb(IWebHost host, IConfiguration conf)
		{
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;

				try
				{
					var context = new AppDbContextFactory(conf).CreateDbContext(new string[]{""});
					await AppDbInit.Initialize(context, conf);
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred while seeding the database.");
				}
			}
		}
	}
}