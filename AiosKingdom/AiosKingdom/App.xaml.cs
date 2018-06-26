﻿using System;
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
                    LoadingScreenManager.Instance.ChangePage(new Views.LoginPage());
                });
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.GameServerDisconnected, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NetworkManager.Instance.DisconnectGame();
                    LoadingScreenManager.Instance.ChangePage(new NavigationPage(new Views.ServerListPage(new List<Network.GameServerInfos>())));
                });
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulConnected, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadingScreenManager.Instance.ChangePage(new MainPage());
                });
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.EnterDungeon, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadingScreenManager.Instance.ChangePage(new Views.Dungeon.DungeonPage());
                });
            });

            MainPage = new Views.LoginPage();

            LoadingScreenCallbacks();
        }

        private Views.LoadingPage _loadingPage = new Views.LoadingPage();
        private void LoadingScreenCallbacks()
        {
            MessagingCenter.Subscribe<LoadingScreenManager, string>(this, MessengerCodes.OpenLoadingScreen, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    _loadingPage.SetMessage(message);
                    await MainPage.Navigation.PushModalAsync(_loadingPage);
                    MessagingCenter.Send(this, MessengerCodes.LoadingScreenOpenned);
                });
            });


            MessagingCenter.Subscribe<LoadingScreenManager>(this, MessengerCodes.CloseLoadingScreen, (sender) =>
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

            MessagingCenter.Subscribe<LoadingScreenManager, Page>(this, MessengerCodes.LoadingScreenChangePage, (sender, page) =>
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

            MessagingCenter.Subscribe<LoadingScreenManager, Page>(this, MessengerCodes.LoadingScreenPushPage, (sender, page) =>
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
