using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication1
{
	public class AuthOptions
	{
		public const string ISSUER = "DreamPlace"; // издатель токена
		public const string AUDIENCE = "http://localhost:58101/"; // потребитель токена
		const string KEY = "mysupersecret_Secretkey!123"; // ключ для шифрации
		public const int LIFETIME = 1000000; // время жизни токена - 1 минута

		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
		}
	}
}