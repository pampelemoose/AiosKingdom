using System;
using System.Collections.Generic;
using System.Linq;
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

                NotifyPropertyChanged(nameof(Head));
                NotifyPropertyChanged(nameof(Shoulder));
                NotifyPropertyChanged(nameof(Torso));
                NotifyPropertyChanged(nameof(Belt));
                NotifyPropertyChanged(nameof(Hand));
                NotifyPropertyChanged(nameof(Leg));
                NotifyPropertyChanged(nameof(Pants));
                NotifyPropertyChanged(nameof(Feet));
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

        public DataModels.Items.Armor Head => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Soul.Equipment.Head));
        public DataModels.Items.Armor Shoulder => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Soul.Equipment.Shoulder));
        public DataModels.Items.Armor Torso => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Soul.Equipment.Torso));
        public DataModels.Items.Armor Belt => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Soul.Equipment.Belt));
        public DataModels.Items.Armor Hand => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Soul.Equipment.Hand));
        public DataModels.Items.Armor Leg => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Soul.Equipment.Leg));
        public DataModels.Items.Armor Pants => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Soul.Equipment.Pants));
        public DataModels.Items.Armor Feet => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Soul.Equipment.Feet));
        //public DataModels.Items.Bag Bag => DatasManager.Instance.Bags?.FirstOrDefault(a => a.ItemId.Equals(Soul.Equipment.Bag));

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
                var armor = (DataModels.Items.Armor)item;

                if (armor == null) return;

                if (_selectedItem == null || (_selectedItem != null && _selectedItem == armor))
                {
                    ShowArmorDetails = !ShowArmorDetails;
                }

                if (ShowArmorDetails)
                {
                    _selectedItem = armor;
                    NotifyPropertyChanged(nameof(SelectedItem));
                }
            }));
    }
}
