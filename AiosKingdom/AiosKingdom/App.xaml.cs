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
                    ScreenManager.Instance.ChangePage(new MainPage());
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

            LoadingScreenCallbacks();
        }

        private void Subscribe_GameServerDisconnected()
        {
            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.GameServerDisconnected, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NetworkManager.Instance.DisconnectGame();
                    ScreenManager.Instance.ChangePage(new NavigationPage(new Views.ServerListPage(new List<Network.GameServerInfos>())));
                    MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.GameServerDisconnected);
                });
            });
        }

        #region LOADING SCREEN

        private Views.LoadingPage _loadingPage = new Views.LoadingPage();
        private void LoadingScreenCallbacks()
        {
            MessagingCenter.Subscribe<ScreenManager, string>(this, MessengerCodes.OpenLoadingScreen, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    _loadingPage.SetMessage(message);
                    await MainPage.Navigation.PushModalAsync(_loadingPage);
                    MessagingCenter.Send(this, MessengerCodes.LoadingScreenOpenned);
                });
            });


            MessagingCenter.Subscribe<ScreenManager>(this, MessengerCodes.CloseLoadingScreen, (sender) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (MainPage.Navigation.ModalStack.Count > 0)
                    {
                        await MainPage.Navigation.PopModalAsync();
                        MessagingCenter.Send(this, MessengerCodes.LoadingScreenClosed);
                    }
                });
            });

            MessagingCenter.Subscribe<ScreenManager, Page>(this, MessengerCodes.LoadingScreenChangePage, (sender, page) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (MainPage.Navigation.ModalStack.Count > 0)
                    {
                        await MainPage.Navigation.PopModalAsync();
                        MessagingCenter.Send(this, MessengerCodes.LoadingScreenClosed);
                    }
                    MainPage = page;
                });
            });

            MessagingCenter.Subscribe<ScreenManager, Page>(this, MessengerCodes.LoadingScreenPushPage, (sender, page) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (MainPage.Navigation.ModalStack.Count > 0)
                    {
                        await MainPage.Navigation.PopModalAsync();
                        MessagingCenter.Send(this, MessengerCodes.LoadingScreenClosed);
                    }
                    await MainPage.Navigation.PushAsync(page);
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
