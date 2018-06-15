using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientConnectSoulCommand : ACommand
    {
        private DataModels.Config _config;

        public ClientConnectSoulCommand(CommandArgs args, DataModels.Config config) 
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = Guid.Parse(_args.Args[0]);
            var soul = DataRepositories.SoulRepository.GetById(soulId);
            if (soul != null)
            {
                if (SoulManager.Instance.ConnectSoul(ret.ClientId, soul))
                {
                    SoulManager.Instance.UpdateCurrentDatas(ret.ClientId, _config);

                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_ConnectSoul,
                        Success = true,
                        Json = "Soul Connected"
                    };
                    ret.Succeeded = true;
                    return ret;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_ConnectSoul,
                Success = false,
                Json = "Could not connect soul"
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
