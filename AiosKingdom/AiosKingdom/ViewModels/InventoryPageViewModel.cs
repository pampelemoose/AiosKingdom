using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class InventoryPageViewModel : BaseViewModel
    {
        public InventoryPageViewModel()
            : base(null)
        {
            Title = "Inventory";

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.InventoryUpdated, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetInventories();
                    IsBusy = false;
                });
            });

            NetworkManager.Instance.AskInventory();

            IsInfoVisible = false;
        }

        ~InventoryPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.InventoryUpdated);
        }

        #region Panels IsActive

        private bool _isArmorPanelActive = true;
        public bool IsArmorPanelActive
        {
            get { return _isArmorPanelActive; }
            set
            {
                _isArmorPanelActive = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isWeaponPanelActive;
        public bool IsWeaponPanelActive
        {
            get { return _isWeaponPanelActive; }
            set
            {
                _isWeaponPanelActive = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBagPanelActive;
        public bool IsBagPanelActive
        {
            get { return _isBagPanelActive; }
            set
            {
                _isBagPanelActive = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isConsumablePanelActive;
        public bool IsConsumablePanelActive
        {
            get { return _isConsumablePanelActive; }
            set
            {
                _isConsumablePanelActive = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _showArmorPanelAction;
        public ICommand ShowArmorPanelAction =>
            _showArmorPanelAction ?? (_showArmorPanelAction = new Command(() =>
            {
                IsArmorPanelActive = true;
                IsWeaponPanelActive = false;
                IsBagPanelActive = false;
                IsConsumablePanelActive = false;
            }));

        private ICommand _showWeaponPanelAction;
        public ICommand ShowWeaponPanelAction =>
            _showWeaponPanelAction ?? (_showWeaponPanelAction = new Command(() =>
            {
                IsArmorPanelActive = false;
                IsWeaponPanelActive = true;
                IsBagPanelActive = false;
                IsConsumablePanelActive = false;
            }));

        private ICommand _showBagPanelAction;
        public ICommand ShowBagPanelAction =>
            _showBagPanelAction ?? (_showBagPanelAction = new Command(() =>
            {
                IsArmorPanelActive = false;
                IsWeaponPanelActive = false;
                IsBagPanelActive = true;
                IsConsumablePanelActive = false;
            }));

        private ICommand _showConsumablePanelAction;
        public ICommand ShowConsumablePanelAction =>
            _showConsumablePanelAction ?? (_showConsumablePanelAction = new Command(() =>
            {
                IsArmorPanelActive = false;
                IsWeaponPanelActive = false;
                IsBagPanelActive = false;
                IsConsumablePanelActive = true;
            }));

        #endregion

        private List<Models.InventoryItemModel<DataModels.Items.Armor>> _armors;
        public List<Models.InventoryItemModel<DataModels.Items.Armor>> Armors => _armors;

        private Models.InventoryItemModel<DataModels.Items.Armor> _selectedArmor;
        public Models.InventoryItemModel<DataModels.Items.Armor> SelectedArmor
        {
            get { return _selectedArmor; }
            set
            {
                _selectedArmor = value;
                _armorSlotIsSelected = _selectedArmor != null;
                _armorSlotEquipAction?.ChangeCanExecute();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ArmorSlotIsSelected));
            }
        }

        private bool _armorSlotIsSelected;
        public bool ArmorSlotIsSelected => _armorSlotIsSelected;

        private Command _armorSlotEquipAction;
        public ICommand ArmorSlotEquipAction =>
            _armorSlotEquipAction ?? (_armorSlotEquipAction = new Command(() =>
            {
                MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.ItemEquiped, (sender, msg) =>
                {
                    IsBusy = false;
                    IsInfoVisible = true;
                    ResultMessage = msg;

                    MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ItemEquiped);
                });

                NetworkManager.Instance.EquipItem(_selectedArmor.Slot.Id);
                IsBusy = true;
            }, () => { return _armorSlotIsSelected && _selectedArmor?.Item.UseLevelRequired <= DatasManager.Instance.Datas.Level; }));

        private List<Models.InventoryItemModel<DataModels.Items.Bag>> _bags;
        public List<Models.InventoryItemModel<DataModels.Items.Bag>> Bags => _bags;

        private Models.InventoryItemModel<DataModels.Items.Bag> _selectedBag;
        public Models.InventoryItemModel<DataModels.Items.Bag> SelectedBag
        {
            get { return _selectedBag; }
            set
            {
                _selectedBag = value;
                _bagSlotIsSelected = _selectedBag != null;
                _bagSlotEquipAction?.ChangeCanExecute();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(BagSlotIsSelected));
            }
        }

        private bool _bagSlotIsSelected;
        public bool BagSlotIsSelected => _bagSlotIsSelected;

        private Command _bagSlotEquipAction;
        public ICommand BagSlotEquipAction =>
            _bagSlotEquipAction ?? (_bagSlotEquipAction = new Command(() =>
            {
                //NetworkManager.Instance.EquipItem(_selectedWeapon.Slot.Id);
            }, () => { return _bagSlotIsSelected && _selectedBag?.Item.UseLevelRequired <= DatasManager.Instance.Datas.Level; }));

        private List<Models.InventoryItemModel<DataModels.Items.Consumable>> _consumables;
        public List<Models.InventoryItemModel<DataModels.Items.Consumable>> Consumables => _consumables;

        private Models.InventoryItemModel<DataModels.Items.Consumable> _selectedConsumable;
        public Models.InventoryItemModel<DataModels.Items.Consumable> SelectedConsumable
        {
            get { return _selectedConsumable; }
            set
            {
                _selectedConsumable = value;
                _consumableSlotIsSelected = _selectedConsumable != null;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ConsumableSlotIsSelected));
            }
        }

        private bool _consumableSlotIsSelected;
        public bool ConsumableSlotIsSelected => _consumableSlotIsSelected;

        private List<Models.InventoryItemModel<DataModels.Items.Weapon>> _weapons;
        public List<Models.InventoryItemModel<DataModels.Items.Weapon>> Weapons => _weapons;

        private Models.InventoryItemModel<DataModels.Items.Weapon> _selectedWeapon;
        public Models.InventoryItemModel<DataModels.Items.Weapon> SelectedWeapon
        {
            get { return _selectedWeapon; }
            set
            {
                _selectedWeapon = value;
                _weaponSlotIsSelected = _selectedWeapon != null;
                _weaponSlotEquipAction?.ChangeCanExecute();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(WeaponSlotIsSelected));
            }
        }

        private bool _weaponSlotIsSelected;
        public bool WeaponSlotIsSelected => _weaponSlotIsSelected;

        private Command _weaponSlotEquipAction;
        public ICommand WeaponSlotEquipAction =>
            _weaponSlotEquipAction ?? (_weaponSlotEquipAction = new Command(() =>
            {
                MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.ItemEquiped, (sender, msg) =>
                {
                    IsBusy = false;
                    IsInfoVisible = true;
                    ResultMessage = msg;

                    MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ItemEquiped);
                });

                NetworkManager.Instance.EquipItem(_selectedWeapon.Slot.Id);
                IsBusy = true;
            }, () => { return _weaponSlotIsSelected && _selectedWeapon?.Item.UseLevelRequired <= DatasManager.Instance.Datas.Level; }));

        private Command _sellItemAction;
        public ICommand SellItemAction =>
            _sellItemAction ?? (_sellItemAction = new Command(() =>
            {
                if (_isArmorPanelActive && _armorSlotIsSelected)
                    NetworkManager.Instance.SellItem(_selectedArmor.Slot.Id);
                if (_isConsumablePanelActive && _consumableSlotIsSelected)
                    NetworkManager.Instance.SellItem(_selectedConsumable.Slot.Id);
                if (_isBagPanelActive && _bagSlotIsSelected)
                    NetworkManager.Instance.SellItem(_selectedBag.Slot.Id);
                if (_isWeaponPanelActive && _weaponSlotIsSelected)
                    NetworkManager.Instance.SellItem(_selectedWeapon.Slot.Id);

                MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.ItemSold, (sender, msg) =>
                {
                    IsBusy = false;
                    IsInfoVisible = true;
                    ResultMessage = msg;

                    MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.ItemSold);
                });

                IsBusy = true;
            }));

        private bool _isInfoVisible;
        public bool IsInfoVisible
        {
            get { return _isInfoVisible; }
            set
            {
                _isInfoVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string _resultMessage;
        public string ResultMessage
        {
            get { return _resultMessage; }
            set
            {
                _resultMessage = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _closeInfoAction;
        public ICommand CloseInfoAction =>
            _closeInfoAction ?? (_closeInfoAction = new Command(() =>
            {
                IsInfoVisible = false;
            }));

        private void SetInventories()
        {
            _armors = new List<Models.InventoryItemModel<DataModels.Items.Armor>>();
            SelectedArmor = null;

            _bags = new List<Models.InventoryItemModel<DataModels.Items.Bag>>();
            SelectedBag = null;

            _consumables = new List<Models.InventoryItemModel<DataModels.Items.Consumable>>();
            SelectedConsumable = null;

            _weapons = new List<Models.InventoryItemModel<DataModels.Items.Weapon>>();
            SelectedWeapon = null;

            foreach (var slot in DatasManager.Instance.Inventory.OrderBy(i => i.LootedAt).ToList())
            {
                switch (slot.Type)
                {
                    case DataModels.Items.ItemType.Armor:
                        {
                            _armors.Add(new Models.InventoryItemModel<DataModels.Items.Armor>
                            {
                                Slot = slot,
                                Item = DatasManager.Instance.Armors.FirstOrDefault(a => a.ItemId.Equals(slot.ItemId))
                            });
                        }
                        break;
                    case DataModels.Items.ItemType.Bag:
                        {
                            _bags.Add(new Models.InventoryItemModel<DataModels.Items.Bag>
                            {
                                Slot = slot,
                                Item = DatasManager.Instance.Bags.FirstOrDefault(a => a.ItemId.Equals(slot.ItemId))
                            });
                        }
                        break;
                    case DataModels.Items.ItemType.Consumable:
                        {
                            _consumables.Add(new Models.InventoryItemModel<DataModels.Items.Consumable>
                            {
                                Slot = slot,
                                Item = DatasManager.Instance.Consumables.FirstOrDefault(a => a.ItemId.Equals(slot.ItemId))
                            });
                        }
                        break;
                    case DataModels.Items.ItemType.Weapon:
                        {
                            _weapons.Add(new Models.InventoryItemModel<DataModels.Items.Weapon>
                            {
                                Slot = slot,
                                Item = DatasManager.Instance.Weapons.FirstOrDefault(a => a.ItemId.Equals(slot.ItemId))
                            });
                        }
                        break;
                }
            }

            NotifyPropertyChanged(nameof(Armors));
            NotifyPropertyChanged(nameof(SelectedArmor));

            NotifyPropertyChanged(nameof(Bags));
            NotifyPropertyChanged(nameof(SelectedBag));

            NotifyPropertyChanged(nameof(Consumables));
            NotifyPropertyChanged(nameof(SelectedConsumable));

            NotifyPropertyChanged(nameof(Weapons));
            NotifyPropertyChanged(nameof(SelectedWeapon));
        }
    }
}
