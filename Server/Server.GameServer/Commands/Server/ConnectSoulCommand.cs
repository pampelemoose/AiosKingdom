using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Server
{
    public class ConnectSoulCommand : ACommand
    {
        private DataModels.Config _config;

        public ConnectSoulCommand(CommandArgs args, DataModels.Config config) 
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = Guid.Parse(_args.Args[0]);

            if (SoulManager.Instance.ConnectSoul(ret.ClientId, soulId))
            {
                SoulManager.Instance.UpdateCurrentDatas(ret.ClientId, _config);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Server.ConnectSoul,
                    Success = true,
                    Json = "Soul Connected"
                };
                ret.Succeeded = true;
                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Server.ConnectSoul,
                Success = false,
                Json = "Could not connect soul"
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
