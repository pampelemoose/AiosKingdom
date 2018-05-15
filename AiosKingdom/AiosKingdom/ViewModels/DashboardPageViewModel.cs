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

        public DataModels.Items.Armor Head => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Soul.Equipment.Head));
        public DataModels.Items.Armor Shoulder => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Soul.Equipment.Shoulder));
        public DataModels.Items.Armor Torso => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Soul.Equipment.Torso));
        public DataModels.Items.Armor Belt => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Soul.Equipment.Belt));
        public DataModels.Items.Armor Hand => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Soul.Equipment.Hand));
        public DataModels.Items.Armor Leg => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Soul.Equipment.Leg));
        public DataModels.Items.Armor Pants => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Soul.Equipment.Pants));
        public DataModels.Items.Armor Feet => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Soul.Equipment.Feet));
        //public DataModels.Items.Bag Bag => DatasManager.Instance.Bags?.FirstOrDefault(a => a.Id.Equals(Soul.Equipment.Bag));

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
                var armor = DatasManager.Instance.Armors.FirstOrDefault(a => a.ItemId.Equals((Guid)item));

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
