using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Jobs
{
    public class GetCommand : ACommand
    {
        public GetCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var job = SoulManager.Instance.GetJob(ret.ClientId);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Job.Get,
                Json = JsonConvert.SerializeObject(job),
                Success = true
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
