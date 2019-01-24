using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Player
{
    public class CurrentSoulDatasCommand : ACommand
    {
        public CurrentSoulDatasCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var datas = SoulManager.Instance.GetDatas(ret.ClientId);

            if (datas != null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.CurrentSoulDatas,
                    Json = JsonConvert.SerializeObject(datas),
                    Success = true
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.CurrentSoulDatas,
                Json = "No datas available.",
                Success = false
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
