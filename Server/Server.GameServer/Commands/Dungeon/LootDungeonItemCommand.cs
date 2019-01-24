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
            var soulId = SoulManager.Instance.GetSoulId(ret.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soulId);
            var lootId = Guid.Parse(_args.Args[0]);

            if (adventure.LootItem(ret.ClientId, lootId))
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
