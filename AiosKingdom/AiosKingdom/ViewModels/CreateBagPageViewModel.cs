using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class CreateBagPageViewModel : BaseViewModel
    {
        private DataModels.Dungeons.Dungeon _dungeon;

        public CreateBagPageViewModel(DataModels.Dungeons.Dungeon dungeon)
            : base(null)
        {
            Title = "Bag Creation";

            _dungeon = dungeon;

            _items = new List<Models.InventoryItemModel<DataModels.Items.Consumable>>();

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.InventoryUpdated, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetInventory();
                    IsBusy = false;
                });
            });

            NetworkManager.Instance.AskInventory();

            IsInfoVisible = true;
        }

        private void SetInventory()
        {
            _inventory = new List<Models.InventoryItemModel<DataModels.Items.Consumable>>();
            foreach (var slot in DatasManager.Instance.Inventory.OrderBy(i => i.LootedAt).ToList())
            {
                switch (slot.Type)
                {
                    case DataModels.Items.ItemType.Consumable:
                        {
                            _inventory.Add(new Models.InventoryItemModel<DataModels.Items.Consumable>
                            {
                                Slot = slot,
                                Item = DatasManager.Instance.Consumables.FirstOrDefault(a => a.ItemId.Equals(slot.ItemId))
                            });
                        }
                        break;
                }
            }
            NotifyPropertyChanged(nameof(Inventory));
        }

        public int Slots
        {
            get
            {
                return DatasManager.Instance.Datas.BagSpace - _items.Select(i => i.Slot.Quantity).Sum();
            }
        }

        private List<Models.InventoryItemModel<DataModels.Items.Consumable>> _items;
        public List<Models.InventoryItemModel<DataModels.Items.Consumable>> Items => _items;

        private Models.InventoryItemModel<DataModels.Items.Consumable> _selectedItem;
        public Models.InventoryItemModel<DataModels.Items.Consumable> SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                _removeItemAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        private ICommand _addItemAction;
        public ICommand AddItemAction =>
            _addItemAction ?? (_addItemAction = new Command(() =>
            {
                ShowAddPanel = true;
            }));

        private bool _showAddPanel;
        public bool ShowAddPanel
        {
            get { return _showAddPanel; }
            set
            {
                _showAddPanel = value;
                NotifyPropertyChanged();
            }
        }

        private int _quantity = 1;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                _addAction?.ChangeCanExecute();
                _subQuantityAction?.ChangeCanExecute();
                _addQuantityAction?.ChangeCanExecute();
                _addAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        private Command _subQuantityAction;
        public ICommand SubQuantityAction =>
            _subQuantityAction ?? (_subQuantityAction = new Command(() =>
            {
                --Quantity;
            }, () => { return Quantity > 1; }));

        private Command _addQuantityAction;
        public ICommand AddQuantityAction =>
            _addQuantityAction ?? (_addQuantityAction = new Command(() =>
            {
                ++Quantity;
            }, () =>
            {
                return (Quantity < _selectedInventory?.Slot.Quantity && Quantity < (DatasManager.Instance.Datas.BagSpace - Items.Select(i => i.Slot.Quantity).Sum()));
            }));

        private List<Models.InventoryItemModel<DataModels.Items.Consumable>> _inventory;
        public List<Models.InventoryItemModel<DataModels.Items.Consumable>> Inventory => _inventory;

        private Models.InventoryItemModel<DataModels.Items.Consumable> _selectedInventory;
        public Models.InventoryItemModel<DataModels.Items.Consumable> SelectedInventory
        {
            get { return _selectedInventory; }
            set
            {
                _quantity = 1;
                _selectedInventory = value;
                _subQuantityAction?.ChangeCanExecute();
                _addQuantityAction?.ChangeCanExecute();
                _addAction?.ChangeCanExecute();
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Quantity));
            }
        }

        private Command _addAction;
        public ICommand AddAction =>
            _addAction ?? (_addAction = new Command(() =>
            {
                ShowAddPanel = false;

                _inventory.Remove(_selectedInventory);

                if (_selectedInventory.Slot.Quantity - _quantity >= 1)
                {
                    _selectedInventory.Slot.Quantity -= _quantity;
                    _inventory.Add(_selectedInventory);
                }

                _items = new List<Models.InventoryItemModel<DataModels.Items.Consumable>>(_items);
                var inBag = _items.FirstOrDefault(i => i.Slot.Id.Equals(_selectedInventory.Slot.Id));

                if (inBag != null)
                {
                    _items.Remove(inBag);
                    inBag.Slot.Quantity += _quantity;
                    _items.Add(inBag);
                }
                else
                {
                    _items.Add(new Models.InventoryItemModel<DataModels.Items.Consumable>
                    {
                        Slot = new DataModels.InventorySlot
                        {
                            Id = _selectedInventory.Slot.Id,
                            ItemId = _selectedInventory.Slot.ItemId,
                            Type = _selectedInventory.Slot.Type,
                            SoulId = _selectedInventory.Slot.SoulId,
                            Quantity = _quantity,
                            LootedAt = _selectedInventory.Slot.LootedAt
                        },
                        Item = _selectedInventory.Item
                    });
                }

                _inventory = new List<Models.InventoryItemModel<DataModels.Items.Consumable>>(_inventory);
                _selectedInventory = null;
                NotifyPropertyChanged(nameof(SelectedInventory));
                NotifyPropertyChanged(nameof(Items));
                NotifyPropertyChanged(nameof(Inventory));
                NotifyPropertyChanged(nameof(Slots));
            }, () => { return SelectedInventory != null && Quantity > 0 && Quantity <= (DatasManager.Instance.Datas.BagSpace - Items.Select(i => i.Slot.Quantity).Sum()); }));

        private Command _removeItemAction;
        public ICommand RemoveItemAction =>
            _removeItemAction ?? (_removeItemAction = new Command(() =>
            {
                _items.Remove(_selectedItem);
                _selectedItem.Slot.Quantity--;

                if (_selectedItem.Slot.Quantity >= 1)
                {
                    _items.Add(_selectedItem);
                }

                _inventory = new List<Models.InventoryItemModel<DataModels.Items.Consumable>>(_inventory);
                var inInventory = _inventory.FirstOrDefault(i => i.Slot.Id.Equals(_selectedItem.Slot.Id));

                if (inInventory != null)
                {
                    _inventory.Remove(inInventory);
                    inInventory.Slot.Quantity++;
                    _inventory.Add(inInventory);
                }
                else
                {
                    _inventory.Add(new Models.InventoryItemModel<DataModels.Items.Consumable>
                    {
                        Slot = new DataModels.InventorySlot
                        {
                            Id = _selectedInventory.Slot.Id,
                            ItemId = _selectedInventory.Slot.ItemId,
                            Type = _selectedInventory.Slot.Type,
                            SoulId = _selectedInventory.Slot.SoulId,
                            Quantity = 1,
                            LootedAt = _selectedInventory.Slot.LootedAt
                        },
                        Item = _selectedInventory.Item
                    });
                }

                if (_selectedItem.Slot.Quantity <= 0)
                {
                    SelectedItem = null;
                }

                _items = new List<Models.InventoryItemModel<DataModels.Items.Consumable>>(_items);
                NotifyPropertyChanged(nameof(Items));
                NotifyPropertyChanged(nameof(Inventory));
                NotifyPropertyChanged(nameof(Slots));
            }, () => { return SelectedItem != null; }));

        private Command _closeAction;
        public ICommand CloseAction =>
            _closeAction ?? (_closeAction = new Command(() =>
            {
                ShowAddPanel = false;
            }, () => { return !IsBusy; }));

        private Command _backAction;
        public ICommand BackAction =>
            _backAction ?? (_backAction = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ScreenManager.Instance.ChangePage(new Views.HomePage());
                });
            }));

        private ICommand _enterDungeonAction;
        public ICommand EnterDungeonAction =>
            _enterDungeonAction ?? (_enterDungeonAction = new Command(() =>
            {
                IsBusy = true;

                List<Network.AdventureState.BagItem> bagItems = new List<Network.AdventureState.BagItem>();

                foreach (var item in _items)
                {
                    bagItems.Add(new Network.AdventureState.BagItem
                    {
                        InventoryId = item.Slot.Id,
                        ItemId = item.Slot.ItemId,
                        Type = item.Slot.Type.ToString(),
                        Quantity = item.Slot.Quantity
                    });
                }

                NetworkManager.Instance.EnterDungeon(_dungeon.DungeonId, bagItems);
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

        private ICommand _closeInfoAction;
        public ICommand CloseInfoAction =>
            _closeInfoAction ?? (_closeInfoAction = new Command(() =>
            {
                IsInfoVisible = false;
            }));
    }
}
