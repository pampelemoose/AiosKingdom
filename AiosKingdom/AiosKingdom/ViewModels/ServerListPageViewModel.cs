﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class ServerListPageViewModel : BaseViewModel
    {
        public ServerListPageViewModel(INavigation nav)
            : base(nav)
        {
            Title = "Server List";

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.InitialDatasReceived, (sender) =>
            {
                NotifyPropertyChanged(nameof(ServerInfos));
            });

            MessagingCenter.Subscribe<NetworkManager, Network.GameServerConnection>(this, MessengerCodes.GameServerDatasReceived, (sender, connection) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await _navigation.PushAsync(new Views.SoulListPage(new ViewModels.SoulListPageViewModel(connection)));
                    IsLoading = false;
                });
            });
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public List<Network.GameServerInfos> ServerInfos
        {
            get
            {
                return DatasManager.Instance.ServerInfos;
            }
        }

        private Network.GameServerInfos _selectedInfo;
        public Network.GameServerInfos SelectedInfo
        {
            get { return null; }
            set
            {
                if (value != null && value.Online)
                {
                    NetworkManager.Instance.AnnounceGameServerConnection(value.Id);
                    IsLoading = true;
                    Message = $"Connecting to {value.Name}, please wait...";
                }

                NotifyPropertyChanged();
            }
        }

        private ICommand _refreshServersAction;
        public ICommand RefreshServersAction =>
            _refreshServersAction ?? (_refreshServersAction = new Command(() =>
            {
                NetworkManager.Instance.AskServerInfos();
            }));
    }
}
