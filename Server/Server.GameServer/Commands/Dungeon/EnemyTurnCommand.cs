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
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);
            var datas = SoulManager.Instance.GetDatas(_args.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soul.Id);

            if (adventure != null)
            {
                string message = "";
                if (adventure.EnemyTurn(datas, out message))
                {
                    var state = adventure.GetActualState();

                    if (state.CurrentHealth <= 0)
                    {
                        AdventureManager.Instance.PlayerDied(soul.Id);

                        ret.ClientResponse = new Network.Message
                        {
                            Code = Network.CommandCodes.Dungeon.PlayerDied,
                            Success = true,
                            Json = $"{message}. You died. You lost all Experience and Shards from this dungeon."
                        };
                        ret.Succeeded = true;
                    }
                    else
                    {
                        ret.ClientResponse = new Network.Message
                        {
                            Code = Network.CommandCodes.Dungeon.EnemyTurn,
                            Success = true,
                            Json = message
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
