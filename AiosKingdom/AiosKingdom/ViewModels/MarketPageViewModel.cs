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
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulUpdated, (sender) =>
            {
                SetItems();
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

        private List<DataModels.MarketSlot> _items;
        public List<DataModels.MarketSlot> Items => _items;

        private bool _isItemSelected;
        public bool IsItemSelected => _isItemSelected;

        private DataModels.MarketSlot _selectedItem;
        public DataModels.MarketSlot SelectedItem
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
            NetworkManager.Instance.BuyMarketItem(_selectedItem.Id);
        }, () =>
        {
            return DatasManager.Instance.Soul?.Shards >= _selectedItem?.ShardPrice
                       && DatasManager.Instance.Soul?.Bits >= _selectedItem?.BitPrice;
        }));

        private void SetItems()
        {
            _selectedItem = null;
            _isItemSelected = false;
            _items = DatasManager.Instance.MarketItems?.Where(i => i.Type == _filter).ToList();

            NotifyPropertyChanged(nameof(SelectedItem));
            NotifyPropertyChanged(nameof(IsItemSelected));
            NotifyPropertyChanged(nameof(Items));
        }
    }
}
