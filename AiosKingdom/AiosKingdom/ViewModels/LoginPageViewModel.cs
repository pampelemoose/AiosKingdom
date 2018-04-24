using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        public LoginPageViewModel()
            : base(null)
        {
            _stopLoadingAction = new Command(() =>
            {
                ShowButton = false;
                IsLoading = false;
            });

            _tryToConnectAction = new Command(() =>
            {
                Message = "Connecting to the server. Please wait...";
                IsLoading = true;
                ShowButton = false;

                NetworkManager.Instance.ConnectToServer();
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.ConnectionSuccessful, (sender) => {
                IsLoading = false;
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.ConnectionFailed, (sender, arg) => {
                IsLoading = true;
                ShowButton = true;
                Message = arg;
                CloseMessage = _tryToConnectAction;
            });
            
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.LoginSuccessful, (sender) => {
                Message = "Logged In ! Receiving Soul list..";
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.LoginFailed, (sender, arg) => {
                ShowButton = true;
                Message = arg;
                CloseMessage = _stopLoadingAction;
            });

            NetworkManager.Instance.ConnectToServer();
        }

        ~LoginPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.ConnectionSuccessful);
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ConnectionFailed);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.LoginSuccessful);
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.LoginFailed);
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

        private string _message = "Connecting to the server. Please wait...";
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        private bool _showButton;
        public bool ShowButton
        {
            get { return _showButton; }
            set
            {
                _showButton = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _tryToConnectAction;
        private ICommand _stopLoadingAction;

        private ICommand _closeMessage;
        public ICommand CloseMessage
        {
            get { return _closeMessage; }
            set
            {
                _closeMessage = value;
                NotifyPropertyChanged();
            }
        }

        private Command _logInAction;
        public ICommand LogInAction =>
        _logInAction ?? (_logInAction = new Command(() =>
        {
            LogIn();
        }, () => {
            return !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password); }));

        private void LogIn()
        {
            Message = "Logging In. Please wait..";
            IsLoading = true;

            NetworkManager.Instance.AskLogin(_username, _password);
        }
    }
}
