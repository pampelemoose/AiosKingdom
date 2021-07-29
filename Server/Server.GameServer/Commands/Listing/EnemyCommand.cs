using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class EnemyCommand : ACommand
    {
        public EnemyCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Enemy,
                Json = JsonConvert.SerializeObject(DataManager.Instance.Enemies),
                Success = true
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
