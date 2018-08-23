using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class BuyShopItemCommand : ACommand
    {
        public BuyShopItemCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);
            var tempId = Guid.Parse(_args.Args[0]);
            var quantity = int.Parse(_args.Args[1]);

            var adventure = AdventureManager.Instance.GetAdventure(soul.Id);

            if (adventure != null)
            {
                if (adventure.BuyShopItem(tempId, quantity, soul.Id, ret.ClientId))
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Dungeon.BuyShopItem,
                        Success = true,
                        Json = "Bought item."
                    };
                    ret.Succeeded = true;

                    return ret;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.BuyShopItem,
                Success = false,
                Json = "An error occured when trying to buy item."
            };
            ret.Succeeded = true;

            return ret; 
        }
    }
}
