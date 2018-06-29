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
            var adventure = AdventureManager.Instance.GetAdventure(soul);

            if (adventure != null)
            {
                if (adventure.EnemyTurn(datas))
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Dungeon.EnemyTurn,
                        Success = true,
                        Json = "Enemy turn successful."
                    };
                    ret.Succeeded = true;

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
