﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BackendClient;
using DesctopGui.DateModels;
using DreamPlace.Lib.Rx;
using Serilog.Core;
using MapsterMapper;
using Mapster;

namespace DesctopGui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Auth : Page
	{
		private Logger _logger;
		private string userName = "";
		private string password ;
		private UserAuthInfo _token = null;
		private bool _isAuth = false;
		private ClientToBack _clientToBackOfServers;
		private PrintGoClient _clientToPrint;
		public Auth()
		{
			InitializeComponent();
			_logger = Registry.GetValue<Logger>();
			_clientToBackOfServers = Registry.GetValue<ClientToBack>();
			_clientToPrint = Registry.GetValue<PrintGoClient>();
		}

		private void ButtonAuth_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				string password = textBoxPassword.Password;
				string userName = textBoxUserName.Text;

				Login(userName, password);
			}
			catch (Exception ex)
			{
				textBlockInfo.Text =
					$"Ой, что-то пошло не так, мы не можем дать Вам доступ к системе, возможно отсутствует подключение к интернету. Обратитесь к Администратору";
				// _logger.WriteException(ex);
			}
		}
		
		private void Login(string userName, string password)
		{
			if ((password.IsNullOrEmptyOrSpace() || userName.IsNullOrEmptyOrSpace())) 
				// && string.IsNullOrEmpty(WindowVkOAuth.Token))
			{
				textBlockInfo.Text = "Введите электронную почту и пароль";
				return;
			}

			var token = _clientToBackOfServers.GetToken(userName, password);
			Registry.Public(token, "token");
			UserInfo userInfo = _clientToBackOfServers.GetUserInfo(token).Adapt<UserInfo>();
			
			if (userInfo == null)
			{
				_logger.Error("Пользователь не получен", new ArgumentException("Не удалось полчить пользователя"));
				return;
			}
			_logger.Information(userInfo.Balance.ToString());
			Registry<UserInfo, UserInfo>.Public(userInfo);
			
			if (userInfo.Email != null)
			{
				if (userInfo.Balance > 0)
				{
					_logger.Information("Try login in print controller");
					_clientToPrint.LogIn(token);
					_logger.Information("Logined in print controller");
					_isAuth = true;
					if (Registry<Frame, MainWindow>.Get() != null)
						Registry<Frame, MainWindow>.Get().Content = new Welcome();
				}
				else
				{
					textBlockInfo.Text = $"Вы не можете войти, ваш баланс равен {userInfo.Balance}";
				}
			}
			else
			{
				textBlockInfo.Text = "Введен неверный пароль или электронная почта";
				_logger.Information($"False auth dates  {userInfo.Email}");
				_isAuth = false;
			}
		}
		
		private async void ButtonVk_OnClick(object sender, RoutedEventArgs e)
		{
			var priew = textBlockInfo.Text;

			var reslt = Task.Factory.StartNew(() =>
				Dispatcher.Invoke(() => { textBlockInfo.Text = "Авторизация через ВК не реализована."; })
				)
				.ContinueWith(el=>Task.Delay(3000).Wait())
				.ContinueWith(
				(el) =>
					Dispatcher.Invoke(() => { textBlockInfo.Text = priew; })
				);
		}

		private void ButtonSignUp_OnClick(object sender, RoutedEventArgs e)
		{
			
		}
	}
}