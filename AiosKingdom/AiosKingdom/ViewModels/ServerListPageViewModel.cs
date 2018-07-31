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

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.ConnectedToServer, (sender) =>
            {
                ScreenManager.Instance.UpdateLoadingScreen("Loading Soul list, please wait.");
                NetworkManager.Instance.AskSoulList();
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.ConnectedToServerFailed, (sender, arg) =>
            {
                ScreenManager.Instance.AlertScreen("Connection Failed", $"Couldn't connect to server. Please try again later.\n{arg}");
            });

            MessagingCenter.Subscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived, (sender, servers) =>
            {
                ServerInfos = servers;
                ScreenManager.Instance.CloseLoadingScreen();
            });

            MessagingCenter.Subscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived, (sender, souls) =>
            {
                ScreenManager.Instance.PushPage(new Views.SoulListPage(new SoulListPageViewModel(souls)));
            });
        }

        ~ServerListPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.ConnectedToServer);
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ConnectedToServerFailed);
            MessagingCenter.Unsubscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived);
            MessagingCenter.Unsubscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived);
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
                    ScreenManager.Instance.OpenLoadingScreen($"Connecting to {value.Name}, please wait...");
                    NetworkManager.Instance.AnnounceGameServerConnection(value.Id);
                }

                NotifyPropertyChanged();
            }
        }

        private ICommand _refreshServersAction;
        public ICommand RefreshServersAction =>
            _refreshServersAction ?? (_refreshServersAction = new Command(() =>
            {
                ScreenManager.Instance.OpenLoadingScreen("Refreshing server list, please wait.");
                NetworkManager.Instance.AskServerInfos();
            }));
    }
}
