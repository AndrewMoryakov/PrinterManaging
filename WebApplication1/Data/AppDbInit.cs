using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using WebApplication1.DataModel;

namespace WebApplication1.Data
{
	public class AppDbInit
	{
		public static async Task Initialize(ApplicationDbContext context, IConfiguration conf)
		{
			if (conf["SeedForce"].ToLower() == "true"
			    || !(context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator)
				    .Exists())
			{
				using (context)
					await CreateFirstUser(context);
			}
		}
		
		private static async Task CreateFirstUser(ApplicationDbContext context)
		{
			var admin = new ApplicationUser
			{
				NormalizedEmail = "admin@admin.com",
				NormalizedUserName = "admin",
				Email = "Admin@admin.com",
				FirstName = "Admin",
				LastName = "Admin",
				UserName = "Admin",
				EmailConfirmed = true,
				LockoutEnabled = false,
				SecurityStamp = Guid.NewGuid().ToString()
			};
			
			var client = new ApplicationUser
			{
				NormalizedEmail = "andrew@gmail.com",
				NormalizedUserName = "andrew",
				Email = "Andrew@gmail.com",
				FirstName = "Andrew",
				LastName = "Andrew",
				UserName = "Andrew",
				EmailConfirmed = true,
				LockoutEnabled = false,
				SecurityStamp = Guid.NewGuid().ToString()
			};
			

			var roleStore = new RoleStore<IdentityRole>(context);

			var adminRole = Role.Admin.ToString();
			var clientRole = Role.Client.ToString();
			if (!context.Roles.Any(r => r.Name == adminRole))
			{
				await roleStore.CreateAsync(new IdentityRole { Name = adminRole, NormalizedName = adminRole });
			}

			if (!context.Roles.Any(r => r.Name == clientRole))
			{
				await roleStore
					.CreateAsync(new IdentityRole 
						{
							Name = clientRole, 
							NormalizedName = clientRole 
						});
			}

			if (!context.Users.Any(u => u.UserName == admin.UserName))
			{
				var password = new PasswordHasher<ApplicationUser>();
				var hashed = password.HashPassword(admin, "password");
				admin.PasswordHash = hashed;
				var userStore = new UserStore<ApplicationUser>(context);
				await userStore.CreateAsync(admin);
				await userStore.AddToRoleAsync(admin, adminRole);
			}

			if (!context.Users.Any(u => u.UserName == client.UserName))
			{
				var password = new PasswordHasher<ApplicationUser>();
				var hashed = password.HashPassword(client, "password");
				admin.PasswordHash = hashed;
				var userStore = new UserStore<ApplicationUser>(context);
				await userStore.CreateAsync(admin);
				await userStore.AddToRoleAsync(admin, clientRole);
			}
			
			await context.SaveChangesAsync();
		}
	}
}