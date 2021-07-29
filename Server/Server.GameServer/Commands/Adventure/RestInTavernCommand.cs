using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Adventure
{
    public class RestInTavernCommand : ACommand
    {
        public RestInTavernCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var tavernId = Guid.Parse(_args.Args[0]);

            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            if (adventure == null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.RestInTavern,
                    Success = false,
                    Json = "No adventure found."
                };
                ret.Succeeded = true;

                return ret;
            }

            var tavern = DataManager.Instance.Taverns.FirstOrDefault(t => t.Id == tavernId);

            if (tavern == null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.RestInTavern,
                    Success = false,
                    Json = "No tavern found."
                };
                ret.Succeeded = true;

                return ret;
            }

            adventure.RestInTavern(tavern.RestStamina);


            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Adventure.RestInTavern,
                Success = true,
                Json = $"Restored {tavern.RestStamina} stamina."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
