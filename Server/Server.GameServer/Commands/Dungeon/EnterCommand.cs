using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class EnterCommand : ACommand
    {
        public EnterCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);
            var datas = SoulManager.Instance.GetDatas(_args.ClientId);
            var dungeonId = Guid.Parse(_args.Args[0]);
            var bagItems = JsonConvert.DeserializeObject<List<Network.AdventureState.BagItem>>(_args.Args[1]);

            if (soul.Knowledge.Count > 0)
            {
                var adventure = AdventureManager.Instance.OpenRoom(soul, datas, dungeonId, bagItems);

                SoulManager.Instance.UpdateSoul(_args.ClientId, soul);

                if (adventure.RoomNumber == 0)
                {
                    adventure.SetPlayerState(SoulManager.Instance.GetDatas(_args.ClientId));
                }

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Dungeon.Enter,
                    Success = true,
                    Json = "Entered the dungeon."
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.Enter,
                Success = false,
                Json = "You must have learned at least one skill."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
