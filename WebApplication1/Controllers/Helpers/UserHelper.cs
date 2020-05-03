namespace WebApplication1.Controllers.Helpers
{
	public class UserHelper
	{
		// public static (IdentityResult Result, ApplicationUser User) CreateUser(RegisterBindingModel model, AppRole role, bool isVk = false)
		// {
		// 	try
		// 	{
		// 		using (var db = new ApplicationDbContext())
		// 		{
		// 			var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
		// 			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
		// 			var role1 = new IdentityRole
		// 			{
		// 				Name = role.ToString()
		// 			};
		// 			//создаем роль
		// 			roleManager.Create(role1);
		// 			//Создаем пользователя
		// 			var user = new ApplicationUser
		// 			{
		// 				UserName = model.Email,
		// 				Email = model.Email,
		// 				FirstName = model.FirstName,
		// 				LastName =  model.LastName,
		// 				EmailConfirmed = isVk,
		// 				ImageUrl = model.ImageUrl,
		// 				SocialId = model.SocialId,
		// 				SocialEmail = model.SocialEmail
		// 			};
		//
		// 			IdentityResult result = null;
		// 			if (model.Password==null)
		// 			 result = userManager.Create(user);
		// 			else
		// 				result = userManager.Create(user, model.Password);
		//
		// 			if (result.Succeeded)
		// 			{
		// 				userManager.AddToRole(user.Id, role1.Name);
		// 			}
		// 			return (result, user);
		// 		}
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		throw new Exception("Не удалось создать пользователя", ex);
		// 	}
		// }
		//
		// /// <summary>
		// ///     Создает нового пользователя
		// /// </summary>
		// /// <param name="model">Объект с информацией о пользователе</param>
		// /// <returns>Результат операции</returns>
		// public static IdentityResult GetRoleOfUser(RegisterBindingModel model, AppRole role)
		// {
		// 	try
		// 	{
		// 		using (var db = new ApplicationDbContext())
		// 		{
		// 			var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
		// 			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
		// 			var role1 = new IdentityRole
		// 			{
		// 				Name = role.ToString()
		// 			};
		// 			//создаем роль
		// 			roleManager.Create(role1);
		// 			//Создаем пользователя
		// 			var admin = new ApplicationUser
		// 			{
		// 				UserName = model.Email,
		// 				Email = model.Email,
		// 				FirstName = model.FirstName,
		// 				LastName = model.LastName
		// 			};
		//
		// 			var result = userManager.Create(admin, model.Password);
		// 			if (result.Succeeded)
		// 			{
		// 				userManager.AddToRole(admin.Id, role1.Name);
		// 			}
		// 			return result;
		// 		}
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		throw new Exception("Не удалось создать пользователя", ex);
		// 	}
		// }
		//
		// /// <summary>
		// ///     Изменяет пользователя
		// /// </summary>
		// /// <param name="model">
		// ///     Пользователь с одним или несколькими новыми свойствами, свойство Id должно быть таким же как и у
		// ///     изменяемой сущности
		// /// </param>
		// public static (IdentityResult Result, ApplicationUser User) EditUser(ApplicationUser model)
		// {
		// 	using (var db = new ApplicationDbContext())
		// 	{
		// 		var user = db.Users.FirstOrDefault(el => el.Id == model.Id);
		//
		// 		user.FirstName = model.FirstName;
		// 		user.LastName = model.LastName;
		// 		user.CurrentBalance = model.CurrentBalance;
		// 		user.Email = model.Email;
		// 		user.UserName = model.Email;
		// 		user.EmailConfirmed = model.EmailConfirmed;
		//
		// 		db.Entry(user).State = EntityState.Modified;
		// 		db.SaveChanges();
		//
		// 		return (new IdentityResult(), user);
		// 	}
		// }
		//
		// /// <summary>
		// ///     Изменяет пользователя
		// /// </summary>
		// /// <param name="model">
		// ///     Пользователь с одним или несколькими новыми свойствами, свойство Id должно быть таким же как и у
		// ///     изменяемой сущности
		// /// </param>
		// public static (IdentityResult Result, ApplicationUser User) EditUser(ChangeBindingModel model)
		// {
		// 	using (var db = new ApplicationDbContext())
		// 	{
		// 		var user = db.Users.FirstOrDefault(el => el.Id == model.Id);
		//
		// 		user.FirstName = model.FirstName;
		// 		user.LastName = model.LastName;
		// 		user.CurrentBalance = model.FoundAccount.Amount.GetInvariantDecimal(decimal.Parse(model.FoundAccount.Amount,
		// 			NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture));//decimal.Parse(model.FoundAccount.Amount, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
		// 		user.Email = model.Email;
		// 		user.UserName = model.Email;
		// 		user.EmailConfirmed = model.PasswordConfirmed;
		//
		// 		db.Entry(user).State = EntityState.Modified;
		// 		db.SaveChanges();
		//
		// 		return (new IdentityResult(), user);
		// 	}
		// }
		//
		// /// <summary>
		// ///     ПОдтверждает пароль
		// /// </summary>
		// /// <param name="model">
		// ///     Пользователь с одним или несколькими новыми свойствами, свойство Id должно быть таким же как и у
		// ///     изменяемой сущности
		// /// </param>
		// public static ApplicationUser ConfirmEmail(bool confirm, string id)
		// {
		// 	using (var db = new ApplicationDbContext())
		// 	{
		// 		var user = db.Users.FirstOrDefault(el => el.Id == id);
		// 		user.EmailConfirmed = confirm;
		//
		// 		db.Entry(user).State = EntityState.Modified;
		// 		db.SaveChanges();
		//
		// 		return user;
		// 	}
		// }
		//
		// public static (IdentityResult Result, ApplicationUser User) GetUserById(string id)
		// {
		// 	using (var db = new ApplicationDbContext())
		// 	{
		// 		var user = db.Users.FirstOrDefault(el => el.Id == id);
		// 		return (new IdentityResult(), user);
		// 	}
		// }
		//
		// public static (IdentityResult Result, ApplicationUser User) GetUserBySocialId(string socialId)
		// {
		// 	using (var db = new ApplicationDbContext())
		// 	{
		// 		var user = db.Users.FirstOrDefault(el => el.SocialId == socialId);
		// 		return (new IdentityResult(), user);
		// 	}
		// }
		//
		// public static (IdentityResult Result, ApplicationUser User) GetUserBySocialEmail(string socialEmail)
		// {
		// 	using (var db = new ApplicationDbContext())
		// 	{
		// 		var user = db.Users.FirstOrDefault(el => el.SocialEmail == socialEmail);
		// 		return (new IdentityResult(), user);
		// 	}
		// }
		//
		//
		// public static (IdentityResult Result, ApplicationUser User) GetUserByEmail(string email)
		// {
		// 	using (var db = new ApplicationDbContext())
		// 	{
		// 		var user = db.Users.FirstOrDefault(el => el.Email == email);
		// 		return (new IdentityResult(), user);
		// 	}
		// }
		//
		// /// <summary>
		// ///     Изменяет счет
		// /// </summary>
		// /// <param name="model">
		// ///     Пользователь с одним или несколькими новыми свойствами, свойство Id должно быть таким же как и у
		// ///     изменяемой сущности
		// /// </param>
		// public static IdentityResult EditFundAccountUser(FundAccountBindingModel model)
		// {
		// 	using (var db = new ApplicationDbContext())
		// 	{
		// 		var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
		// 		bool isNotAdmin = userManager.GetRoles(model?.IdOfCurrentUser).FirstOrDefault() != AppRole.Administrator.ToString();
		// 		bool isNotYourSelf = (userManager.GetEmail(model?.IdOfCurrentUser) != model.Email);
		// 		if (model.Amount[0] == '-')
		// 		{
		// 			if (isNotAdmin && isNotYourSelf)
		// 				return new IdentityResult("Вы не можете изменить баланс пользователя.");
		//
		// 			model.Amount = $"({model.Amount.Remove(0, 1)})";
		// 		}
		// 		else
		// 		{
		// 			if (isNotAdmin)
		// 				return new IdentityResult("Вы не можете изменить баланс пользователя.");
		//
		// 		}
		//
		// 		return ChangeInvoice(model, db);
		// 	}
		// }
		//
		// internal static IdentityResult ChangeInvoice(FundAccountBindingModel model, ApplicationDbContext db)
		// {
		// 	var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
		// 	var user = db.Users.FirstOrDefault(el => el.Email == model.Email);
		//
		// 	decimal amountForSum = model.Amount.GetInvariantDecimal(0);//decimal.Parse(model.Amount, NumberStyles.AllowDecimalPoint | NumberStyles.AllowParentheses | NumberStyles.AllowThousands, CultureInfo.CurrentCulture));
		// 	decimal resultAmount = user.CurrentBalance += amountForSum;
		//
		// 	if (resultAmount < 0)
		// 	{
		// 		return new IdentityResult("Баланс не может быть отрицательным");
		// 	}
		//
		// 	db.InvoiceMovements.Add(new InvoiceMovement(amountForSum, DateTime.Now, model.IdOfCurrentUser,
		// 		user?.Id));
		// 	db.SaveChanges();
		// 	user.CurrentBalance = resultAmount;
		//
		// 	return userManager.Update(user);
		// }
		//
		// /// <summary>
		// /// Удаляет пользователя
		// /// </summary>
		// /// <param name="id">Идетфикатор пользователя</param>
		// /// <returns>Возвращает удаленного пользователя</returns>
		// public static ApplicationUser DeleteUser(string id)
		// {
		// 	using (ApplicationDbContext db = new ApplicationDbContext())
		// 	{
		// 		ApplicationUser user = db.Users.Include(el => el.InvoiceMovements).Include(el=>el.PrintedPages).FirstOrDefault(el => el.Id == id);
		// 		//var r = user.InvoiceMovements;
		// 		//foreach (var inv in r)
		// 		//{
		// 		//	db.InvoiceMovements.Remove(inv);
		// 		//}
		// 		db.SaveChanges();
		// 		db.Users.Remove(user);
		// 		db.SaveChanges();
		//
		// 		return user;
		// 	}
		// }
		//
		// #region Async
		//
		// /// <summary>
		// /// Удаляет
		// /// </summary>
		// /// <param name="id">Идетфикатор пользователя</param>
		// /// <returns>Возвращает удаленного пользователя</returns>
		// public static async Task<ApplicationUser> DeleteUserAsync(string id)
		// {
		// 	return await Task.FromResult(DeleteUser(id));
		// }
		//
		// /// <summary>
		// ///     Создает нового пользователя
		// /// </summary>
		// /// <param name="model">Объект с информацией о пользователе</param>
		// /// <returns>Результат операции</returns>
		// public static async Task<(IdentityResult Result, ApplicationUser User)> CreateUserAsync(RegisterBindingModel model, AppRole role, bool isVk = false)
		// {
		// 	return await Task.FromResult(CreateUser(model, role, isVk));
		// }
		//
		// /// <summary>
		// ///     Асинхронно зменяет счет
		// /// </summary>
		// /// <param name="model">
		// ///     Пользователь с одним или несколькими новыми свойствами, свойство Id должно быть таким же как и у
		// ///     изменяемой сущности
		// /// </param>
		// public static async Task<IdentityResult> EditFundAccountUserAsync(FundAccountBindingModel model)
		// {
		// 	return await Task.FromResult(EditFundAccountUser(model));
		// }
		//
		//
		// public static async Task<(IdentityResult Result, ApplicationUser User)> EditUserAsync(ApplicationUser email)
		// {
		// 	return await Task.FromResult(EditUser(email));
		// }
		//
		// public static async Task<(IdentityResult Result, ApplicationUser User)> EditUserAsync(ChangeBindingModel email)
		// {
		// 	return await Task.FromResult(EditUser(email));
		// }
		//
		// public static async Task<(IdentityResult Result, ApplicationUser User)> GetUserBySocialIdAsync(string socialId)
		// {
		// 	return await Task.FromResult(GetUserBySocialId(socialId));
		// }
		//
		// public static async Task<(IdentityResult Result, ApplicationUser User)> GetUserBySocialEmailAsync(string socialEmail)
		// {
		// 	return await Task.FromResult(GetUserBySocialEmail(socialEmail));
		// }
		//
		// #endregion
	}
}