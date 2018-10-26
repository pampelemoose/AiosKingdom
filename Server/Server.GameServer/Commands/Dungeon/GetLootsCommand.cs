using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class GetLootsCommand : ACommand
    {
        public GetLootsCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(ret.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.GetLoots,
                Success = true,
                Json = JsonConvert.SerializeObject(adventure.GetLoots())
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
