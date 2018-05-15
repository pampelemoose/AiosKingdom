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
                    Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = false, Message = "Item sold or not available anymore." })
                };
                ret.Succeeded = true;
                return ret;
            }

            datas.Shards -= marketItem.ShardPrice;
            datas.Bits -= marketItem.BitPrice;

            //PlayerLootItem(soulId, marketItem.ItemId, marketItem.Type, 1);

            datas.Inventory.Add(new DataModels.InventorySlot
            {
                ItemId = marketItem.ItemId,
                Type = marketItem.Type,
                Quantity = 1,
                SoulId = datas.Id
            });
            DataRepositories.SoulRepository.Update(datas);

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
                Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = true, Message = "Item added to inventory." })
            };
            ret.Succeeded = true;
            return ret;
        }
    }
}
