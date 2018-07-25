using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DispatchServer.Commands
{
    public class ClientAnnounceDisconnectionCommand : ACommand
    {
        public ClientAnnounceDisconnectionCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ClientsManager.Instance.DisconnectClient(ret.ClientId);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_AnnounceDisconnection,
                Json = JsonConvert.SerializeObject("Disconnected !")
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
