using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AiosKingdom
{
    public class ScreenManager
    {
        private static ScreenManager _instance;
        public static ScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScreenManager();
                }

                return _instance;
            }
        }

        private ScreenManager()
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

        public void AlertScreen(string title, string message)
        {
            lock (_screenLock)
            {
                if (_isOpen)
                {
                    MessagingCenter.Send(this, MessengerCodes.AlertScreen, new string[2] { title, message });
                }
            }
        }

        public void ChangePage(Page page)
        {
            MessagingCenter.Send(this, MessengerCodes.LoadingScreenChangePage, page);
        }

        public void PushPage(Page page)
        {
            MessagingCenter.Send(this, MessengerCodes.LoadingScreenPushPage, page);
        }

        public void CloseLoadingScreen()
        {
            if (_isOpen)
            {
                MessagingCenter.Send(this, MessengerCodes.CloseLoadingScreen);
            }
        }
    }
}
