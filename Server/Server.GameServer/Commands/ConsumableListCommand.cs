using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ConsumableListCommand : ACommand
    {
        public ConsumableListCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var consumables = DataRepositories.ConsumableRepository.GetAll();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.ConsumableList,
                Json = JsonConvert.SerializeObject(consumables)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
