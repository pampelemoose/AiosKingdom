using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class SoulListPageViewModel : BaseViewModel
    {
        public SoulListPageViewModel(List<DataModels.Soul> souls)
            : base(null)
        {
            Title = "Soul List";

            Souls = souls;

            MessagingCenter.Subscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived, (sender, soulsUpdated) =>
            {
                Souls = soulsUpdated;
                ScreenManager.Instance.CloseLoadingScreen();
            });

            // TODO : Fix when Top bar UWP is fixed
            Task.Delay(5000).ContinueWith(t =>
            {
                if (Souls.Count == 0)
                {
                    ScreenManager.Instance.OpenLoadingScreen("Create Soul");
                    ScreenManager.Instance.PushPage(new Views.CreateSoulPage());
                }
            });
        }

        ~SoulListPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager, List<DataModels.Soul>>(this, MessengerCodes.SoulListReceived);
        }

        private List<DataModels.Soul> _souls;
        public List<DataModels.Soul> Souls
        {
            get { return _souls; }
            set
            {
                _souls = value;
                NotifyPropertyChanged();
            }
        }

        private DataModels.Soul _selectedSoul;
        public DataModels.Soul SelectedSoul
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    ScreenManager.Instance.OpenLoadingScreen($"Connecting {value.Name}, please wait...");
                    NetworkManager.Instance.ConnectSoul(value.Id);
                }

                NotifyPropertyChanged();
            }
        }

        private ICommand _createSoulAction;
        public ICommand CreateSoulAction =>
            _createSoulAction ?? (_createSoulAction = new Command(() =>
            {
                ScreenManager.Instance.OpenLoadingScreen("Create Soul");
                ScreenManager.Instance.PushPage(new Views.CreateSoulPage());
            }));
    }
}
