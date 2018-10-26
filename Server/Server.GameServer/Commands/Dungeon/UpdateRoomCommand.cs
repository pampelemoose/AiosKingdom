using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class UpdateRoomCommand : ACommand
    {
        public UpdateRoomCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            if (adventure != null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Dungeon.UpdateRoom,
                    Success = true,
                    Json = JsonConvert.SerializeObject(adventure.GetActualState())
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.UpdateRoom,
                Success = false,
                Json = "Failed to update the room."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
