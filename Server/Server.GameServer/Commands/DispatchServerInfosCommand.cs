using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.GameServer.Commands
{
    public class DispatchServerInfosCommand : ACommand
    {
        public DispatchServerInfosCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var gameInfos = new Network.GameServerInfos // TODO : Get real infos
            {
                Name = "Some Name",
                Difficulty = "Much Hard",
                SlotsLimit = 100,
                SlotsAvailable = 100
            };

            Console.WriteLine($"Sending infos [{gameInfos}]");

            return new CommandResult
            {
                Succeeded = true,
                ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Gameserver_Infos,
                    Json = JsonConvert.SerializeObject(gameInfos)
                }
            };
        }
    }
}
