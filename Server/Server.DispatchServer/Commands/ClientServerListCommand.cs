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
            var id = ClientsManager.Instance.GetUserId(ret.ClientId);
            var infos = GameServerManager.Instance.ServerInfos.ToArray();
            var slots = DataRepositories.UserRepository.GetById(id).ServerSlots;

            for (int i = 0; i < infos.Length; ++i)
            {
                var soulOnServer = slots.Where(s => s.ServerIdentifier.Equals(infos[i].Name)).ToList();
                if (soulOnServer != null)
                {
                    infos[i].SoulCount = soulOnServer.Count;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_ServerList,
                Json = JsonConvert.SerializeObject(infos.ToList())
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
