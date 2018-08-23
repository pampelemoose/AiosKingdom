using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class DoNothingTurnCommand : ACommand
    {
        public DoNothingTurnCommand(CommandArgs args)
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
                string message = "";
                if (adventure.DoNothingTurn(datas, out message))
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Dungeon.DoNothingTurn,
                        Success = true,
                        Json = message
                    };
                    ret.Succeeded = true;

                    return ret;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.DoNothingTurn,
                Success = false,
                Json = "Failed to do nothing..."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
