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
	public partial class SoulListPage : ContentPage
	{
		public SoulListPage(ViewModels.SoulListPageViewModel viewModel)
		{
			InitializeComponent();

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SoulCreationFailed, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Soul Creation Failed", message, "OK");
                });
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SoulConnectionFailed, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Soul Connection Failed", message, "OK");
                });
            });

            BindingContext = viewModel;
		}
	}
}