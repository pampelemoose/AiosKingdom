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
        public MarketPageViewModel() 
            : base(null)
        {
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.MarketUpdated, (sender) =>
            {
                SetItems();
                LoadingScreenManager.Instance.CloseLoadingScreen();
            });

            NetworkManager.Instance.AskMarketItems();
        }

        private DataModels.Items.ItemType _filter;
        public DataModels.Items.ItemType Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                SetItems();
                NotifyPropertyChanged();
            }
        }

        public List<DataModels.Items.ItemType> FilterList => Enum.GetValues(typeof(DataModels.Items.ItemType)).Cast<DataModels.Items.ItemType>().ToList();

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
            LoadingScreenManager.Instance.OpenLoadingScreen($"Buying {_selectedItem.Item.Name}, please wait...");
            NetworkManager.Instance.BuyMarketItem(_selectedItem.Slot.Id);
        }, () =>
        {
            return DatasManager.Instance.Soul?.Shards >= _selectedItem?.Slot.ShardPrice
                       && DatasManager.Instance.Soul?.Bits >= _selectedItem?.Slot.BitPrice;
        }));

        private void SetItems()
        {
            _selectedItem = null;
            _isItemSelected = false;
            _items = new List<Models.MarketItemModel>();

            if (DatasManager.Instance.MarketItems == null) return;

            foreach (var slot in DatasManager.Instance.MarketItems?.Where(i => i.Type == _filter).ToList())
            {
                DataModels.Items.AItem itm = null;

                switch (slot.Type)
                {
                    case DataModels.Items.ItemType.Armor:
                        itm = DatasManager.Instance.Armors.FirstOrDefault(i => i.ItemId.Equals(slot.ItemId));
                        break;
                    case DataModels.Items.ItemType.Consumable:
                        itm = DatasManager.Instance.Consumables.FirstOrDefault(i => i.ItemId.Equals(slot.ItemId));
                        break;
                    case DataModels.Items.ItemType.Bag:
                        itm = DatasManager.Instance.Bags.FirstOrDefault(i => i.ItemId.Equals(slot.ItemId));
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
