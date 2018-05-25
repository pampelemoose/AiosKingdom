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

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulUpdated, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetInventories();
                    LoadingScreenManager.Instance.CloseLoadingScreen();
                });
            });

            SetInventories();
        }

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
                NetworkManager.Instance.EquipItem(_selectedArmor.Slot.Id);
                LoadingScreenManager.Instance.OpenLoadingScreen($"Equiping {_selectedArmor.Item.Name}...");
            }, () => { return _armorSlotIsSelected && _selectedArmor?.Item.UseLevelRequired <= DatasManager.Instance.Soul.Level; }));

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
                //NetworkManager.Instance.EquipItem(_selectedArmor.Slot.Id);
            }, () => { return _bagSlotIsSelected && _selectedBag?.Item.UseLevelRequired <= DatasManager.Instance.Soul.Level; }));

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

        private void SetInventories()
        {
            _armors = new List<Models.InventoryItemModel<DataModels.Items.Armor>>();
            SelectedArmor = null;

            _bags = new List<Models.InventoryItemModel<DataModels.Items.Bag>>();
            SelectedBag = null;

            _consumables = new List<Models.InventoryItemModel<DataModels.Items.Consumable>>();
            SelectedConsumable = null;

            foreach (var slot in DatasManager.Instance.Soul.Inventory.OrderBy(i => i.LootedAt).ToList())
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
                }
            }

            NotifyPropertyChanged(nameof(Armors));
            NotifyPropertyChanged(nameof(SelectedArmor));

            NotifyPropertyChanged(nameof(Bags));
            NotifyPropertyChanged(nameof(SelectedBag));

            NotifyPropertyChanged(nameof(Consumables));
            NotifyPropertyChanged(nameof(SelectedConsumable));
        }
    }
}
