using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientMarketListCommand : ACommand
    {
        public ClientMarketListCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var serverId = Guid.Parse(ConfigurationManager.AppSettings.Get("ConfigId"));
            var items = DataRepositories.MarketRepository.GetAll().Where(i => i.ServerId.Equals(serverId)).ToList();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_MarketList,
                Json = JsonConvert.SerializeObject(items, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
