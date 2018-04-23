using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DispatchServer.Commands
{
    public class ClientServerListCommand : ACommand
    {
        public ClientServerListCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_Authenticate,
                Json = JsonConvert.SerializeObject(GameServerManager.Instance.ServerInfos)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
