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
                    MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.GameServerDisconnected);

                    NetworkManager.Instance.Disconnect();
                    NetworkManager.Instance.DisconnectSoul();
                    NetworkManager.Instance.DisconnectGame();

                    ScreenManager.Instance.ChangePage(new Views.LoginPage());
                });
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulConnected, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ScreenManager.Instance.ChangePage(new Views.HomePage());
                    Subscribe_GameServerDisconnected();
                });
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.ExitedDungeon, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ScreenManager.Instance.ChangePage(new Views.HomePage());
                    Subscribe_GameServerDisconnected();
                });
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.EnterDungeon, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ScreenManager.Instance.ChangePage(new Views.Dungeon.DungeonPage());
                });
            });

            MainPage = new Views.LoginPage();

            ScreenCallbacks();
        }

        private void Subscribe_GameServerDisconnected()
        {
            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.GameServerDisconnected, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NetworkManager.Instance.DisconnectGame();
                    ScreenManager.Instance.ChangePage(new Views.ServerListPage(new List<Network.GameServerInfos>()));
                    MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.GameServerDisconnected);
                });
            });
        }

        #region LOADING SCREEN

        private void ScreenCallbacks()
        {
            MessagingCenter.Subscribe<ScreenManager, Page>(this, MessengerCodes.ScreenPushPage, (sender, page) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await MainPage.Navigation.PushAsync(page);
                });
            });

            MessagingCenter.Subscribe<ScreenManager, Page>(this, MessengerCodes.ScreenChangePage, (sender, page) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MainPage = page;
                });
            });

            MessagingCenter.Subscribe<ScreenManager, string[]>(this, MessengerCodes.AlertScreen, (sender, args) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MainPage.DisplayAlert(args[0], args[1], "OK");
                });
            });
        }

        #endregion

        protected override void OnStart()
        {
            // Handle when your app starts
            NetworkManager.Instance.ConnectToServer();

            if (Application.Current.Properties.ContainsKey("AiosKingdom_IdentifyingKey"))
            {
                var identifier = Application.Current.Properties["AiosKingdom_IdentifyingKey"] as string;

                if (!string.IsNullOrEmpty(identifier))
                {
                    NetworkManager.Instance.AskAuthentication(identifier);
                }
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            NetworkManager.Instance.Disconnect();
        }
    }
}
