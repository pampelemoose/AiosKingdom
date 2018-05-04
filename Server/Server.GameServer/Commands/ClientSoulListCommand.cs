using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientSoulListCommand : ACommand
    {
        public ClientSoulListCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var userId = ClientsManager.Instance.GetUserId(ret.ClientId);
            var souls = DataRepositories.SoulRepository.GetSoulsByUserId(userId);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_SoulList,
                Json = JsonConvert.SerializeObject(souls)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
