using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class ArmorCommand : ACommand
    {
        public ArmorCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var armors = DataRepositories.ArmorRepository.GetAll();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Armor,
                Json = JsonConvert.SerializeObject(armors)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
