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
        public SoulListPageViewModel(List<DataModels.Soul> souls)
            : base(null)
        {
            Title = "Soul List";

            Souls = souls;

            MessagingCenter.Subscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived, (sender, soulsUpdated) =>
            {
                IsBusy = false;
                Souls = soulsUpdated;
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.CreateSoulFailed, (sender) =>
            {
                IsBusy = false;
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.CreateSoulSuccess, (sender) =>
            {
                IsBusy = false;
                ShowCreatePanel = false;
            });
        }

        ~SoulListPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived);
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
                    IsBusy = true;
                    NetworkManager.Instance.ConnectSoul(value.Id);
                }

                NotifyPropertyChanged();
            }
        }

        private ICommand _createSoulAction;
        public ICommand CreateSoulAction =>
            _createSoulAction ?? (_createSoulAction = new Command(() =>
            {
                ShowCreatePanel = true;
            }));

        private bool _showCreatePanel;
        public bool ShowCreatePanel
        {
            get { return _showCreatePanel; }
            set
            {
                _showCreatePanel = value;
                NotifyPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _createAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        private Command _createAction;
        public ICommand CreateAction =>
            _createAction ?? (_createAction = new Command(() =>
            {
                IsBusy = true;
                NetworkManager.Instance.CreateSoul(_name);
            }, () => { return !string.IsNullOrWhiteSpace(_name) && _name.Length > 4; }));

        private Command _closeAction;
        public ICommand CloseAction =>
            _closeAction ?? (_closeAction = new Command(() =>
            {
                ShowCreatePanel = false;
            }, () => { return !IsBusy; }));

        private Command _backAction;
        public ICommand BackAction =>
            _backAction ?? (_backAction = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NetworkManager.Instance.DisconnectGame();
                    ScreenManager.Instance.ChangePage(new Views.ServerListPage(new List<Network.GameServerInfos>()));
                });
            }));
    }
}
