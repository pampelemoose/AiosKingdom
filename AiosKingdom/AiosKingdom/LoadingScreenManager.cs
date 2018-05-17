using System;
using System.Collections.Generic;
using System.Text;
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
        }

        private object _screeLock = new object();
        private bool _isOpen;

        public void OpenLoadingScreen(string message)
        {
            lock (_screeLock)
            {
                if (!_isOpen)
                {
                    _isOpen = true;
                    MessagingCenter.Send(this, MessengerCodes.OpenLoadingScreen, message);
                }
            }
        }

        public void UpdateLoadingScreen(string message)
        {
            lock (_screeLock)
            {

                if (_isOpen)
                {
                    MessagingCenter.Send(this, MessengerCodes.UpdateLoadingScreen, message);
                }
            }
        }

        public void AlertLoadingScreen(string title, string message)
        {
            lock (_screeLock)
            {
                if (_isOpen)
                {
                    MessagingCenter.Send(this, MessengerCodes.AlertLoadingScreen, new string[2] { title, message });
                }
            }
        }

        public void CloseLoadingScreen()
        {
            lock (_screeLock)
            {
                if (_isOpen)
                {
                    _isOpen = false;
                    MessagingCenter.Send(this, MessengerCodes.CloseLoadingScreen);
                }
            }
        }
    }
}
