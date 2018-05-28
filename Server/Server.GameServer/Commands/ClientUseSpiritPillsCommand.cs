using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientUseSpiritPillsCommand : ACommand
    {
        private DataModels.Config _config;

        public ClientUseSpiritPillsCommand(CommandArgs args, DataModels.Config config)
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(ret.ClientId);
            DataModels.Soul.Stats statId = (DataModels.Soul.Stats)Enum.Parse(typeof(DataModels.Soul.Stats), _args.Args[0]);
            int amount = int.Parse(_args.Args[1]);

            if (soul.Spirits >= amount && amount > 0)
            {
                soul.Spirits -= amount;
                switch (statId)
                {
                    case DataModels.Soul.Stats.Stamina:
                        soul.Stamina += amount;
                        break;
                    case DataModels.Soul.Stats.Energy:
                        soul.Energy += amount;
                        break;
                    case DataModels.Soul.Stats.Strength:
                        soul.Strength += amount;
                        break;
                    case DataModels.Soul.Stats.Agility:
                        soul.Agility += amount;
                        break;
                    case DataModels.Soul.Stats.Intelligence:
                        soul.Intelligence += amount;
                        break;
                    case DataModels.Soul.Stats.Wisdom:
                        soul.Wisdom += amount;
                        break;
                    default:
                        soul.Spirits += amount;
                        break;
                }

                if (DataRepositories.SoulRepository.Update(soul))
                {
                    SoulManager.Instance.UpdateSoul(ret.ClientId, soul);
                    SoulManager.Instance.UpdateCurrentDatas(ret.ClientId, _config);

                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_UseSpiritPills,
                        Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = true, Message = "Used pills" })
                    };
                    ret.Succeeded = true;

                    return ret;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_UseSpiritPills,
                Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = false, Message = "Couldn't use pills" })
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
