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
            var servers = DataRepositories.ConfigRepository.GetAll();
            var slots = DataRepositories.UserRepository.GetById(id).Souls;
            var infos = new List<Network.GameServerInfos>();

            foreach (var server in servers)
            {
                int soulCount = 0;
                var soulOnServer = slots.Where(s => s.ServerId.Equals(server.Id)).ToList();
                if (soulOnServer != null)
                {
                    soulCount = soulOnServer.Count;
                }

                var info = new Network.GameServerInfos
                {
                    Id = server.Id,
                    Name = server.Name,
                    Online = server.Online,
                    Difficulty = server.Difficulty.ToString(),
                    SlotsLimit = server.SlotLimit,
                    SlotsAvailable = server.SlotAvailable,
                    SoulCount = soulCount
                };
                infos.Add(info);
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
