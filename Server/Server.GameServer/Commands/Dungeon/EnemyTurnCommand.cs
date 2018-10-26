using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class EnemyTurnCommand : ACommand
    {
        public EnemyTurnCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            if (adventure != null)
            {
                List<Network.AdventureState.ActionResult> message;

                if (adventure.EnemyTurn(out message))
                {
                    var state = adventure.GetActualState();

                    if (state.State.CurrentHealth <= 0)
                    {
                        AdventureManager.Instance.PlayerDied(soulId);

                        message.Add(new Network.AdventureState.ActionResult
                        {
                            ResultType = Network.AdventureState.ActionResult.Type.PlayerDeath
                        });

                        ret.ClientResponse = new Network.Message
                        {
                            Code = Network.CommandCodes.Dungeon.PlayerDied,
                            Success = true,
                            Json = JsonConvert.SerializeObject(message)
                        };
                        ret.Succeeded = true;
                    }
                    else
                    {
                        ret.ClientResponse = new Network.Message
                        {
                            Code = Network.CommandCodes.Dungeon.EnemyTurn,
                            Success = true,
                            Json = JsonConvert.SerializeObject(message)
                        };
                        ret.Succeeded = true;
                    }

                    return ret;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.EnemyTurn,
                Success = false,
                Json = "An error occured when trying to execute enemy turn."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
