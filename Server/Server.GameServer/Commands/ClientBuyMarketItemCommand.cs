using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientBuyMarketItemCommand : ACommand
    {
        public ClientBuyMarketItemCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var marketSlotId = Guid.Parse(_args.Args[0]);
            var datas = SoulManager.Instance.GetSoul(ret.ClientId);

            var marketItem = DataRepositories.MarketRepository.GetById(marketSlotId);

            if (marketItem == null || (marketItem != null && (datas.Shards < marketItem.ShardPrice || datas.Bits < marketItem.ShardPrice)))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_BuyMarketItem,
                    Success = false,
                    Json = "Item sold or not available anymore."
                };
                ret.Succeeded = true;
                return ret;
            }

            datas.Shards -= marketItem.ShardPrice;
            datas.Bits -= marketItem.BitPrice;

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
                            Quantity = 1,
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
                            ++exists.Quantity;
                            datas.Inventory.Add(exists);
                            DataRepositories.SoulRepository.Update(datas);
                        }
                        else
                        {
                            datas.Inventory.Add(new DataModels.InventorySlot
                            {
                                ItemId = marketItem.ItemId,
                                Type = marketItem.Type,
                                Quantity = 1,
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
                --marketItem.Quantity;

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
                Code = Network.CommandCodes.Client_BuyMarketItem,
                Success = true,
                Json = "Item added to inventory."
            };
            ret.Succeeded = true;
            return ret;
        }
    }
}
