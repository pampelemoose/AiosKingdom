using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AiosKingdom
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.Disconnected, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NetworkManager.Instance.Disconnect();
                    MainPage = new Views.LoginPage();
                });
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.GameServerDisconnected, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    //NetworkManager.Instance.DisconnectGame();
                    MainPage = new NavigationPage(new Views.ServerListPage(new List<Network.GameServerInfos>()));
                });
            });

            MessagingCenter.Subscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived, (sender, servers) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new NavigationPage(new Views.ServerListPage(servers));
                });
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulConnected, (sender) =>
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
