﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Server
{
    public class CreateSoulCommand : ACommand
    {
        private DataModels.Config _config;

        public CreateSoulCommand(CommandArgs args, DataModels.Config config)
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulName = _args.Args[0];
            var userId = ClientsManager.Instance.GetUserId(ret.ClientId);

            var soul = new DataModels.Soul
            {
                Id = Guid.NewGuid(),
                ServerId = Guid.Parse(ConfigurationManager.AppSettings.Get("ConfigId")),
                Name = soulName,
                AppUserId = userId,
                Level = 1,
                TimePlayed = 0,
                CurrentExperience = 0,
                Stamina = 0,
                Energy = 0,
                Strength = 0,
                Agility = 0,
                Intelligence = 0,
                Wisdom = 0,
                Spirits = _config.SpiritsPerLevelUp,
                Embers = _config.EmbersPerLevelUp,
                Shards = 0,
                Bits = 0,
                Equipment = new DataModels.Equipment
                {
                    Id = Guid.NewGuid()
                }
            };

            var result = DataRepositories.SoulRepository.CreateSoul(soul);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Server.CreateSoul,
                Success = result,
                Json = result ? "Soul created !" : "Soulname already in use. Please enter another one."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
