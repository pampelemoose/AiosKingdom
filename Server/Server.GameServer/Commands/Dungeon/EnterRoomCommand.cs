using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class EnterRoomCommand : ACommand
    {
        public EnterRoomCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            //if (adventure != null)
            //{
            //    var dungeonId = adventure.AdventureId;

            //    AdventureManager.Instance.OpenRoom(soulId, dungeonId);

            //    ret.ClientResponse = new Network.Message
            //    {
            //        Code = Network.CommandCodes.Dungeon.EnterRoom,
            //        Success = true,
            //        Json = "Entered next room."
            //    };
            //    ret.Succeeded = true;

            //    return ret;
            //}

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.EnterRoom,
                Success = false,
                Json = "Failed to enter the room."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
