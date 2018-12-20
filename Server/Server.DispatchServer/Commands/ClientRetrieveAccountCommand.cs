using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DispatchServer.Commands
{
    public class ClientRetrieveAccountCommand : ACommand
    {
        public ClientRetrieveAccountCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var safeKey = Guid.Parse(_args.Args[0]);
            var user = DataRepositories.AppUserRepository.GetBySafeKey(safeKey);

            if (user != null)
            {
                Log.Instance.Write(Log.Level.Infos, $"User [{user.Identifier}] retrieved with safeKey.");

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_RetrieveAccount,
                    Json = JsonConvert.SerializeObject(user),
                    Success = true
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_RetrieveAccount,
                Json = "Failed to retrieve account.",
                Success = false
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
