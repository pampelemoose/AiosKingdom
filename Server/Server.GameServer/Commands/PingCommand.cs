using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class PingCommand : ACommand
    {
        public PingCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Ping,
                Json = JsonConvert.SerializeObject(0)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
