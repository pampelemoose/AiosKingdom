using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class MarketBuyItemPageViewModel : ABuyItemViewModel
    {
        public MarketBuyItemPageViewModel(INavigation nav, Models.MarketItemModel item, int quantity = 1)
            : base(nav, quantity)
        {
            _item = item;
        }

        private Models.MarketItemModel _item;
        public Models.MarketItemModel Item => _item;

        public override bool IsConsumable => _item.Item.Type == Network.Items.ItemType.Consumable;

        public override int Shards => _item.Slot.ShardPrice * _quantity;
        public override int Bits => _item.Slot.BitPrice * _quantity;

        protected override int GetMaxQuantityForShards()
        {
            var shards = DatasManager.Instance.Currencies.Shards;
            var max = shards / _item.Slot.ShardPrice;
            return _item.Slot.Quantity > 0 ? (max > _item.Slot.Quantity ? _item.Slot.Quantity : max) : max;
        }

        protected override int GetMaxQuantityForBits()
        {
            var bits = DatasManager.Instance.Currencies.Bits;
            var max = bits / _item.Slot.BitPrice;
            return _item.Slot.Quantity > 0 ? (max > _item.Slot.Quantity ? _item.Slot.Quantity : max) : max;
        }

        protected override void ExecuteBuyAction()
        {
            IsBusy = true;
            NetworkManager.Instance.BuyMarketItem(
                _item.Slot.Id, 
                _quantity, 
                (IsBitSelected ? _item.Slot.BitPrice : _item.Slot.ShardPrice) * _quantity,
                IsBitSelected);
        }
    }
}
