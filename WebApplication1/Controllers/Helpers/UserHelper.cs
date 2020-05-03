using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WebApplication1.DataModel;

namespace WebApplication1.Controllers.Helpers
{
	public class UserHelper
	{
		public class UserService : IUserService, IDisposable
		{
			private UserManager<ApplicationUser> _userManager;

			private ApplicationDbContext _context;
			//private Bot _bot;

			private SignInManager<ApplicationUser> _signInManager2;
			private IPasswordHasher<ApplicationUser> _passwordHasher;
			private IHttpContextAccessor _httpContext;

			public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
				SignInManager<ApplicationUser> signInManager2, IPasswordHasher<ApplicationUser> hasher,
				IHttpContextAccessor httpContext
			) //,Bot bot)
			{
				//_bot = bot;
				_userManager = userManager;
				_signInManager2 = signInManager2;
				_passwordHasher = hasher;
				_httpContext = httpContext;
				_context = context;
			}

			public UserManager<ApplicationUser> UserManager
			{
				get { return _userManager; }
				private set { _userManager = value; }
			}

			public string GetEmailCurrentUser()
			{
				return _httpContext.HttpContext.User.Claims
					.FirstOrDefault(el => el.Type == ClaimTypes.Email)?.Value;
			}

			public string GetCurrentUserId()
			{
				return _httpContext.HttpContext.User.Claims
					.FirstOrDefault(el => el.Type == ClaimTypes.NameIdentifier)?.Value;
			}

			public SignInManager<ApplicationUser> InManager2
			{
				get { return _signInManager2; }
				set { _signInManager2 = value; }
			}

			public async Task<ApplicationUser> GetUserByUsernameOrEmailAsync(string username)
			{
				var user = await _userManager.FindByEmailAsync(username)
				           ?? await _userManager.FindByNameAsync(username);

				return user;
			}

			//public async Task<ApplicationUser> GetUserByPublicId(string username)
			//{
			//	var user = await _context.Users.FirstOrDefaultAsync(el => el.PublicId == username);
			//	return user;
			//}

			public async Task<ApplicationUser> GetUserById(string id)
			{
				var user = await _userManager.FindByIdAsync(id);

				return user;
			}

			public async Task<IdentityResult> CreateUser(ApplicationUser user, string password)
			{
				return await _userManager.CreateAsync(user, password);
			}

			public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string password)
			{
				return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
			}

			public ApplicationUser GetUser()
			{
				string email = GetEmailCurrentUser();
				if (email == null)
					return null;

				var users = _context.Users; //.Include(el => el.Stores).Include(el => el.UsersStore);
				return users.FirstOrDefault(el => el.Email == email);
			}

			public ApplicationUser GetUser(string id)
			{
				if (id == null)
					return null;

				var users = _context
					.Users; ////.Include(el => el.Stores).Include(el => el.UsersStore).Include(el => el.UsersStore);
				return users.FirstOrDefault(el => el.Id == id);
			}

			public async Task<IList<string>> GetRolesAsync()
			{
				return await _userManager.GetRolesAsync(GetUser());
			}

			public async Task<IList<string>> GetRolesAsync(string userId)
			{
				var user = _context.Users.FirstOrDefault(el => el.Id == userId);
				return await _userManager.GetRolesAsync(user);
			}

			//public IList<string> GetRoles(string userId)
			//{
			//	if (userId == null)
			//		throw new ArgumentException("Не указано имя пользователя", nameof(userId));

			//	var users = _context.Users
			//		.Include(el => el.Roles);
			//	var user = users.FirstOrDefault(el => el.Id == userId);
			//		IEnumerable<string> rolesIdForUser = user?.Roles.Select(el => el.RoleId);

			//	var allRoles = _context.Roles;

			//	IList<string> rolesForUser =
			//		(from roleId in rolesIdForUser
			//		 from role in allRoles
			//		 where roleId == role.Id
			//		 select role.Name).ToList();

			//	return rolesForUser;
			//}

			//public async Task AddToTelegram(Telegram.Bot.Types.Message message)
			//{

			//	try
			//	{

			//		(await _bot.GetClientAsync())
			//			?.SendTextMessageAsync(message.Chat.Id, "I see you", replyToMessageId: message.MessageId);

			//		if (message.Text == null || message.Text.Length < 10)
			//		{
			//			return; //ToDo Тут надо логи писать
			//		}

			//		var chatId = message.Chat.Id;
			//		var messageId = message.MessageId;




			//		var commandParameters = message.Text.Split(' ');
			//		if (commandParameters.Length != 2)
			//		{
			//			await(await _bot.GetClientAsync()).SendTextMessageAsync(chatId, "Your command isn't right");

			//			return;
			//		}

			//		var user = commandParameters[1];
			//		var currUser = _context.Users.FirstOrDefault(el => el.Email.ToUpper() == user);
			//		(await _bot.GetClientAsync())
			//			?.SendTextMessageAsync(message.Chat.Id, $"Your id: {currUser.Id}", replyToMessageId: message.MessageId);



			//		//var currUser = _context.Users.FirstOrDefault(el => el.Email.ToLower() == user);//it) GetUserByUsernameOrEmailAsync(user));
			//		if (currUser == null)
			//			await(await _bot.GetClientAsync()).SendTextMessageAsync(chatId, "I can't find user");


			//		currUser.ChatId = chatId.ToString();

			//		if (currUser.ChatId == null && currUser.ChatId == chatId.ToString())
			//		{
			//			await(await _bot.GetClientAsync()).SendTextMessageAsync(chatId, "I already know u", replyToMessageId: messageId);
			//		}

			//		_context.Users.Update(currUser);
			//		await _context.SaveChangesAsync();

			//		await(await _bot.GetClientAsync()).SendTextMessageAsync(chatId, "Ok, I added u", replyToMessageId: messageId);
			//	}
			//	catch (Exception ex)
			//	{
			//		await(await _bot.GetClientAsync()).SendTextMessageAsync(message.Chat.Id, ex.Message, replyToMessageId: message.MessageId);
			//		await(await _bot.GetClientAsync()).SendTextMessageAsync(message.Chat.Id, ex.StackTrace, replyToMessageId: message.MessageId);

			//	}
			//}

			public async Task<IList<Claim>> GetClaims(ApplicationUser user)
			{
				return await _userManager.GetClaimsAsync(user);
			}

			private bool disposed = false;

			public void Dispose()
			{
				Dispose(true);
				// подавляем финализацию
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposed)
				{
					if (disposing)
					{
						// Освобождаем управляемые ресурсы
					}

					_userManager?.Dispose();
					_context?.Dispose();

					disposed = true;
				}
			}

			~UserService()
			{
				Dispose(false);
			}
		}
	}
}