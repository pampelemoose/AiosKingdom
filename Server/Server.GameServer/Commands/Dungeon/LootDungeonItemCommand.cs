using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class LootDungeonItemCommand : ACommand
    {
        public LootDungeonItemCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(ret.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soul);
            var lootId = Guid.Parse(_args.Args[0]);

            if (adventure.LootItem(soul, ret.ClientId, lootId))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Dungeon.LootItem,
                    Success = true,
                    Json = "Item looted successfully"
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.LootItem,
                Success = false,
                Json = "Couldn't loot item."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
