using BackendClient.DateModels;
using DataModels;
using Newtonsoft.Json;
using RestSharp;

namespace BackendClient
{
	public class ClientToBack
	{
		private static string _baseUrl;

		public ClientToBack(string baseUrl)
		{
			_baseUrl = baseUrl;
		}

		public UserInfo GetUserInfo(string userName, string password)
		{
			string token = //WindowVkOAuth.Token ??
			               GetToken(userName, password);
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

		public string GetToken(string username, string password)
		{
			string api = "auth";
			var client = new RestClient($"{_baseUrl}/api/{api}");
			var request = new RestRequest(Method.GET);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("password", password);
			request.AddHeader("username", username);
			IRestResponse response = client.Execute(request);

			return JsonConvert.DeserializeObject<UserAuthInfo>(response?.Content).access_token;
		}
		
		public static IRestResponse SendPrintedDocumentsOnServer(string token, PrintedDocument[] printedPages)
		{
			var client = new RestClient($"{_baseUrl}/api/Account/PrintDocument");
			var request = new RestRequest(Method.POST);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("authorization", $"Bearer {token}");
			request.AddHeader("content-type", "application/json");

			//string parametersForSend = "";

			//foreach (var printedPage in printedPages)
			//{

			//}
			request.AddParameter("application/json", JsonConvert.SerializeObject(printedPages), ParameterType.RequestBody);
			return client.Execute(request);
		}
	}
}