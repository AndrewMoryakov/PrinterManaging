using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.DataModel;

namespace WebApplication1
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//allow for mssql | mysql | pgsql | ram
			switch (Configuration["DbProvider"])
			{
				// case "pgsql":
				// 	services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(
				// 		opt => opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnectionPgsql"))
				// 	);
				// 	break;
				// case "mssql":
				// 	services.AddEntityFrameworkSqlServer().AddDbContext<ApplicationDbContext>(
				// 		opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionMsSql")));
				// 	break;
				// case "mysql":
				// 	services.AddEntityFrameworkMySQL().AddDbContext<ApplicationDbContext>(
				// 		opt => opt.UseMySQL(Configuration.GetConnectionString("DefaultConnectionMysql")));
				// 	break;
				case "ram":
					services.AddEntityFrameworkInMemoryDatabase().AddDbContext<ApplicationDbContext>(
						opt => opt.UseInMemoryDatabase("dateBaseInMemory"));
					break;
			}
			
			services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
				.AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}