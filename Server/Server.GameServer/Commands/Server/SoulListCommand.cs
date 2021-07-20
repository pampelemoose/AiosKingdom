using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Server
{
    public class SoulListCommand : ACommand
    {
        private DataModels.Town _config;

        public SoulListCommand(CommandArgs args, DataModels.Town config) 
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var userId = ClientsManager.Instance.GetUserId(ret.ClientId);
            var souls = DataRepositories.SoulRepository.GetSoulsByAppUserId(userId);

            var soulList = new List<Network.SoulInfos>();
            foreach (var soul in souls)
            {
                var requiredExp = SoulManager.Instance.GetSoulRequiredExperienceToLevelUp(soul, _config);

                soulList.Add(new Network.SoulInfos
                {
                    Id = soul.Id,
                    Name = soul.Name,
                    Level = soul.Level,

                    TotalExperience = requiredExp,
                    Experience = soul.CurrentExperience,
                    Stamina = soul.Stamina,
                    Energy = soul.Energy,
                    Strength = soul.Strength,
                    Agility = soul.Agility,
                    Intelligence = soul.Intelligence,
                    Wisdom = soul.Wisdom
                });
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Server.SoulList,
                Json = JsonConvert.SerializeObject(soulList),
                Success = true
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
