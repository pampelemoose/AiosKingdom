using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AiosKingdom
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.Disconnected, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new Views.LoginPage();
                });
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.InitialDatasReceived, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new Views.ServerListPage();
                });
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.ConnectedToServer, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new MainPage();
                });
            });

            MainPage = new Views.LoginPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
