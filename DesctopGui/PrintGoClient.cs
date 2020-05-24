using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DesctopGui.DateModels;
using Newtonsoft.Json;
using RestSharp;

namespace DesctopGui
{
	public class PrintGoClient
	{
		private static string _printControllerHost;
		public PrintGoClient(string printControllerHost)
			=> _printControllerHost = printControllerHost;

		/// <summary>
		/// Обновление баланса
		/// </summary>
		/// <remarks>Это может понадобится, если списание произошло на другом клиенте.</remarks>
		/// <returns></returns>
		public UserInfo RefrasheBalanse()
		{
			var client = new RestClient($"{_printControllerHost}/api/values/RefreshUserInfo");
			var request = new RestRequest(Method.POST);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			IRestResponse response = client.Execute(request);

			return JsonConvert.DeserializeObject<UserInfo>(response.Content);
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

		public void LogOut(string token)
		{
			var client = new RestClient($"{_printControllerHost}/api/Values/LogOut");
			var request = new RestRequest(Method.POST);
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			request.AddParameter("token", token);
			client.Execute(request);
		}
		
		public async Task LogOutAsync(string token)
		{
			var client = new RestClient($"{_printControllerHost}/api/Values/LogOut");
			var request = new RestRequest(Method.POST);
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			request.AddParameter("token", token);
			await client.ExecuteAsync(request);
		}

		public void LogIn(string token)
		{
			var client = new RestClient($"{_printControllerHost}/login");
			var request = new RestRequest(Method.POST);
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			request.AddParameter("token", token);
			client.Execute(request);
		}
	}
}