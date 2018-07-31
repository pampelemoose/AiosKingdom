using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DispatchServer.Commands
{
    public class ClientCreateAccountCommand : ACommand
    {
        public ClientCreateAccountCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var appUser = new DataModels.AppUser
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                PublicKey = Guid.NewGuid(),
                SafeKey = Guid.NewGuid()
            };

            if (DataRepositories.AppUserRepository.Create(appUser))
            {
                Console.WriteLine($"User [{appUser.Identifier}] created.");

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_CreateAccount,
                    Json = JsonConvert.SerializeObject(appUser),
                    Success = true
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_CreateAccount,
                Json = JsonConvert.SerializeObject("Failed to create appUser."),
                Success = false
            };
            ret.Succeeded = false;

            return ret;
        }
    }
}
