using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BackendClient;
using DreamPlace.Lib.Rx;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace DesctopGui
{
	public partial class MainWindow : Window
	{
		private ClientToBack _clientToBackOfServers;

		public MainWindow()
		{
			InitializeComponent();
			var builder = new ConfigurationBuilder()
				// .SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			IConfigurationRoot configuration = builder.Build();

			_clientToBackOfServers = new ClientToBack(
				configuration.GetSection("appSettings:serviceDomain").Value
				// configuration.GetSection("appSettings:printControllerHost").Value
			);
			
			var printClient = new PrintGoClient(
				configuration.GetSection("appSettings:printControllerHost").Value
			);
			
			var logger = new LoggerConfiguration()
				.WriteTo.Console(theme: SystemConsoleTheme.Colored)
				.WriteTo.File("logs.log")
				.MinimumLevel.Verbose()
				.CreateLogger();
			
			Registry.Public(_clientToBackOfServers);
			Registry.Public(logger);
			Registry.Public(printClient);
			Registry<WebBrowser, WebBrowser>.Public(new WebBrowser());
			Registry<Frame, MainWindow>.Public(this);
			
			this.Content = new Auth();
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