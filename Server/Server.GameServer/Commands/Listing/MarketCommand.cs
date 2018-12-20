using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class MarketCommand : ACommand
    {
        public MarketCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Market,
                //Json = JsonConvert.SerializeObject(items, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })
                Json = JsonConvert.SerializeObject(Market.Instance.Items),
                Success = true
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
