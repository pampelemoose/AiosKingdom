using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels.Dungeon
{
    public class ShopBuyItemPageViewModel : ABuyItemViewModel
    {
        public ShopBuyItemPageViewModel(INavigation nav, KeyValuePair<Guid, Network.AdventureState.ShopState> shopItem, int quantity = 1) 
            : base(nav, quantity)
        {
            _item = shopItem;
        }

        private KeyValuePair<Guid, Network.AdventureState.ShopState> _item;
        public KeyValuePair<Guid, Network.AdventureState.ShopState> Item => _item;

        public override bool IsConsumable => (DataModels.Items.ItemType)Enum.Parse(typeof(DataModels.Items.ItemType), _item.Value.Type) == DataModels.Items.ItemType.Consumable;

        public override int Shards => _item.Value.ShardPrice * _quantity;
        public override int Bits => -1;

        protected override void ExecuteBuyAction()
        {
            IsBusy = true;
            NetworkManager.Instance.BuyShopItem(_item.Key, _quantity);
        }

        protected override int GetMaxQuantityForBits()
        {
            return 0;
        }

        protected override int GetMaxQuantityForShards()
        {
            var shards = DatasManager.Instance.Currencies.Shards;
            var max = shards / _item.Value.ShardPrice;
            return _item.Value.Quantity > 0 ? (max > _item.Value.Quantity ? _item.Value.Quantity : max) : max;
        }
    }
}
