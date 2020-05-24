using BackendClient;
using ConsoleApp1_2;
using DreamPlace.Lib.Rx;
using Mapster;

namespace Project53.New_arhtech
{
	public class Auth
	{
		public static Client GetClient()
		{
			var client = Registry.GetValue<ClientToBack>();
			var token = Registry.GetValue<string>(RegistryAddresses.Login);
			var appClient = client.GetUserInfo(token).Adapt<Client>();
			appClient.Token = token;
			return appClient;
		}
	}
}