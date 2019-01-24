using Newtonsoft.Json;
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
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            if (adventure != null)
            {
                var result = adventure.PlayerRest();

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Dungeon.PlayerRest,
                    Success = true,
                    Json = JsonConvert.SerializeObject(result)
                };
                ret.Succeeded = true;

                return ret;
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
