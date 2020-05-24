using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BackendClient;
using DesctopGui.DateModels;
using DreamPlace.Lib.Rx;

namespace DesctopGui
{
	public partial class Welcome : Page
	{
		private PrintGoClient _clientToPrint;
		private ClientToBack _clientToBackOfServers;
		public Welcome()
		{
			InitializeComponent();

			var user = Registry<UserInfo, UserInfo>.Get();
			textBlockBalance.Text = $"{user.Balance.ToString()} ₽";
			_clientToPrint = Registry.GetValue<PrintGoClient>();
			_clientToBackOfServers = Registry.GetValue<ClientToBack>();
		}

		private async void ButtonLogOut_OnClick(object sender, RoutedEventArgs e)
		{
			//ClearIECookies.Clear("https://oauth.vk.com");
			await _clientToPrint.LogOutAsync(Registry.GetValue<string>("token"));
			//Environment.Exit(0);
		}

		private void ButtonBalanseRefresh_OnClick(object sender, RoutedEventArgs e)
		{
			//ToDo
			//надо оповещать P53 о том,
			//что сделали рефреш баланса, чтобы P53 тоже обновил себя
			
			textBlockBalance.Text =
				_clientToBackOfServers
					.GetUserInfo(Registry.GetValue<string>("token")).Balance
					.ToString(CultureInfo.InvariantCulture);
		}
	}
}