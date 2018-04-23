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
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.ConnectionSuccessful, (sender) => {
                IsLoading = false;
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.ConnectionFailed, (sender, arg) => {
                IsLoading = false;
                IsError = true;
                Message = arg;
            });
            
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.LoginSuccessful, (sender) => {
                Message = "Logged In ! Receiving Soul list..";
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.LoginFailed, (sender, arg) => {
                IsLoading = false;
                IsError = true;
                Message = arg;
            });

            NetworkManager.Instance.ConnectToServer();
        }

        ~LoginPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ConnectionFailed);
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

        private bool _isError;
        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
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

        private ICommand _tryToConnectAction;
        public ICommand TryToConnectAction =>
        _tryToConnectAction ?? (_tryToConnectAction = new Command(() =>
        {
            Message = "Connecting to the server. Please wait...";
            IsLoading = true;
            IsError = false;

            NetworkManager.Instance.ConnectToServer();
        }));

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
