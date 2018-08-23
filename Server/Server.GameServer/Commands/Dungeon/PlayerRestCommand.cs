using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class PlayerRestCommand : ACommand
    {
        public PlayerRestCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);

            if (soul != null)
            {
                var adventure = AdventureManager.Instance.GetAdventure(soul.Id);

                if (adventure != null)
                {
                    adventure.PlayerRest(SoulManager.Instance.GetDatas(ret.ClientId));

                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Dungeon.PlayerRest,
                        Success = true,
                        Json = "You are well rested."
                    };
                    ret.Succeeded = true;

                    return ret;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.PlayerRest,
                Success = false,
                Json = "Couldn't rest."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
