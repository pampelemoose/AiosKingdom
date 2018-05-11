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
            Message = "Waiting Soul List..";

            MessagingCenter.Subscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived, (sender, souls) =>
            {
                Souls = souls;
                IsLoading = false;
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SoulCreationFailed, (sender, message) =>
            {
                IsLoading = false;
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SoulConnectionFailed, (sender, message) =>
            {
                IsLoading = false;
            });

            NetworkManager.Instance.ConnectToGameServer(connection);
        }

        private bool _isLoading = true;
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
                    NetworkManager.Instance.ConnectSoul(value.Id);
                    IsLoading = true;
                    Message = $"Connecting {value.Name}, please wait...";
                }

                NotifyPropertyChanged();
            }
        }

        private ICommand _createSoulAction;
        public ICommand CreateSoulAction =>
            _createSoulAction ?? (_createSoulAction = new Command(() =>
            {
                NetworkManager.Instance.CreateSoul("test");
                IsLoading = true;
                Message = "Creating new soul...";
            }));
    }
}
