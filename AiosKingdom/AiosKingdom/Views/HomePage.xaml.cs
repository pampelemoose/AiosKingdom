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
	public partial class HomePage : ContentPage
	{
        private ViewModels.HomePageViewModel _model;

		public HomePage()
		{
            Title = "Home";

            _model = new ViewModels.HomePageViewModel();

			InitializeComponent();

            BindingContext = _model;
		}

        private static string Close = "Close";
        private static string Souls = "Souls";
        private static string Exit = "Exit";
        private void ShowSetting_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await App.Current.MainPage.DisplayActionSheet("Settings", Close, null, new string[] { Souls, Exit });

                if (result == Souls)
                {
                    NetworkManager.Instance.DisconnectSoul();

                    _model.IsBusy = true;
                }
                if (result == Exit)
                {
                    NetworkManager.Instance.AnnounceDisconnection();

                    _model.IsBusy = true;

                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            });
        }
    }
}