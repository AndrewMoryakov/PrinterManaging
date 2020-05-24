using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApplication1.Controllers.Helpers;
using WebApplication1.DataModel;
using Swashbuckle.AspNetCore.Swagger;

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
			
			
			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})

				.AddJwtBearer(options =>
				{
					options.SaveToken = true;
					options.RequireHttpsMetadata = false;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						// укзывает, будет ли валидироваться издатель при валидации токена
						ValidateIssuer = true,
						// строка, представляющая издателя
						ValidIssuer = AuthOptions.ISSUER,

						// будет ли валидироваться потребитель токена
						ValidateAudience = true,
						// установка потребителя токена
						ValidAudience = AuthOptions.AUDIENCE,
						// будет ли валидироваться время существования
						ValidateLifetime = true,

						// установка ключа безопасности
						IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
						// валидация ключа безопасности
						ValidateIssuerSigningKey = true,
					};
				});

			services.AddTransient<IUserService, UserHelper.UserService>();
			services.AddMemoryCache();
				services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
			services.AddControllers();
			
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo {Title = "P53 Backend api", Version = "v1"});
				// var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				// var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				// c.IncludeXmlComments(xmlPath);
				// c.EnableAnnotations();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
		{
			app.Use(async (context, next) =>
			{
				logger.LogInformation($"headers:: \r\n {context.Request.Headers.Select(el=>$"{el.Key}: {el.Value}").Join("\n")}");
				await next.Invoke();
			});
			
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			
			app.UseCors(builder =>
				builder.AllowAnyOrigin()
					.AllowAnyHeader()
					.AllowAnyMethod()
			);

			// app.UseHttpsRedirection();

			app.UseSwagger();
			app.UseSwaggerUI(c 
				=> { c.SwaggerEndpoint("/swagger/v1/swagger.json", "P53 Backend api"); });
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}