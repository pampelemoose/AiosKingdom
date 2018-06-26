using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class DungeonLootRoomCommand : ACommand
    {
        public DungeonLootRoomCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(ret.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soul);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon_LootRoom,
                Success = true,
                Json = JsonConvert.SerializeObject(adventure.LootRoom())
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
