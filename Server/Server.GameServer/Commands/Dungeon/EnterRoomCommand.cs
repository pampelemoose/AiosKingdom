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
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);
            var datas = SoulManager.Instance.GetDatas(_args.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soul.Id);

            if (adventure != null)
            {
                var dungeonId = adventure.DungeonId;

                AdventureManager.Instance.OpenRoom(soul, datas, dungeonId);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Dungeon.EnterRoom,
                    Success = true,
                    Json = "Entered next room."
                };
                ret.Succeeded = true;

                return ret;
            }

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
