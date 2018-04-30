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
            string username = _args.Args[0];
            string password = _args.Args[1];

            var userExists = DataRepositories.UserRepository.GetByCredentials(username, password);
            if (userExists != null)
            {
                Console.WriteLine($"User [{userExists.Username}] connected.");

                var token = Guid.NewGuid();

                ClientsManager.Instance.AuthenticateClient(_args.ClientId, token);
                ClientsManager.Instance.SetUserId(_args.ClientId, userExists.Id);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_Authenticate,
                    Json = JsonConvert.SerializeObject(token)
                };
                ret.Succeeded = true;
            }
            else
            {
                Console.WriteLine($"credentials unknown or not matching");

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Client_Authenticate,
                    Json = JsonConvert.SerializeObject(Guid.Empty)
                };
                ret.Succeeded = true;
            }

            return ret;
        }
    }
}
