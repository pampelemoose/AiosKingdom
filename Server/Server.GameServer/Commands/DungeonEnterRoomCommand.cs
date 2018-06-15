using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class DungeonEnterRoomCommand : ACommand
    {
        public DungeonEnterRoomCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);
            var dungeonId = Guid.Parse(_args.Args[0]);

            if (soul.Knowledge.Count > 0)
            {
                var adventure = AdventureManager.Instance.OpenRoom(soul, dungeonId);

                if (adventure.RoomNumber == 0)
                {
                    adventure.SetPlayerState(SoulManager.Instance.GetDatas(_args.ClientId));
                }

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Dungeon_EnterRoom,
                    Success = true,
                    Json = JsonConvert.SerializeObject(adventure.GetActualState())
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon_EnterRoom,
                Success = false,
                Json = "You must have learned at least one skill."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
