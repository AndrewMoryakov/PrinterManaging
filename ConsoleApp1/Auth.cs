using ConsoleApp1;
using DreamPlace.Lib.Rx;

namespace Project53.New_arhtech
{
	public class Auth
	{
		public static Client GetClient()
		{
			return Registry.GetValue<Client>(RegistryAddresses.Login);
		}
	}
}