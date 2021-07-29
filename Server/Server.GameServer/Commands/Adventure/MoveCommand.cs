using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Adventure
{
    public class MoveCommand : ACommand
    {
        public MoveCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var movement = (GameServer.Adventure.Movement)Enum.Parse(typeof(GameServer.Adventure.Movement), _args.Args[0]);

            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            if (adventure == null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.Move,
                    Success = false,
                    Json = "No adventure found."
                };
                ret.Succeeded = true;

                return ret;
            }

            if (!adventure.Move(movement))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.Move,
                    Success = false,
                    Json = "Could not move."
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Adventure.Move,
                Success = true,
                Json = movement.ToString()
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
