using System.Windows;
using System.Windows.Controls;
using DesctopGui.DateModels;
using DreamPlace.Lib.Rx;

namespace DesctopGui
{
	public partial class Welcome : Page
	{
		public Welcome()
		{
			InitializeComponent();

			var user = Registry<UserInfo, UserInfo>.Get();
			textBlockBalance.Text = $"{user.Balance.ToString()} ₽";
		}

		private void ButtonLogOut_OnClick(object sender, RoutedEventArgs e)
		{
			throw new System.NotImplementedException();
		}

		private void ButtonBalanseRefresh_OnClick(object sender, RoutedEventArgs e)
		{
			throw new System.NotImplementedException();
		}
	}
}