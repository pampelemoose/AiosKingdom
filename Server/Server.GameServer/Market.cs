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
                    SellerId = marketSlot.SellerId != null ? (Guid)marketSlot.SellerId : Guid.Empty,
                    BitPrice = marketSlot.BitPrice,
                    ShardPrice = marketSlot.ShardPrice,
                    Quantity = marketSlot.Quantity
                };

                slot.Type = ConvertItemType(marketSlot.Type);

                marketList.Add(slot);
            }

            Items = marketList;
        }

        private Network.Items.ItemType ConvertItemType(DataModels.Items.ItemType type)
        {
            switch (type)
            {
                case DataModels.Items.ItemType.Armor:
                    return Network.Items.ItemType.Armor;
                case DataModels.Items.ItemType.Axe:
                    return Network.Items.ItemType.Axe;
                case DataModels.Items.ItemType.Bag:
                    return Network.Items.ItemType.Bag;
                case DataModels.Items.ItemType.Book:
                    return Network.Items.ItemType.Book;
                case DataModels.Items.ItemType.Bow:
                    return Network.Items.ItemType.Bow;
                case DataModels.Items.ItemType.Consumable:
                    return Network.Items.ItemType.Consumable;
                case DataModels.Items.ItemType.Crossbow:
                    return Network.Items.ItemType.Crossbow;
                case DataModels.Items.ItemType.Dagger:
                    return Network.Items.ItemType.Dagger;
                case DataModels.Items.ItemType.Fist:
                    return Network.Items.ItemType.Fist;
                case DataModels.Items.ItemType.Gun:
                    return Network.Items.ItemType.Gun;
                case DataModels.Items.ItemType.Jewelry:
                    return Network.Items.ItemType.Jewelry;
                case DataModels.Items.ItemType.Junk:
                    return Network.Items.ItemType.Junk;
                case DataModels.Items.ItemType.Mace:
                    return Network.Items.ItemType.Mace;
                case DataModels.Items.ItemType.Polearm:
                    return Network.Items.ItemType.Polearm;
                case DataModels.Items.ItemType.Shield:
                    return Network.Items.ItemType.Shield;
                case DataModels.Items.ItemType.Staff:
                    return Network.Items.ItemType.Staff;
                case DataModels.Items.ItemType.Sword:
                    return Network.Items.ItemType.Sword;
                case DataModels.Items.ItemType.Wand:
                    return Network.Items.ItemType.Wand;
            }

            return Network.Items.ItemType.Whip;
        }
    }
}
