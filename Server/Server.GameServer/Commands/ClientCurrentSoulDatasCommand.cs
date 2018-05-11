using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientCurrentSoulDatasCommand : ACommand
    {
        public ClientCurrentSoulDatasCommand(CommandArgs args) 
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
                    Code = Network.CommandCodes.Client_CurrentSoulDatas,
                    Json = JsonConvert.SerializeObject(datas)
                };
                ret.Succeeded = true;
            }

            return ret;
        }
    }
}
