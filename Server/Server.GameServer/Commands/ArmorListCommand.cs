using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ArmorListCommand : ACommand
    {
        public ArmorListCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var armors = DataRepositories.ArmorRepository.GetAll();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.ArmorList,
                Json = JsonConvert.SerializeObject(armors)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
