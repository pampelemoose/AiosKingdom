using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DispatchServer.Commands
{
    public class ClientAuthenticateCommand : ACommand
    {
        public ClientAuthenticateCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            Guid identifier = Guid.Parse(_args.Args[0]);

            var userExists = DataRepositories.AppUserRepository.GetByIdentifier(identifier);
            if (userExists != null)
            {
                Console.WriteLine($"User [{userExists.Identifier}] connected.");

                var token = Guid.NewGuid();

                ClientsManager.Instance.AuthenticateClient(_args.ClientId, token);
                ClientsManager.Instance.SetUserId(_args.ClientId, userExists.Identifier);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_Authenticate,
                    Json = JsonConvert.SerializeObject(token),
                    Success = true
                };
                ret.Succeeded = true;
            }
            else
            {
                Console.WriteLine($"credentials unknown or not matching");

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_Authenticate,
                    Json = JsonConvert.SerializeObject(Guid.Empty),
                    Success = false
                };
                ret.Succeeded = true;
            }

            return ret;
        }
    }
}
