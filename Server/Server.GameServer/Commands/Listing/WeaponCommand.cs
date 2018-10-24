using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class WeaponCommand : ACommand
    {
        public WeaponCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Weapon,
                Json = JsonConvert.SerializeObject(DataManager.Instance.Weapons)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
