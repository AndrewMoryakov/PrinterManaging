using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DreamPlace.Lib.Rx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;

namespace DesctopGui
{
	public partial class MainWindow : Window
	{
		private ClientOfServers _clientOfServers;

		public MainWindow()
		{
			InitializeComponent();
			var builder = new ConfigurationBuilder()
				// .SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			IConfigurationRoot configuration = builder.Build();
			
			Registry<WebBrowser, WebBrowser>.Public(new WebBrowser());
			this.Content = new Auth();
			Registry<Frame, MainWindow>.Public(this);
			
			_clientOfServers = new ClientOfServers(
				configuration.GetSection("appSettings:serviceDomain").Value,
				configuration.GetSection("appSettings:printControllerHost").Value
			);
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			// ClearIECookies.Clear("https://oauth.vk.com");
			// _clientOfServers.LogOut();
		}

		private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
		{
		}
	}
}