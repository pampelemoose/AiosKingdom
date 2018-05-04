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

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulListReceived, (sender) =>
            {
                IsLoading = false;
                NotifyPropertyChanged(nameof(Souls));
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SoulCreationFailed, (sender, message) =>
            {
                Message = message;
                Task.Run(() =>
                {
                    Task.Delay(2000);
                    IsLoading = false;
                });
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

        public List<DataModels.Soul> Souls => DatasManager.Instance.Souls;

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
