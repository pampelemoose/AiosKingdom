using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        public LoginPageViewModel()
            : base(null)
        {
            // TODO : Show `connect` button to retry the process.
            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.ConnectionFailed, (sender, arg) =>
            {
                ScreenManager.Instance.AlertScreen(MessengerCodes.ConnectionFailed, arg);
                IsBusy = false;
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.LoginSuccessful, (sender) =>
            {
                IsBusy = true;
                NetworkManager.Instance.AskServerInfos();
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.LoginFailed, (sender, arg) =>
            {
                ScreenManager.Instance.AlertScreen("Login Failed", arg);
                Application.Current.Properties.Remove("AiosKingdom_IdentifyingKey");
                IsNewDevice = true;
                IsBusy = false;
            });

            #region Account Management
            MessagingCenter.Subscribe<NetworkManager, Guid>(this, MessengerCodes.CreateNewAccount, (sender, arg) =>
            {
                ScreenManager.Instance.AlertScreen("Account", $"Please save this SafeKey[{arg}] in case you need to retrieve your account to any Device.");
                IsBusy = false;
                var identifier = Application.Current.Properties["AiosKingdom_IdentifyingKey"] as string;

                if (!string.IsNullOrEmpty(identifier))
                {
                    IsBusy = true;
                    NetworkManager.Instance.AskAuthentication(identifier);
                }
                IsNewDevice = false;
            });

            // TODO : Show `retry` button to retry the process.
            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.CreateNewAccountFailed, (sender, arg) =>
            {
                ScreenManager.Instance.AlertScreen("Account", $"There were an error creating a new account. Please try again later.\n{arg}");
                IsBusy = false;
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.RetrievedAccount, (sender) =>
            {
                IsBusy = false;
                var identifier = Application.Current.Properties["AiosKingdom_IdentifyingKey"] as string;

                if (!string.IsNullOrEmpty(identifier))
                {
                    IsBusy = true;
                    NetworkManager.Instance.AskAuthentication(identifier);
                }
                IsNewDevice = false;
                MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.RetrievedAccount);
            });
            #endregion

            MessagingCenter.Subscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived, (sender, servers) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = false;
                    ScreenManager.Instance.ChangePage(new Views.ServerListPage(servers));
                    MessagingCenter.Unsubscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived);
                });
            });

            _isNewDevice = true;
            if (Application.Current.Properties.ContainsKey("AiosKingdom_IdentifyingKey"))
            {
                var identifier = Application.Current.Properties["AiosKingdom_IdentifyingKey"] as string;

                if (!string.IsNullOrEmpty(identifier))
                {
                    _isNewDevice = false;
                    IsBusy = true;
                }
            }
        }

        ~LoginPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ConnectionFailed);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.LoginSuccessful);
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.LoginFailed);
            MessagingCenter.Unsubscribe<NetworkManager, Guid>(this, MessengerCodes.CreateNewAccount);
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.CreateNewAccountFailed);
        }

        private bool _isNewDevice;
        public bool IsNewDevice
        {
            get { return _isNewDevice; }
            set
            {
                _isNewDevice = value;
                NotifyPropertyChanged();
            }
        }

        private Guid _key;
        private string _safeKey;
        public string SafeKey
        {
            get { return _safeKey; }
            set
            {
                _safeKey = value;
                NotifyPropertyChanged();
                _retrieveAccountAction?.ChangeCanExecute();
            }
        }

        private Command _retrieveAccountAction;
        public ICommand RetrieveAccountAction =>
        _retrieveAccountAction ?? (_retrieveAccountAction = new Command(() =>
        {
            IsBusy = true;
            NetworkManager.Instance.AskOldAccount(_safeKey);
        }, () =>
        {
            return !string.IsNullOrEmpty(_safeKey) && Guid.TryParse(_safeKey, out _key) && !Guid.Empty.Equals(_key);
        }));

        private ICommand _newAccountAction;
        public ICommand NewAccountAction =>
        _newAccountAction ?? (_newAccountAction = new Command(() =>
        {
            IsBusy = true;
            NetworkManager.Instance.AskNewAccount();
        }));
    }
}
