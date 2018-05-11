using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class DashboardPageViewModel : BaseViewModel
    {
        public DashboardPageViewModel()
            : base(null)
        {
            Title = "Dashboard";

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Soul));
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulDatasUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Datas));
            });

            NetworkManager.Instance.AskSoulDatas();
            NetworkManager.Instance.AskSoulCurrentDatas();
        }

        public DataModels.Soul Soul => DatasManager.Instance.Soul;
        public Network.SoulDatas Datas => DatasManager.Instance.Datas;

        private bool _showArmorDetails;
        public bool ShowArmorDetails
        {
            get { return _showArmorDetails; }
            set
            {
                _showArmorDetails = value;
                NotifyPropertyChanged();
            }
        }

        private DataModels.Items.AItem _selectedItem;
        public DataModels.Items.AItem SelectedItem => _selectedItem;

        private Command _showArmorAction;
        public ICommand ShowArmorAction =>
            _showArmorAction ?? (_showArmorAction = new Command((item) =>
            {
                var armor = (DataModels.Items.AItem)item;

                if (armor == null) return;

                ShowArmorDetails = !ShowArmorDetails;

                if (ShowArmorDetails)
                {
                    _selectedItem = armor;
                    NotifyPropertyChanged(nameof(SelectedItem));
                }
            }));
    }
}
