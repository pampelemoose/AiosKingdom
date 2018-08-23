using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Player
{
    public class BuyMarketItemCommand : ACommand
    {
        public BuyMarketItemCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(ret.ClientId);
            var marketSlotId = Guid.Parse(_args.Args[0]);
            var quantity = int.Parse(_args.Args[1]);
            var isBits = bool.Parse(_args.Args[2]);

            var marketItem = DataRepositories.MarketRepository.GetById(marketSlotId);

            if (marketItem == null || (marketItem != null && (!IsQuantityEnought(marketItem.Quantity, quantity) || !IsCurrencyAvailable(soul, marketItem, quantity, isBits))))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.BuyMarketItem,
                    Success = false,
                    Json = !IsCurrencyAvailable(soul, marketItem, quantity, isBits) ? $"Not enought {(isBits ? "bits" : "shards")}" : "Item sold or not available anymore."
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
                case DataModels.Items.ItemType.Armor:
                case DataModels.Items.ItemType.Bag:
                case DataModels.Items.ItemType.Weapon:
                case DataModels.Items.ItemType.Jewelry:
                    {
                        soul.Inventory.Add(new DataModels.InventorySlot
                        {
                            ItemId = marketItem.ItemId,
                            Type = marketItem.Type,
                            Quantity = quantity,
                            SoulId = soul.Id,
                            LootedAt = DateTime.Now
                        });
                        DataRepositories.SoulRepository.Update(soul);
                    }
                    break;
                case DataModels.Items.ItemType.Consumable:
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
                            soul.Inventory.Add(new DataModels.InventorySlot
                            {
                                ItemId = marketItem.ItemId,
                                Type = marketItem.Type,
                                Quantity = quantity,
                                SoulId = soul.Id,
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

            SoulManager.Instance.UpdateSoul(ret.ClientId, soul);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.BuyMarketItem,
                Success = true,
                Json = $"Buy successful. Please check your inventory."
            };
            ret.Succeeded = true;
            return ret;
        }

        private bool IsQuantityEnought(int marketQuantity, int quantity)
        {
            return marketQuantity < 0 ? true : marketQuantity <= quantity;
        }

        private bool IsCurrencyAvailable(DataModels.Soul datas, DataModels.MarketSlot slot, int quantity, bool isBits)
        {
            return isBits ? datas.Bits >= (slot.BitPrice * quantity) : datas.Shards >= (slot.ShardPrice * quantity);
        }
    }
}
