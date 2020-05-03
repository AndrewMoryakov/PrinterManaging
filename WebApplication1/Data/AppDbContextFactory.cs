using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using WebApplication1.DataModel;

namespace WebApplication1.Data
{
	public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		private IConfiguration _config;
		public AppDbContextFactory(IConfiguration config)
		{
			_config = config;
		}

		public AppDbContextFactory()
		{
		}

		public ApplicationDbContext CreateDbContext(string[] args)
		{
			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
			
			// if (_config!=null && _config["DbProvider"] == "pgsql")
			// 	builder.UseNpgsql(configuration.GetConnectionString("DefaultConnectionPgsql"));
			// else if (_config ==null || _config["DbProvider"] == "mssql")
			// 	builder.UseSqlServer(configuration.GetConnectionString("DefaultConnectionMsSql"));
			// else if (_config == null || _config["DbProvider"] == "mysql")
			// 	builder.UseMySQL(configuration.GetConnectionString("DefaultConnectionMysql"));
			// else 
			if (_config == null || _config["DbProvider"] == "ram")
				builder.UseInMemoryDatabase("dateBaseInMemory");

			return new ApplicationDbContext(builder.Options);
		}
	}
}