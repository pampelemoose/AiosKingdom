using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class MarketPageViewModel : BaseViewModel
    {
        public MarketPageViewModel(INavigation nav) 
            : base(nav)
        {
            _filter = Network.Items.ItemType.Armor;

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.MarketUpdated, (sender) =>
            {
                SetItems();
                IsBusy = false;
            });

            NetworkManager.Instance.AskMarketItems();
        }

        ~MarketPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.MarketUpdated);
        }

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

        private Network.Items.ItemType _filter;
        private Command _setFilterAction;
        public ICommand SetFilterAction =>
        _setFilterAction ?? (_setFilterAction = new Command((arg) =>
        {
            string filter = (string)arg;
            Network.Items.ItemType newFilter;

            IsArmorPanelActive = false;
            IsWeaponPanelActive = false;
            IsBagPanelActive = false;
            IsConsumablePanelActive = false;

            switch (filter)
            {
                case "Armors":
                    newFilter = Network.Items.ItemType.Armor;
                    IsArmorPanelActive = true;
                    break;
                case "Consumables":
                    newFilter = Network.Items.ItemType.Consumable;
                    IsConsumablePanelActive = true;
                    break;
                case "Weapons":
                    newFilter = Network.Items.ItemType.Weapon;
                    IsWeaponPanelActive = true;
                    break;
                case "Bags":
                    newFilter = Network.Items.ItemType.Bag;
                    IsBagPanelActive = true;
                    break;
                default:
                    return;
            }

            if (newFilter != _filter)
            {
                _filter = newFilter;
                SetItems();
            }
        }));

        private List<Models.MarketItemModel> _items;
        public List<Models.MarketItemModel> Items => _items;

        private bool _isItemSelected;
        public bool IsItemSelected => _isItemSelected;

        private Models.MarketItemModel _selectedItem;
        public Models.MarketItemModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                _isItemSelected = _selectedItem != null;
                _buyItemAction?.ChangeCanExecute();
                NotifyPropertyChanged(nameof(IsItemSelected));
                NotifyPropertyChanged();
            }
        }

        private Command _buyItemAction;
        public ICommand BuyItemAction =>
        _buyItemAction ?? (_buyItemAction = new Command(() =>
        {
            _navigation.PushModalAsync(new Views.BuyItemPage(new MarketBuyItemPageViewModel(_navigation, _selectedItem)));
        }, () =>
        {
            return DatasManager.Instance.Currencies?.Shards >= _selectedItem?.Slot.ShardPrice
                       || DatasManager.Instance.Currencies?.Bits >= _selectedItem?.Slot.BitPrice;
        }));

        private void SetItems()
        {
            _selectedItem = null;
            _isItemSelected = false;
            _items = new List<Models.MarketItemModel>();

            if (DatasManager.Instance.MarketItems == null) return;

            foreach (var slot in DatasManager.Instance.MarketItems?.Where(i => i.Type == _filter).ToList())
            {
                Network.Items.AItem itm = null;

                switch (slot.Type)
                {
                    case Network.Items.ItemType.Armor:
                        itm = DatasManager.Instance.Armors.FirstOrDefault(i => i.Id.Equals(slot.ItemId));
                        break;
                    case Network.Items.ItemType.Consumable:
                        itm = DatasManager.Instance.Consumables.FirstOrDefault(i => i.Id.Equals(slot.ItemId));
                        break;
                    case Network.Items.ItemType.Bag:
                        itm = DatasManager.Instance.Bags.FirstOrDefault(i => i.Id.Equals(slot.ItemId));
                        break;
                    case Network.Items.ItemType.Weapon:
                        itm = DatasManager.Instance.Weapons.FirstOrDefault(i => i.Id.Equals(slot.ItemId));
                        break;
                }

                _items.Add(new Models.MarketItemModel
                {
                    Slot = slot,
                    Item = itm
                });
            }

            NotifyPropertyChanged(nameof(SelectedItem));
            NotifyPropertyChanged(nameof(IsItemSelected));
            NotifyPropertyChanged(nameof(Items));
        }
    }
}
