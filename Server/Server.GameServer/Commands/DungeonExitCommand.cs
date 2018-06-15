using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class DungeonExitCommand : ACommand
    {
        public DungeonExitCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);

            AdventureManager.Instance.ExitRoom(soul.Id);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon_Exit,
                Success = true,
                Json = "Exited the dungeon."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
