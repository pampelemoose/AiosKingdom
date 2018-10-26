using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// TODO : test
/// </summary>
namespace Server.GameServer.Commands.Player
{
    public class UseSpiritPillsCommand : ACommand
    {
        private DataModels.Config _config;

        public UseSpiritPillsCommand(CommandArgs args, DataModels.Config config)
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var currencies = SoulManager.Instance.GetCurrencies(ret.ClientId);
            var datas = SoulManager.Instance.GetBaseDatas(ret.ClientId);
            Network.Stats statId = (Network.Stats)Enum.Parse(typeof(Network.Stats), _args.Args[0]);
            int amount = int.Parse(_args.Args[1]);

            if (currencies.Spirits >= amount && amount > 0)
            {
                currencies.Spirits -= amount;
                switch (statId)
                {
                    case Network.Stats.Stamina:
                        datas.Stamina += amount;
                        break;
                    case Network.Stats.Energy:
                        datas.Energy += amount;
                        break;
                    case Network.Stats.Strength:
                        datas.Strength += amount;
                        break;
                    case Network.Stats.Agility:
                        datas.Agility += amount;
                        break;
                    case Network.Stats.Intelligence:
                        datas.Intelligence += amount;
                        break;
                    case Network.Stats.Wisdom:
                        datas.Wisdom += amount;
                        break;
                    default:
                        currencies.Spirits += amount;
                        break;
                }

                SoulManager.Instance.UpdateBaseDatas(ret.ClientId, datas);
                SoulManager.Instance.UpdateCurrencies(ret.ClientId, currencies);
                SoulManager.Instance.UpdateCurrentDatas(ret.ClientId, _config);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.UseSpiritPills,
                    Success = true,
                    Json = $"Used {amount} pill{(amount > 0 ? "s" : "")} in {statId}."
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.UseSpiritPills,
                Success = false,
                Json = "Couldn't use pills"
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
