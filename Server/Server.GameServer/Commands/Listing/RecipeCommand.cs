using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class RecipeCommand : ACommand
    {
        public RecipeCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Recipes,
                Json = JsonConvert.SerializeObject(DataManager.Instance.Recipes),
                Success = true
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
