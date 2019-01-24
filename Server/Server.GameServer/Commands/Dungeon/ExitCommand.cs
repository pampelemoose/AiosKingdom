using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class ExitCommand : ACommand
    {
        public ExitCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);

            AdventureManager.Instance.ExitRoom(soulId);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.Exit,
                Success = true,
                Json = "Exited the dungeon."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
