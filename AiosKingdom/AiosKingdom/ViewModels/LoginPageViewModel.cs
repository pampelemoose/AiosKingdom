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
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.LoginSuccessful, (sender) =>
            {
                LoadingScreenManager.Instance.UpdateLoadingScreen("Logged In ! Receiving Server list..");
                NetworkManager.Instance.AskServerInfos();
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.ConnectionFailed, (sender, arg) =>
            {
                LoadingScreenManager.Instance.AlertLoadingScreen(MessengerCodes.ConnectionFailed, arg);
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.LoginFailed, (sender, arg) =>
            {
                LoadingScreenManager.Instance.AlertLoadingScreen("Login Failed", arg);
                Application.Current.Properties.Remove("AiosKingdom_IdentifyingKey");
                IsNewDevice = true;
            });

            MessagingCenter.Subscribe<NetworkManager, Guid>(this, MessengerCodes.CreateNewAccount, (sender, arg) =>
            {
                LoadingScreenManager.Instance.AlertLoadingScreen("New Account Created", $"Please save this SafeKey[{arg}] in case you need to retrieve your account to any Device.");
                var identifier = Application.Current.Properties["AiosKingdom_IdentifyingKey"] as string;

                if (!string.IsNullOrEmpty(identifier))
                {
                    NetworkManager.Instance.AskAuthentication(identifier);
                }
                IsNewDevice = false;
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.RetrievedAccount, (sender) =>
            {
                var identifier = Application.Current.Properties["AiosKingdom_IdentifyingKey"] as string;

                if (!string.IsNullOrEmpty(identifier))
                {
                    NetworkManager.Instance.AskAuthentication(identifier);
                }
                IsNewDevice = false;
            });

            MessagingCenter.Subscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived, (sender, servers) =>
            {
                LoadingScreenManager.Instance.ChangePage(new NavigationPage(new Views.ServerListPage(servers)));
            });

            _isNewDevice = true;
            if (Application.Current.Properties.ContainsKey("AiosKingdom_IdentifyingKey"))
            {
                var identifier = Application.Current.Properties["AiosKingdom_IdentifyingKey"] as string;

                if (!string.IsNullOrEmpty(identifier))
                {
                    _isNewDevice = false;
                }
            }
        }

        ~LoginPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ConnectionFailed);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.LoginSuccessful);
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.LoginFailed);
            MessagingCenter.Unsubscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived);
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
            LoadingScreenManager.Instance.OpenLoadingScreen("Try to retrieve account. Please wait...");
            NetworkManager.Instance.AskOldAccount(_safeKey);
        }, () =>
        {
            return !string.IsNullOrEmpty(_safeKey) && Guid.TryParse(_safeKey, out _key) && !Guid.Empty.Equals(_key);
        }));

        private ICommand _newAccountAction;
        public ICommand NewAccountAction =>
        _newAccountAction ?? (_newAccountAction = new Command(() =>
        {
            LoadingScreenManager.Instance.OpenLoadingScreen("Creating new account. Please wait...");
            NetworkManager.Instance.AskNewAccount();
        }));
    }
}
