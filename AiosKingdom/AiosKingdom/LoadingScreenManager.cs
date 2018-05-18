using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AiosKingdom
{
    public class LoadingScreenManager
    {
        private static LoadingScreenManager _instance;
        public static LoadingScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoadingScreenManager();
                }

                return _instance;
            }
        }

        private LoadingScreenManager()
        {
            MessagingCenter.Subscribe<App>(this, MessengerCodes.LoadingScreenOpenned, (sender) =>
            {
                lock (_screenLock)
                {
                    _isOpen = true;
                }
            });

            MessagingCenter.Subscribe<App>(this, MessengerCodes.LoadingScreenClosed, (sender) =>
            {
                lock (_screenLock)
                {
                    _isOpen = false;
                }
            });
        }

        private object _screenLock = new object();
        private bool _isOpen;

        public void OpenLoadingScreen(string message)
        {
            lock (_screenLock)
            {
                if (!_isOpen)
                {
                    MessagingCenter.Send(this, MessengerCodes.OpenLoadingScreen, message);
                }
            }
        }

        public void UpdateLoadingScreen(string message)
        {
            lock (_screenLock)
            {
                if (_isOpen)
                {
                    MessagingCenter.Send(this, MessengerCodes.UpdateLoadingScreen, message);
                }
            }
        }

        public void AlertLoadingScreen(string title, string message)
        {
            lock (_screenLock)
            {
                if (_isOpen)
                {
                    MessagingCenter.Send(this, MessengerCodes.AlertLoadingScreen, new string[2] { title, message });
                }
            }
        }

        public async Task CloseLoadingScreen(bool shouldWait = false)
        {
            if (_isOpen)
            {
                MessagingCenter.Send(this, MessengerCodes.CloseLoadingScreen);

                if (shouldWait)
                {
                    await Task.Run(() =>
                    {
                        var waitingClose = true;
                        while (waitingClose)
                        {
                            lock (_screenLock)
                            {
                                if (!_isOpen)
                                    waitingClose = false;
                            }
                        }
                    });
                }
            }
        }
    }
}
