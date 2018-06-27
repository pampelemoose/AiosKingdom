using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class BagCommand : ACommand
    {
        public BagCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var bags = DataRepositories.BagRepository.GetAll();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Bag,
                Json = JsonConvert.SerializeObject(bags)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
