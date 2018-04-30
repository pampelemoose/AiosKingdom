using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class ServerListPageViewModel : BaseViewModel
    {
        public ServerListPageViewModel()
            : base(null)
        {
            MessagingCenter.Subscribe<NetworkManager, bool>(this, MessengerCodes.InitialDatasReceived, (sender, reset) =>
            {
                NotifyPropertyChanged(nameof(ServerInfos));
                NotifyPropertyChanged(nameof(ShowList));
            });
        }

        public List<Network.GameServerInfos> ServerInfos
        {
            get
            {
                return DatasManager.Instance.ServerInfos;
            }
        }

        public bool ShowList
        {
            get
            {
                return DatasManager.Instance.ServerInfos.Count > 0;
            }
        }

        private ICommand _refreshServersAction;
        public ICommand RefreshServersAction =>
            _refreshServersAction ?? (_refreshServersAction = new Command(() =>
            {
                NetworkManager.Instance.AskServerInfos(false);
            }));
    }
}
