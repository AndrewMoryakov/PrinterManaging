using System;
using System.Security.Cryptography;
using System.Text;
using BackendClient.DateModels;
using Newtonsoft.Json;
using RestSharp;

namespace BackendClient
{
	public class Client
	{
		private static string _baseUrl;

		public Client(string baseUrl)
		{
			_baseUrl = baseUrl;
		}

		public UserInfo GetUserInfo(string userName, string password)
		{
			string token = //WindowVkOAuth.Token ??
			               GetToken(userName, password)?.access_token;
			return GetUserInfo(token);
		}

		public UserInfo GetUserInfo(string token)
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