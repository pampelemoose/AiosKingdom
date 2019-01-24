using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels.Dungeon
{
    public class ExitDungeonPageViewModel : BaseViewModel
    {
        public ExitDungeonPageViewModel(INavigation nav)
            : base(nav)
        {
            MessagingCenter.Subscribe<DungeonPageViewModel>(this, MessengerCodes.OpenExitDungeon, (sender) =>
            {
                ExitDungeonVisible = true;
            });
        }

        public string Confirmation => $"Are you sure you want to leave the dungeon ?";
        public string Warning => $"**You will lose {DatasManager.Instance.Adventure.StackedExperience} experience. You will lose all your items in your Bag.**";

        private bool _exitDungeonVisible;
        public bool ExitDungeonVisible
        {
            get { return _exitDungeonVisible; }
            set
            {
                _exitDungeonVisible = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _closeAction;
        public ICommand CloseAction =>
            _closeAction ?? (_closeAction = new Command(() =>
            {
                ExitDungeonVisible = false;
            }));

        private ICommand _exitAction;
        public ICommand ExitAction =>
            _exitAction ?? (_exitAction = new Command(() =>
            {
                Subscribe_ExitDungeon();

                IsBusy = true;
                NetworkManager.Instance.ExitDungeon();
            }));

        private void Subscribe_ExitDungeon()
        {
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.ExitedDungeon, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ExitDungeonVisible = false;
                    MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.ExitedDungeon);
                });
            });
        }
    }
}
