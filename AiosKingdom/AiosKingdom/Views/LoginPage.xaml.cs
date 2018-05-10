using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AiosKingdom.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.LoginFailed, (sender, arg) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Login Failed", arg, "OK");
                });
            });
        }
	}
}