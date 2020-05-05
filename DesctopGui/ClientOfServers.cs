using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using DesctopGui.DateModels;
using Newtonsoft.Json;
using RestSharp;

namespace DesctopGui
{
	public class ClientOfServers
	{
		private static string _printControllerHost;
		private static string _baseUrl;

		public ClientOfServers(string baseUrl, string printControllerHost)
		{
			_baseUrl = baseUrl;
			_printControllerHost = printControllerHost;
		}

		public UserInfo RefrasheBalanse()
		{
			var client = new RestClient($"{_printControllerHost}/api/values/RefreshUserInfo");
			var request = new RestRequest(Method.POST);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			IRestResponse response = client.Execute(request);

			return JsonConvert.DeserializeObject<UserInfo>(response.Content);
		}

		public static bool Pnig()
		{
			var client =
				new RestClient(
					$"http://backzilla20170323015527.azurewebsites.net/api/Values/Get/?id=kasdsd@d.com&key=4810159491035854975053985510296");
			var request = new RestRequest(Method.GET);
			//request.AddHeader("cache-control", "no-cache");
			IRestResponse response = client.Execute(request);

			return response?.Content?.Replace($"{'"'}", "") == GetMd5();
		}

		static string GetMd5()
		{
			byte[] hash = Encoding.ASCII.GetBytes(
				$"{DateTime.UtcNow.Year ^ 7}{DateTime.UtcNow.Month ^ 7}{DateTime.UtcNow.Day ^ 7}{DateTime.UtcNow.Hour ^ 7}{DateTime.UtcNow.Minute ^ 7}");
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] hashenc = md5.ComputeHash(hash);
			string result = "";
			foreach (var b in hashenc)
			{
				result += b.ToString("x2");
			}

			return result;
		}

		public void LogOut()
		{
			var client = new RestClient($"{_printControllerHost}/api/Values/LogOut");
			var request = new RestRequest(Method.POST);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			client.Timeout = 10000;
			client.Execute(request);
		}

		public void LogIn(string userName, string password)
		{
			string token = // WindowVkOAuth.Token ??
			               GetToken(userName, password)?.access_token;
			LogIn(token);

		}

		public void LogIn(string token)
		{
			var client = new RestClient($"{_printControllerHost}/api/values/LogIn?token={token}");
			var request = new RestRequest(Method.POST);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			client.Execute(request);
		}

		public UserInfo GetUserNameInfo(string userName, string password)
		{
			string token = //WindowVkOAuth.Token ??
			               GetToken(userName, password)?.access_token;
			return GetUserNameInfo(token);
		}

		public UserInfo GetUserNameInfo(string token)
		{
			string url = $"{_baseUrl}/api/user";
			var client =
				new RestClient(url);
			
			var request = new RestRequest(Method.GET);
			// request.RequestFormat = RestSharp.DataFormat.Json;
			// request.AddHeader("Content-Type", "application/json");
			request.AddParameter("Authorization", $"Bearer {token}", ParameterType.HttpHeader);
			
			IRestResponse response = client.Execute(request);

			return JsonConvert.DeserializeObject<UserInfo>(response.Content);
		}


		private UserAuthInfo GetToken(string username, string password)
		{
			string api = "auth";
			var client = new RestClient($"{_baseUrl}/api/{api}");
			var request = new RestRequest(Method.GET);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("password", password);
			request.AddHeader("username", username);
			IRestResponse response = client.Execute(request);

			return JsonConvert.DeserializeObject<UserAuthInfo>(response?.Content);
		}
	}
}