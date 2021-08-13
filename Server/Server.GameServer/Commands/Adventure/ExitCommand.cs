using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Adventure
{
    public class ExitCommand : ACommand
    {
        public ExitCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);

            AdventureManager.Instance.FinishAdventure(soulId);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Adventure.Exit,
                Success = true,
                Json = "Exited the edventure."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
