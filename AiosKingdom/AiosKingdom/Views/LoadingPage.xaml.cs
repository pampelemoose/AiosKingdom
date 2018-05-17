using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AiosKingdom.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<LoadingScreenManager, string>(this, MessengerCodes.UpdateLoadingScreen, (sender, content) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Message.Text = content;
                });
            });

            MessagingCenter.Subscribe<LoadingScreenManager, string[]>(this, MessengerCodes.AlertLoadingScreen, (sender, args) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert(args[0], args[1], "OK");
                    LoadingScreenManager.Instance.CloseLoadingScreen();
                });
            });
        }

        public void SetMessage(string message)
        {
            Message.Text = message;
        }

        ~LoadingPage()
        {
            MessagingCenter.Unsubscribe<LoadingScreenManager, string>(this, MessengerCodes.UpdateLoadingScreen);
            MessagingCenter.Unsubscribe<LoadingScreenManager, string[]>(this, MessengerCodes.AlertLoadingScreen);
        }
    }
}