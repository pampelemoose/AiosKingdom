using Newtonsoft.Json;
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
                List<Network.AdventureState.ActionResult> message;
                if (adventure.DoNothingTurn(datas, out message))
                {
                    var state = adventure.GetActualState();

                    if (state.CurrentHealth <= 0)
                    {
                        AdventureManager.Instance.PlayerDied(soul.Id);

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
                            Code = Network.CommandCodes.Dungeon.DoNothingTurn,
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
                Code = Network.CommandCodes.Dungeon.DoNothingTurn,
                Success = false,
                Json = "Failed to do nothing..."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
