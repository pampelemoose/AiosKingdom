using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class MarketBuyItemPageViewModel : BaseViewModel
    {
        public MarketBuyItemPageViewModel(INavigation nav, Models.MarketItemModel item, int quantity = 1)
            : base(nav)
        {
            _item = item;
            _quantity = quantity;
        }

        private Models.MarketItemModel _item;
        public Models.MarketItemModel Item => _item;

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                if (_quantity < 1)
                    _quantity = 1;

                _subAction?.ChangeCanExecute();
                _addAction?.ChangeCanExecute();

                NotifyPropertyChanged();

                NotifyPropertyChanged(nameof(Shards));
                NotifyPropertyChanged(nameof(Bits));
            }
        }

        public bool IsConsumable => _item.Item.Type == DataModels.Items.ItemType.Consumable;

        private bool _isShardSelected = true;
        public bool IsShardSelected
        {
            get { return _isShardSelected; }
            set
            {
                _isShardSelected = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBitSelected;
        public bool IsBitSelected
        {
            get { return _isBitSelected; }
            set
            {
                _isBitSelected = value;
                NotifyPropertyChanged();
            }
        }

        public int Shards => _item.Slot.ShardPrice * _quantity;
        public int Bits => _item.Slot.BitPrice * _quantity;

        private ICommand _minAction;
        public ICommand MinAction =>
            _minAction ?? (_minAction = new Command(() =>
            {
                Quantity = 1;
            }));

        private Command _subAction;
        public ICommand SubAction =>
            _subAction ?? (_subAction = new Command(() =>
            {
                --Quantity;
            }, () => { return Quantity > 1; }));

        private Command _addAction;
        public ICommand AddAction =>
            _addAction ?? (_addAction = new Command(() =>
            {
                ++Quantity;
            }, () =>
            {
                return (IsBitSelected ? Quantity < GetMaxQuantityForBits() : Quantity < GetMaxQuantityForShards());
            }));

        private ICommand _maxAction;
        public ICommand MaxAction =>
            _maxAction ?? (_maxAction = new Command(() =>
            {
                if (_isBitSelected)
                {
                    Quantity = GetMaxQuantityForBits();
                }
                else
                {
                    Quantity = GetMaxQuantityForShards();
                }
            }));

        private int GetMaxQuantityForShards()
        {
            var shards = DatasManager.Instance.Soul.Shards;
            var max = shards / _item.Slot.ShardPrice;
            return _item.Slot.Quantity > 0 ? (max > _item.Slot.Quantity ? _item.Slot.Quantity : max) : max;
        }

        private int GetMaxQuantityForBits()
        {
            var bits = DatasManager.Instance.Soul.Bits;
            var max = bits / _item.Slot.BitPrice;
            return _item.Slot.Quantity > 0 ? (max > _item.Slot.Quantity ? _item.Slot.Quantity : max) : max;
        }

        private ICommand _selectShardAction;
        public ICommand SelectShardAction =>
            _selectShardAction ?? (_selectShardAction = new Command(() =>
            {
                IsBitSelected = false;
                IsShardSelected = true;
            }));

        private ICommand _selectBitAction;
        public ICommand SelectBitAction =>
            _selectBitAction ?? (_selectBitAction = new Command(() =>
            {
                IsBitSelected = true;
                IsShardSelected = false;
            }));

        private ICommand _closeAction;
        public ICommand CloseAction =>
            _closeAction ?? (_closeAction = new Command(() =>
            {
                _navigation.PopAsync();
            }));

        private ICommand _buyAction;
        public ICommand BuyAction =>
            _buyAction ?? (_buyAction = new Command(() =>
            {
                _navigation.PopAsync();

                LoadingScreenManager.Instance.OpenLoadingScreen($"Buying {_item.Item.Name} * ({_quantity}) with { (IsShardSelected ? "Shards" : "Bits") }, please wait...");
                NetworkManager.Instance.BuyMarketItem(_item.Slot.Id, _quantity, IsBitSelected);
            }));
    }
}
