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
        }

        public void AlertScreen(string title, string message)
        {
            MessagingCenter.Send(this, MessengerCodes.AlertScreen, new string[2] { title, message });
        }

        public void ChangePage(Page page)
        {
            MessagingCenter.Send(this, MessengerCodes.ScreenChangePage, page);
        }

        public void PushPage(Page page)
        {
            MessagingCenter.Send(this, MessengerCodes.ScreenPushPage, page);
        }
    }
}
