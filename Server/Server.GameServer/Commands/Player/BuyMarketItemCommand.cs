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
            var datas = SoulManager.Instance.GetSoul(ret.ClientId);
            var marketSlotId = Guid.Parse(_args.Args[0]);
            var quantity = int.Parse(_args.Args[1]);
            var isBits = bool.Parse(_args.Args[2]);

            var marketItem = DataRepositories.MarketRepository.GetById(marketSlotId);

            if (marketItem == null || (marketItem != null && marketItem.Quantity >= quantity && IsCurrencyAvailable(datas, marketItem, quantity, isBits)))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.BuyMarketItem,
                    Success = false,
                    Json = "Item sold or not available anymore."
                };
                ret.Succeeded = true;
                return ret;
            }

            if (isBits)
                datas.Bits -= (marketItem.BitPrice * quantity);
            else
                datas.Shards -= (marketItem.ShardPrice * quantity);

            switch (marketItem.Type)
            {
                case DataModels.Items.ItemType.Armor:
                case DataModels.Items.ItemType.Bag:
                case DataModels.Items.ItemType.Weapon:
                case DataModels.Items.ItemType.Jewelry:
                    {
                        datas.Inventory.Add(new DataModels.InventorySlot
                        {
                            ItemId = marketItem.ItemId,
                            Type = marketItem.Type,
                            Quantity = quantity,
                            SoulId = datas.Id,
                            LootedAt = DateTime.Now
                        });
                        DataRepositories.SoulRepository.Update(datas);
                    }
                    break;
                case DataModels.Items.ItemType.Consumable:
                    {
                        var exists = datas.Inventory.FirstOrDefault(i => i.ItemId.Equals(marketItem.ItemId));
                        if (exists != null)
                        {
                            datas.Inventory.Remove(exists);
                            exists.Quantity += quantity;
                            datas.Inventory.Add(exists);
                            DataRepositories.SoulRepository.Update(datas);
                        }
                        else
                        {
                            datas.Inventory.Add(new DataModels.InventorySlot
                            {
                                ItemId = marketItem.ItemId,
                                Type = marketItem.Type,
                                Quantity = quantity,
                                SoulId = datas.Id,
                                LootedAt = DateTime.Now
                            });
                            DataRepositories.SoulRepository.Update(datas);
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

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.BuyMarketItem,
                Success = true,
                Json = "Item added to inventory."
            };
            ret.Succeeded = true;
            return ret;
        }

        private bool IsCurrencyAvailable(DataModels.Soul datas, DataModels.MarketSlot slot, int quantity, bool isBits)
        {
            return isBits ? (datas.Bits * quantity) < (slot.BitPrice * quantity) : (datas.Shards * quantity) < (slot.ShardPrice * quantity);
        }
    }
}
