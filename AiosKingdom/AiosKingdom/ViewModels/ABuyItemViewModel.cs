using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public abstract class ABuyItemViewModel : BaseViewModel
    {
        public struct BuyableItem
        {
            public DataModels.Items.ItemType Type { get; set; }

        }

        public ABuyItemViewModel(INavigation nav, int quantity = 1) 
            : base(nav)
        {
            _quantity = quantity;
        }

        protected int _quantity;
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

        public abstract bool IsConsumable { get; }

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

        public abstract int Shards { get; }
        public abstract int Bits { get; }

        public bool ShowBits => Bits > 0;

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

        protected abstract int GetMaxQuantityForShards();
        protected abstract int GetMaxQuantityForBits();

        private ICommand _selectShardAction;
        public ICommand SelectShardAction =>
            _selectShardAction ?? (_selectShardAction = new Command(() =>
            {
                IsBitSelected = false;
                IsShardSelected = true;
            }, () => { return GetMaxQuantityForShards() > 0; }));

        private ICommand _selectBitAction;
        public ICommand SelectBitAction =>
            _selectBitAction ?? (_selectBitAction = new Command(() =>
            {
                IsBitSelected = true;
                IsShardSelected = false;
            }, () => { return GetMaxQuantityForBits() > 0; }));

        private ICommand _closeAction;
        public ICommand CloseAction =>
            _closeAction ?? (_closeAction = new Command(() =>
            {
                _navigation.PopModalAsync();
            }));

        private ICommand _buyAction;
        public ICommand BuyAction =>
            _buyAction ?? (_buyAction = new Command(() =>
            {
                ExecuteBuyAction();
            }));

        protected abstract void ExecuteBuyAction();
    }
}
