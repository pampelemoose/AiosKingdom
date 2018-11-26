using Newtonsoft.Json;
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
            public Guid ClientId { get; set; }
            public Guid Buyer { get; set; }
            public Guid MarketSlotId { get; set; }
            public int Price { get; set; }
        }

        public List<Network.MarketSlot> Items { get; private set; }
        public List<Network.MarketSlot> Specials { get; private set; }

        private Object _orderListLock = new Object();
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
            LoadSpecialMarket();

            Console.WriteLine($"Market Initialized. Ready to take Orders");
            Log.Instance.Write(Log.Level.Infos, $"Market Initialized. Ready to take Orders");
        }

        public bool CanBuy(Guid itemId, Network.Currencies currency)
        {
            var item = Items.FirstOrDefault(i => i.Id.Equals(itemId));
            if (item != null)
            {
                if (currency.Shards >= item.Price)
                    return true;
                return false;
            }
            var special = Specials.FirstOrDefault(i => i.Id.Equals(itemId));
            if (special != null)
            {
                if (currency.Bits >= special.Price)
                    return true;
            }
            return false;
        }

        public bool PlaceOrder(Order order, Network.Currencies currency, Guid clientId)
        {
            lock (_orderListLock)
            {
                bool canAdd = false;
                var item = Items.FirstOrDefault(i => i.Id.Equals(order.MarketSlotId));
                if (item != null)
                {
                    order.Price = item.Price;
                    currency.Shards -= order.Price;
                    canAdd = true;
                }
                var special = Specials.FirstOrDefault(i => i.Id.Equals(order.MarketSlotId));
                if (special != null)
                {
                    order.Price = special.Price;
                    currency.Bits -= order.Price;
                    canAdd = true;
                }

                if (canAdd)
                {
                    SoulManager.Instance.UpdateCurrencies(clientId, currency);
                    _orders.Add(order);
                    return true;
                }
                Log.Instance.Write(Log.Level.Infos, $"Order placed for {order.MarketSlotId}, by {order.Buyer}");
            }

            return false;
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
            lock (_orderListLock)
            {
                var order = _orders.First();
                _orders.Remove(order);

                var history = new DataModels.MarketHistory
                {
                    Type = DataModels.TransactionType.Buy,
                    MarketId = order.MarketSlotId,
                    BuyerId = order.Buyer,
                    Price = order.Price,
                    BoughtAt = DateTime.Now
                };

                int quantity = 0;
                bool isSpecial = false;
                bool processed = false;
                var inventory = SoulManager.Instance.GetInventory(order.ClientId);
                var item = Items.FirstOrDefault(i => i.Id.Equals(order.MarketSlotId));
                if (item != null)
                {
                    InventoryManager.AddItemToInventory(order.ClientId, item.ItemId, item.Quantity);
                    quantity = item.Quantity;
                    Items.Remove(item);
                    history.ItemId = item.ItemId;
                    history.Quantity = item.Quantity;
                    history.SellerId = item.SellerId;
                    processed = true;
                }
                var special = Specials.FirstOrDefault(i => i.Id.Equals(order.MarketSlotId));
                if (special != null)
                {
                    InventoryManager.AddItemToInventory(order.ClientId, special.ItemId, special.Quantity);
                    quantity = special.Quantity;
                    isSpecial = true;
                    history.ItemId = special.ItemId;
                    history.Quantity = special.Quantity;
                    history.ToServer = true;
                    processed = true;
                }

                if (processed)
                {
                    DataRepositories.MarketHistoryRepository.Create(history);

                    return new Commands.CommandResult
                    {
                        ClientId = order.ClientId,
                        ClientResponse = new Network.Message
                        {
                            Code = Network.CommandCodes.Player.Market_OrderProcessed,
                            Success = true,
                            Json = JsonConvert.SerializeObject(new Network.MarketOrderProcessed
                            {
                                ItemId = order.MarketSlotId,
                                Quantity = quantity
                            })
                        },
                        Succeeded = true
                    };
                }

                var currency = SoulManager.Instance.GetCurrencies(order.MarketSlotId);
                if (isSpecial)
                {
                    currency.Bits += order.Price;
                }
                else
                {
                    currency.Shards += order.Price;
                }
                SoulManager.Instance.UpdateCurrencies(order.ClientId, currency);

                return new Commands.CommandResult
                {
                    ClientId = order.ClientId,
                    ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Player.Market_OrderProcessed,
                        Success = false,
                        Json = "Could not process order."
                    },
                    Succeeded = true
                };
            }
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
                    IsSpecial = marketSlot.IsSpecial,
                    Price = marketSlot.Price,
                    Quantity = marketSlot.Quantity
                };

                marketList.Add(slot);
            }

            Items = marketList;
        }

        private void LoadSpecialMarket()
        {
            var market = DataRepositories.MarketRepository.GetAllForServer(_config.Id, true);
            var marketList = new List<Network.MarketSlot>();

            foreach (var marketSlot in market)
            {
                var slot = new Network.MarketSlot
                {
                    Id = marketSlot.Id,
                    ItemId = marketSlot.ItemId,
                    SellerId = marketSlot.SellerId,
                    IsSpecial = marketSlot.IsSpecial,
                    Price = marketSlot.Price,
                    Quantity = marketSlot.Quantity
                };

                marketList.Add(slot);
            }

            Specials = marketList;
        }
    }
}
