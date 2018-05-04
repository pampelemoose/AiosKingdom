using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
