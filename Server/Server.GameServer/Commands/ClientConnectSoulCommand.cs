using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientConnectSoulCommand : ACommand
    {
        public ClientConnectSoulCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = Guid.Parse(_args.Args[0]);
            var soul = DataRepositories.SoulRepository.GetById(soulId);
            if (soul != null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_ConnectSoul,
                    Json = JsonConvert.SerializeObject(soul)
                };
                ret.Succeeded = true;
            }

            return ret;
        }
    }
}
