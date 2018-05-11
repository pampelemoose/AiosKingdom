using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class InventoryPageViewModel : BaseViewModel
    {
        public InventoryPageViewModel(INavigation nav)
            : base(nav)
        {
            Title = "Inventory";

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.InventoryUpdated, (sender) =>
            {
                SetInventories();
                //NotifyPropertyChanged(nameof(Soul));
            });
        }

        private List<ArmorInventorySlot> _armors;
        public List<ArmorInventorySlot> Armors => _armors;

        private ArmorInventorySlot _selectedArmorSlot;
        public ArmorInventorySlot SelectedArmorSlot
        {
            get { return _selectedArmorSlot; }
            set
            {
                _selectedArmorSlot = value;
                _armorSlotIsSelected = _selectedArmorSlot != null;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ArmorSlotIsSelected));
                NotifyPropertyChanged(nameof(ArmorSlotEquipAction));
            }
        }

        private bool _armorSlotIsSelected;
        public bool ArmorSlotIsSelected => _armorSlotIsSelected;

        public ICommand ArmorSlotEquipAction
        {
            get { return (_selectedArmorSlot != null ? _selectedArmorSlot.EquipAction : null); }
        }

        private List<ConsumableInventorySlot> _consumables;
        public List<ConsumableInventorySlot> Consumables => _consumables;

        private ConsumableInventorySlot _selectedConsumableSlot;
        public ConsumableInventorySlot SelectedConsumableSlot
        {
            get { return _selectedConsumableSlot; }
            set
            {
                _selectedConsumableSlot = value;
                _consumableSlotIsSelected = _selectedConsumableSlot != null;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ConsumableSlotIsSelected));
            }
        }

        private bool _consumableSlotIsSelected;
        public bool ConsumableSlotIsSelected => _consumableSlotIsSelected;

        private void SetInventories()
        {
            _armors = new List<ArmorInventorySlot>();
            SelectedArmorSlot = null;
            _consumables = new List<ConsumableInventorySlot>();
            SelectedConsumableSlot = null;

            foreach (var slot in DatasManager.Instance.Soul.Inventory)
            {
                switch (slot.Type)
                {
                    case DataModels.Items.ItemType.Armor:
                        {
                            _armors.Add(new ArmorInventorySlot(
                                DatasManager.Instance.Armors?.FirstOrDefault(a => a.Armor.Id.Equals(slot.ItemId)),
                                slot.Id
                            ));
                        }
                        break;
                    case DataModels.Items.ItemType.Consumable:
                        {
                            _consumables.Add(new ConsumableInventorySlot(
                                DatasManager.Instance.Consumables?.FirstOrDefault(a => a.Consumable.Id.Equals(slot.ItemId)),
                                slot.Quantity
                            ));
                        }
                        break;
                }
            }

            NotifyPropertyChanged(nameof(Armors));
            NotifyPropertyChanged(nameof(Consumables));
        }
    }
}
