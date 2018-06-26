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
            });

            MessagingCenter.Subscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived, (sender, servers) =>
            {
                LoadingScreenManager.Instance.ChangePage(new NavigationPage(new Views.ServerListPage(servers)));
            });
        }

        ~LoginPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ConnectionFailed);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.LoginSuccessful);
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.LoginFailed);
            MessagingCenter.Unsubscribe<NetworkManager, List<Network.GameServerInfos>>(this, MessengerCodes.ServerListReceived);
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyPropertyChanged();
                _logInAction?.ChangeCanExecute();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged();
                _logInAction?.ChangeCanExecute();
            }
        }

        private Command _logInAction;
        public ICommand LogInAction =>
        _logInAction ?? (_logInAction = new Command(() =>
        {
            LogIn();
        }, () =>
        {
            return !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password);
        }));

        private void LogIn()
        {
            LoadingScreenManager.Instance.OpenLoadingScreen("Connecting to the server. Please wait...");
            NetworkManager.Instance.ConnectToServer(_username, _password);
        }
    }
}
