using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public class Market
    {
        private static Market _instance;
        public static Market Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Market();

                return _instance;
            }
        }

        public struct Order
        {
            public Guid Buyer { get; set; }
            public Guid ItemId { get; set; }
            public int Quantity { get; set; }
            public int Value { get; set; }
            public bool isBits { get; set; }
        }

        public List<Network.MarketSlot> Items { get; private set; }

        private object _orderListLock;
        private List<Order> _orders;

        private Market()
        {

        }

        private DataModels.Config _config;

        public void Initialize(DataModels.Config config)
        {
            _config = config;

            _orders = new List<Order>();

            LoadMarket();

            Console.WriteLine($"Market Initialized. Ready to take Orders");
            Log.Instance.Write(Log.Level.Infos, $"Market Initialized. Ready to take Orders");
        }

        public bool CanBuy(Guid itemId, int quantity, int price, bool isBits = true)
        {
            return false;
        }

        public bool PlaceOrder(Order order)
        {
            lock (_orderListLock)
            {
                _orders.Add(order);

                Log.Instance.Write(Log.Level.Infos, $"Order placed for {order.ItemId}, {order.Quantity} for {order.Value}, by {order.Buyer}");
            }

            return true;
        }

        public int OrderCount
        {
            get
            {
                lock (_orderListLock)
                {
                    return _orders.Count;
                }
            }
        }

        public Commands.CommandResult ProcessOrder()
        {
            var order = _orders.Take(1);

            return new Commands.CommandResult
            {
                Succeeded = false
            };
        }

        private void LoadMarket()
        {
            var market = DataRepositories.MarketRepository.GetAllForServer(_config.Id);
            var marketList = new List<Network.MarketSlot>();

            foreach (var marketSlot in market)
            {
                var slot = new Network.MarketSlot
                {
                    Id = marketSlot.Id,
                    ItemId = marketSlot.ItemId,
                    SellerId = marketSlot.SellerId,
                    BitPrice = marketSlot.BitPrice,
                    ShardPrice = marketSlot.ShardPrice,
                    Quantity = marketSlot.Quantity
                };

                switch (marketSlot.Type)
                {
                    case DataModels.Items.ItemType.Armor:
                        slot.Type = Network.Items.ItemType.Armor;
                        break;
                    case DataModels.Items.ItemType.Bag:
                        slot.Type = Network.Items.ItemType.Bag;
                        break;
                    case DataModels.Items.ItemType.Consumable:
                        slot.Type = Network.Items.ItemType.Consumable;
                        break;
                    case DataModels.Items.ItemType.Jewelry:
                        slot.Type = Network.Items.ItemType.Jewelry;
                        break;
                    case DataModels.Items.ItemType.Weapon:
                        slot.Type = Network.Items.ItemType.Weapon;
                        break;
                }

                marketList.Add(slot);
            }

            Items = marketList;
        }
    }
}
