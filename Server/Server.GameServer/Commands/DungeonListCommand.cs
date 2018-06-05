using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class DungeonListCommand : ACommand
    {
        public DungeonListCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var dungeons = DataRepositories.DungeonRepository.GetAll();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.DungeonList,
                Json = JsonConvert.SerializeObject(dungeons)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
