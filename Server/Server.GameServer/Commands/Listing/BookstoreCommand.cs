using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class BookstoreCommand : ACommand
    {
        public BookstoreCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Bookstore,
                Json = JsonConvert.SerializeObject(DataManager.Instance.Bookstores),
                Success = true
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
