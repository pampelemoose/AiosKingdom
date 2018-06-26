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
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadingScreenManager.Instance.UpdateLoadingScreen("Loading Soul list, please wait.");
                    NetworkManager.Instance.AskSoulList();
                });
            });

            MessagingCenter.Subscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived, (sender, souls) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadingScreenManager.Instance.PushPage(new Views.SoulListPage(new SoulListPageViewModel(souls)));
                });
            });
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
                    LoadingScreenManager.Instance.OpenLoadingScreen($"Connecting to {value.Name}, please wait...");
                    NetworkManager.Instance.AnnounceGameServerConnection(value.Id);
                }

                NotifyPropertyChanged();
            }
        }

        private ICommand _refreshServersAction;
        public ICommand RefreshServersAction =>
            _refreshServersAction ?? (_refreshServersAction = new Command(() =>
            {
                LoadingScreenManager.Instance.OpenLoadingScreen("Refreshing server list, please wait.");
                NetworkManager.Instance.AskServerInfos();
            }));
    }
}
