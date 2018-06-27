using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Player
{
    public class SoulDatasCommand : ACommand
    {
        public SoulDatasCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var datas = SoulManager.Instance.GetSoul(ret.ClientId);

            if (datas != null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.SoulDatas,
                    Json = JsonConvert.SerializeObject(datas, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })
                };
                ret.Succeeded = true;
            }

            return ret;
        }
    }
}
