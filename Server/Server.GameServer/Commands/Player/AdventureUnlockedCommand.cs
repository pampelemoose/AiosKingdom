using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Player
{
    public class AdventureUnlockedCommand : ACommand
    {
        public AdventureUnlockedCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var adventureLocks = SoulManager.Instance.GetAdventureLocks(ret.ClientId);

            if (adventureLocks != null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.AdventureUnlocked,
                    Json = JsonConvert.SerializeObject(adventureLocks),
                    Success = true
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.AdventureUnlocked,
                Json = "No datas available.",
                Success = false
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
