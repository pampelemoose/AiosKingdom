using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class ItemCommand : ACommand
    {
        public ItemCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Item,
                Json = JsonConvert.SerializeObject(DataManager.Instance.Items),
                Success = true
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
