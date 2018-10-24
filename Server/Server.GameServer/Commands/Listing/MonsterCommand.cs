using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class MonsterCommand : ACommand
    {
        public MonsterCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Monster,
                Json = JsonConvert.SerializeObject(DataManager.Instance.Monsters)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
