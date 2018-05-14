using System;
using System.Collections.Generic;
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
                SetInventories();
                //NotifyPropertyChanged(nameof(Soul));
            });

            SetInventories();
        }

        private List<DataModels.InventorySlot> _armors;
        public List<DataModels.InventorySlot> Armors => _armors;

        private DataModels.Items.Armor _selectedArmor;
        public DataModels.Items.Armor SelectedArmor
        {
            get { return _selectedArmor; }
            set
            {
                _selectedArmor = value;
                NotifyPropertyChanged();
            }
        }

        private DataModels.InventorySlot _selectedArmorSlot;
        public DataModels.InventorySlot SelectedArmorSlot
        {
            get { return _selectedArmorSlot; }
            set
            {
                _selectedArmorSlot = value;
                _armorSlotIsSelected = _selectedArmorSlot != null;
                _armorSlotEquipAction?.ChangeCanExecute();

                if (_selectedArmorSlot != null)
                    SelectedArmor = _selectedArmorSlot.Item as DataModels.Items.Armor;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ArmorSlotIsSelected));
                NotifyPropertyChanged(nameof(ArmorSlotEquipAction));
            }
        }

        private bool _armorSlotIsSelected;
        public bool ArmorSlotIsSelected => _armorSlotIsSelected;

        private Command _armorSlotEquipAction;
        public ICommand ArmorSlotEquipAction =>
            _armorSlotEquipAction ?? (_armorSlotEquipAction = new Command(() =>
            {

            }, () => { return _armorSlotIsSelected && _selectedArmorSlot.Item.UseLevelRequired < +DatasManager.Instance.Soul.Level; }));

        private List<DataModels.InventorySlot> _consumables;
        public List<DataModels.InventorySlot> Consumables => _consumables;

        private DataModels.InventorySlot _selectedConsumableSlot;
        public DataModels.InventorySlot SelectedConsumableSlot
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
            _armors = new List<DataModels.InventorySlot>();
            SelectedArmorSlot = null;
            _consumables = new List<DataModels.InventorySlot>();
            SelectedConsumableSlot = null;

            foreach (var slot in DatasManager.Instance.Soul.Inventory)
            {
                switch (slot.Type)
                {
                    case DataModels.Items.ItemType.Armor:
                        {
                            _armors.Add(slot);
                        }
                        break;
                    case DataModels.Items.ItemType.Consumable:
                        {
                            _consumables.Add(slot);
                        }
                        break;
                }
            }

            NotifyPropertyChanged(nameof(Armors));
            NotifyPropertyChanged(nameof(Consumables));
        }
    }
}
