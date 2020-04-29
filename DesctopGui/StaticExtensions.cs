namespace DesctopGui
{
	public static class StaticExtensions
	{
		public static bool IsNullOrEmptyOrSpace(this string str)
		{
			return string.IsNullOrWhiteSpace(str) && string.IsNullOrEmpty(str);
		}
	}
}