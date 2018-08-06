using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class ServerListPageViewModel : BaseViewModel
    {
        public ServerListPageViewModel(INavigation nav, List<Network.GameServerInfos> serverList)
            : base(nav)
        {
            Title = "Server List";
            ServerInfos = serverList;

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.ConnectedToServerFailed, (sender, arg) =>
            {
                ScreenManager.Instance.AlertScreen("Connection Failed", $"Couldn't connect to server. Please try again later.\n{arg}");
                IsBusy = false;
            });

            MessagingCenter.Subscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived, (sender, servers) =>
            {
                ServerInfos = servers;
                IsBusy = false;
            });
        }

        private bool _subscribed;
        private void Subscribe_SoulListReceived()
        {
            if (!_subscribed)
            {
                MessagingCenter.Subscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived, (sender, souls) =>
                {
                    ScreenManager.Instance.PushPage(new Views.SoulListPage(new SoulListPageViewModel(souls)));
                    IsBusy = false;
                    _subscribed = false;
                    MessagingCenter.Unsubscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived);
                });
                _subscribed = true;
            }
        }

        ~ServerListPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ConnectedToServerFailed);
            MessagingCenter.Unsubscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived);
        }

        private List<Network.GameServerInfos> _serverInfos;
        public List<Network.GameServerInfos> ServerInfos
        {
            get { return _serverInfos; }
            set
            {
                _serverInfos = value;
                NotifyPropertyChanged();
            }
        }

        public Network.GameServerInfos SelectedInfo
        {
            get { return null; }
            set
            {
                if (value != null && value.Online)
                {
                    Subscribe_SoulListReceived();

                    IsBusy = true;
                    NetworkManager.Instance.AnnounceGameServerConnection(value.Id);
                }

                value = null;
                NotifyPropertyChanged();
            }
        }

        private ICommand _refreshServersAction;
        public ICommand RefreshServersAction =>
            _refreshServersAction ?? (_refreshServersAction = new Command(() =>
            {
                IsBusy = true;
                NetworkManager.Instance.AskServerInfos();
            }));
    }
}
