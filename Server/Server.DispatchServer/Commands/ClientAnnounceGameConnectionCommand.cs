using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DispatchServer.Commands
{
    public class ClientAnnounceGameConnectionCommand : ACommand
    {
        public ClientAnnounceGameConnectionCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var id = Guid.Parse(_args.Args[0]);
            var userId = ClientsManager.Instance.GetUserId(ret.ClientId);
            var server = DataRepositories.ConfigRepository.GetById(id);

            if (server != null && server.SlotAvailable < server.SlotLimit)
            {
                var token = DataRepositories.AuthTokenRepository.Create(userId);
                var connect = new Network.GameServerConnection
                {
                    Host = server.Host,
                    Port = server.Port,
                    Token = token.Token
                };

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_AnnounceGameConnection,
                    Json = JsonConvert.SerializeObject(connect),
                    Success = true
                };
                ret.Succeeded = true;

                return ret;
            }

            if (server != null && server.SlotAvailable >= server.SlotLimit)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_AnnounceGameConnection,
                    Json = "No free slot available, please connect to another server or try again later.",
                    Success = false
                };
                ret.Succeeded = true;
            }
            else
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_AnnounceGameConnection,
                    Json = "Server unknown. Please connect to a listed server.",
                    Success = false
                };
                ret.Succeeded = true;
            }


            return ret;
        }
    }
}
