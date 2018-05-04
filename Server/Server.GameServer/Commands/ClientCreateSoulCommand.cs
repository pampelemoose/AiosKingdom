using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientCreateSoulCommand : ACommand
    {
        public ClientCreateSoulCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulName = _args.Args[0];
            var userId = ClientsManager.Instance.GetUserId(ret.ClientId);

            var soul = new DataModels.Soul
            {
                Id = Guid.NewGuid(),
                Name = soulName,
                UserId = userId,
                Level = 1,
                TimePlayed = 0,
                CurrentExperience = 0,
                Stamina = 0,
                Energy = 0,
                Strength = 0,
                Agility = 0,
                Intelligence = 0,
                Wisdom = 0,
                Spirits = 0, // TODO : get value from config
                Embers = 0, // TODO : get value from config
                Shards = 0,
                Bits = 0
            };
            var equipment = new DataModels.Equipment
            {
                Id = Guid.NewGuid(),
                Soul = soul
            };
            soul.Equipment = equipment;

            if (GameRepository.CreateSoul(soul))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_SoulList,
                    Json = JsonConvert.SerializeObject(souls)
                };
                ret.Succeeded = true;
            }

            return ret;
        }
    }
}
