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
        private DataModels.Town _config;

        public UseSpiritPillsCommand(CommandArgs args, DataModels.Town config)
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var currencies = SoulManager.Instance.GetCurrencies(ret.ClientId);
            var datas = SoulManager.Instance.GetBaseDatas(ret.ClientId);
            int statCount = int.Parse(_args.Args[0]);
            int statIndex = 1;
            int totalAmount = 0;

            while (statCount > 0)
            {
                Network.Stats statId = (Network.Stats)Enum.Parse(typeof(Network.Stats), _args.Args[statIndex]);
                int amount = int.Parse(_args.Args[statIndex + 1]);

                if (currencies.Spirits >= amount && amount > 0)
                {
                    totalAmount += amount;
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

                }

                statIndex += 2;
                --statCount;
            }

            if (totalAmount > 0)
            {
                SoulManager.Instance.UpdateBaseDatas(ret.ClientId, datas);
                SoulManager.Instance.UpdateCurrencies(ret.ClientId, currencies);
                SoulManager.Instance.UpdateCurrentDatas(ret.ClientId, _config);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.UseSpiritPills,
                    Success = true,
                    Json = $"Used {statIndex} pill{(statIndex > 0 ? "s" : "")}."
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
