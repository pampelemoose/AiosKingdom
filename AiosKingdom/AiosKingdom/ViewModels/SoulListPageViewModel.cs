using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class SoulListPageViewModel : BaseViewModel
    {
        public SoulListPageViewModel(Network.GameServerConnection connection)
            : base(null)
        {
            Title = "Soul List";

            LoadingScreenManager.Instance.OpenLoadingScreen("Waiting Soul List..");

            MessagingCenter.Subscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived, (sender, souls) =>
            {
                Souls = souls;
                LoadingScreenManager.Instance.CloseLoadingScreen();
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SoulCreationFailed, (sender, message) =>
            {
                LoadingScreenManager.Instance.AlertLoadingScreen("Soul Creation Failed", message);
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SoulConnectionFailed, (sender, message) =>
            {
                LoadingScreenManager.Instance.AlertLoadingScreen("Soul Connection Failed", message);
            });

            NetworkManager.Instance.ConnectToGameServer(connection);
        }

        private List<DataModels.Soul> _souls;
        public List<DataModels.Soul> Souls
        {
            get { return _souls; }
            set
            {
                _souls = value;
                NotifyPropertyChanged();
            }
        }

        private DataModels.Soul _selectedSoul;
        public DataModels.Soul SelectedSoul
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    LoadingScreenManager.Instance.OpenLoadingScreen($"Connecting {value.Name}, please wait...");
                    NetworkManager.Instance.ConnectSoul(value.Id);
                }

                NotifyPropertyChanged();
            }
        }

        private ICommand _createSoulAction;
        public ICommand CreateSoulAction =>
            _createSoulAction ?? (_createSoulAction = new Command(() =>
            {
                LoadingScreenManager.Instance.OpenLoadingScreen("Creating new soul...");
                NetworkManager.Instance.CreateSoul("test");
            }));
    }
}
