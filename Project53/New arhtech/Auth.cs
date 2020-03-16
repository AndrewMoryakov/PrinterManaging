namespace Project53.New_arhtech
{
	public class Auth
	{
		public static Client GetClient()
		{
			decimal mockBalance = 10.0m;
			return new Client(mockBalance);
		}
	}
}