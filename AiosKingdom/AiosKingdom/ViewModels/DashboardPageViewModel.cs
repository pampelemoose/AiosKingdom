﻿using System;
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

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.CurrenciesUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Currencies));
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.EquipmentUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Equipment));

                NotifyPropertyChanged(nameof(Head));
                NotifyPropertyChanged(nameof(Shoulder));
                NotifyPropertyChanged(nameof(Torso));
                NotifyPropertyChanged(nameof(Belt));
                NotifyPropertyChanged(nameof(Hand));
                NotifyPropertyChanged(nameof(Leg));
                NotifyPropertyChanged(nameof(Pants));
                NotifyPropertyChanged(nameof(Feet));

                NotifyPropertyChanged(nameof(WeaponRight));
                NotifyPropertyChanged(nameof(WeaponLeft));
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulDatasUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Datas));
            });

            NetworkManager.Instance.AskCurrencies();
            NetworkManager.Instance.AskEquipment();
            NetworkManager.Instance.AskSoulCurrentDatas();
        }

        ~DashboardPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.CurrenciesUpdated);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.EquipmentUpdated);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.SoulDatasUpdated);
        }

        public Network.Currencies Currencies => DatasManager.Instance.Currencies;
        public DataModels.Equipment Equipment => DatasManager.Instance.Equipment;
        public Network.SoulDatas Datas => DatasManager.Instance.Datas;

        public DataModels.Items.Armor Head => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Equipment.Head));
        public DataModels.Items.Armor Shoulder => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Equipment.Shoulder));
        public DataModels.Items.Armor Torso => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Equipment.Torso));
        public DataModels.Items.Armor Belt => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Equipment.Belt));
        public DataModels.Items.Armor Hand => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Equipment.Hand));
        public DataModels.Items.Armor Leg => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Equipment.Leg));
        public DataModels.Items.Armor Pants => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Equipment.Pants));
        public DataModels.Items.Armor Feet => DatasManager.Instance.Armors?.FirstOrDefault(a => a.ItemId.Equals(Equipment.Feet));

        public DataModels.Items.Weapon WeaponRight => DatasManager.Instance.Weapons?.FirstOrDefault(a => a.ItemId.Equals(Equipment.WeaponRight));
        public DataModels.Items.Weapon WeaponLeft => DatasManager.Instance.Weapons?.FirstOrDefault(a => a.ItemId.Equals(Equipment.WeaponLeft));
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

        private bool _showWeaponDetails;
        public bool ShowWeaponDetails
        {
            get { return _showWeaponDetails; }
            set
            {
                _showWeaponDetails = value;
                NotifyPropertyChanged();
            }
        }

        private DataModels.Items.AItem _selectedItem;
        public DataModels.Items.AItem SelectedItem => _selectedItem;

        private ICommand _showArmorAction;
        public ICommand ShowArmorAction =>
            _showArmorAction ?? (_showArmorAction = new Command((item) =>
            {
                var armor = (DataModels.Items.Armor)item;

                if (armor == null) return;

                if (_selectedItem == null || (_selectedItem != null && _selectedItem == armor) || ShowWeaponDetails)
                {
                    ShowArmorDetails = !ShowArmorDetails;
                }

                if (ShowArmorDetails)
                {
                    ShowWeaponDetails = false;
                    _selectedItem = armor;
                    NotifyPropertyChanged(nameof(SelectedItem));
                }
                else
                {
                    _selectedItem = null;
                }
            }));

        private ICommand _showWeaponAction;
        public ICommand ShowWeaponAction =>
            _showWeaponAction ?? (_showWeaponAction = new Command((item) =>
            {
                var weapon = (DataModels.Items.Weapon)item;

                if (weapon == null) return;

                if (_selectedItem == null || (_selectedItem != null && _selectedItem == weapon) || ShowArmorDetails)
                {
                    ShowWeaponDetails = !ShowWeaponDetails;
                }

                if (ShowWeaponDetails)
                {
                    ShowArmorDetails = false;
                    _selectedItem = weapon;
                    NotifyPropertyChanged(nameof(SelectedItem));
                }
                else
                {
                    _selectedItem = null;
                }
            }));
    }
}
