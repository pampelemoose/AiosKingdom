using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// TODO : Heavy changes, to test
/// </summary>
namespace Server.GameServer.Commands.Player
{
    public class MarketPlaceOrderCommand : ACommand
    {
        public MarketPlaceOrderCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var currency = SoulManager.Instance.GetCurrencies(ret.ClientId);
            var marketSlotId = Guid.Parse(_args.Args[0]);
            var quantity = int.Parse(_args.Args[1]);
            var price = int.Parse(_args.Args[2]);
            var isBits = bool.Parse(_args.Args[3]);

            if (!Market.Instance.CanBuy(marketSlotId, quantity, price, isBits))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.Market_PlaceOrder,
                    Success = false,
                    Json = "Could not place order, Item may be sold or not available anymore, price is too low or currency not available."
                };
                ret.Succeeded = true;
                return ret;
            }

            var order = new Market.Order
            {
                Buyer = ret.ClientId,
                ItemId = marketSlotId,
                Quantity = quantity,
                Value = price,
                isBits = isBits
            };

            if (isBits)
                currency.Bits -= price;
            else
                currency.Shards -= price;

            if (!Market.Instance.PlaceOrder(order))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.Market_PlaceOrder,
                    Success = false,
                    Json = "Could not place order, Item may be sold or not available anymore, price is too low or currency not available."
                };
                ret.Succeeded = true;
                return ret;
            }

            /*
            var marketItem = DataRepositories.MarketRepository.GetById(marketSlotId);

            if (marketItem == null || (marketItem != null && (!IsQuantityEnought(marketItem.Quantity, quantity) || !IsCurrencyAvailable(currency, marketItem, quantity, isBits))))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.Market_PlaceOrder,
                    Success = false,
                    Json = !IsCurrencyAvailable(currency, marketItem, quantity, isBits) ? $"Not enought {(isBits ? "bits" : "shards")}" : "Item sold or not available anymore."
                };
                ret.Succeeded = true;
                return ret;
            }

            if (isBits)
                soul.Bits -= (marketItem.BitPrice * quantity);
            else
                soul.Shards -= (marketItem.ShardPrice * quantity);

            switch (marketItem.Type)
            {
                case Network.Items.ItemType.Armor:
                case Network.Items.ItemType.Bag:
                case Network.Items.ItemType.Weapon:
                case Network.Items.ItemType.Jewelry:
                    {
                        soul.Inventory.Add(new Network.InventorySlot
                        {
                            ItemId = marketItem.ItemId,
                            Type = marketItem.Type,
                            Quantity = quantity,
                            LootedAt = DateTime.Now
                        });
                        DataRepositories.SoulRepository.Update(soul);
                    }
                    break;
                case Network.Items.ItemType.Consumable:
                    {
                        var exists = soul.Inventory.FirstOrDefault(i => i.ItemId.Equals(marketItem.ItemId));
                        if (exists != null)
                        {
                            soul.Inventory.Remove(exists);
                            exists.Quantity += quantity;
                            soul.Inventory.Add(exists);
                            DataRepositories.SoulRepository.Update(soul);
                        }
                        else
                        {
                            soul.Inventory.Add(new Network.InventorySlot
                            {
                                ItemId = marketItem.ItemId,
                                Type = marketItem.Type,
                                Quantity = quantity,
                                LootedAt = DateTime.Now
                            });
                            DataRepositories.SoulRepository.Update(soul);
                        }
                    }
                    break;
            }

            if (marketItem.Quantity > 0)
            {
                marketItem.Quantity -= quantity;

                if (marketItem.Quantity == 0)
                {
                    DataRepositories.MarketRepository.DeleteById(marketItem.Id);
                }
                else
                {
                    DataRepositories.MarketRepository.Update(marketItem);
                }
            }

            SoulManager.Instance.UpdateSoul(ret.ClientId, soul);*/

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.Market_PlaceOrder,
                Success = true,
                Json = $"Order placed, will be processed soon."
            };
            ret.Succeeded = true;
            return ret;
        }

        private bool IsQuantityEnought(int marketQuantity, int quantity)
        {
            return marketQuantity < 0 ? true : marketQuantity <= quantity;
        }

        private bool IsCurrencyAvailable(Network.Currencies currency, Network.MarketSlot slot, int quantity, bool isBits)
        {
            return isBits ? currency.Bits >= (slot.BitPrice * quantity) : currency.Shards >= (slot.ShardPrice * quantity);
        }
    }
}
