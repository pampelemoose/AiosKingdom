using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class SpecialMarketCommand : ACommand
    {
        public SpecialMarketCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.SpecialsMarket,
                //Json = JsonConvert.SerializeObject(items, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })
                Json = JsonConvert.SerializeObject(Market.Instance.Specials)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
