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

        private ICommand _showPanelAction;
        public ICommand ShowPanelAction =>
            _showPanelAction ?? (_showPanelAction = new Command((arg) =>
            {
                string type = (string)arg;

                IsArmorPanelActive = false;
                IsWeaponPanelActive = false;
                IsBagPanelActive = false;
                IsConsumablePanelActive = false;

                switch (type)
                {
                    case "Armors":
                        IsArmorPanelActive = true;
                        break;
                    case "Bags":
                        IsBagPanelActive = true;
                        break;
                    case "Weapons":
                        IsWeaponPanelActive = true;
                        break;
                    case "Consumables":
                        IsConsumablePanelActive = true;
                        break;
                }
                
            }));

        #endregion

        private List<Models.InventoryItemModel> _armors;
        public List<Models.InventoryItemModel> Armors => _armors;

        private Models.InventoryItemModel _selectedArmor;
        public Models.InventoryItemModel SelectedArmor
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

        private List<Models.InventoryItemModel> _bags;
        public List<Models.InventoryItemModel> Bags => _bags;

        private Models.InventoryItemModel _selectedBag;
        public Models.InventoryItemModel SelectedBag
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

        private List<Models.InventoryItemModel> _consumables;
        public List<Models.InventoryItemModel> Consumables => _consumables;

        private Models.InventoryItemModel _selectedConsumable;
        public Models.InventoryItemModel SelectedConsumable
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

        private List<Models.InventoryItemModel> _weapons;
        public List<Models.InventoryItemModel> Weapons => _weapons;

        private Models.InventoryItemModel _selectedWeapon;
        public Models.InventoryItemModel SelectedWeapon
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
            _armors = new List<Models.InventoryItemModel>();
            SelectedArmor = null;

            _bags = new List<Models.InventoryItemModel>();
            SelectedBag = null;

            _consumables = new List<Models.InventoryItemModel>();
            SelectedConsumable = null;

            _weapons = new List<Models.InventoryItemModel>();
            SelectedWeapon = null;

            foreach (var slot in DatasManager.Instance.Inventory.OrderBy(i => i.LootedAt).ToList())
            {
                switch (slot.Type)
                {
                    case Network.Items.ItemType.Armor:
                        {
                            _armors.Add(new Models.InventoryItemModel
                            {
                                Slot = slot,
                                Item = DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(slot.ItemId))
                            });
                        }
                        break;
                    case Network.Items.ItemType.Bag:
                        {
                            _bags.Add(new Models.InventoryItemModel
                            {
                                Slot = slot,
                                Item = DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(slot.ItemId))
                            });
                        }
                        break;
                    case Network.Items.ItemType.Consumable:
                        {
                            _consumables.Add(new Models.InventoryItemModel
                            {
                                Slot = slot,
                                Item = DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(slot.ItemId))
                            });
                        }
                        break;
                    case Network.Items.ItemType.Junk:
                        break;
                    default:
                        {
                            _weapons.Add(new Models.InventoryItemModel
                            {
                                Slot = slot,
                                Item = DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(slot.ItemId))
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
