using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Player
{
    public class InventoryCommand : ACommand
    {
        public InventoryCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var inventory = SoulManager.Instance.GetInventory(ret.ClientId);

            if (inventory != null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.Inventory,
                    Json = JsonConvert.SerializeObject(inventory),
                    Success = true
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.Inventory,
                Json = "No datas available.",
                Success = false
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
