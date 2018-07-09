﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Listing
{
    public class MonsterCommand : ACommand
    {
        public MonsterCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var monsters = DataRepositories.MonsterRepository.GetAll();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Listing.Monster,
                Json = JsonConvert.SerializeObject(monsters)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}