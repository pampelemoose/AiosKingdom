using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientAuthenticateCommand : ACommand
    {
        public ClientAuthenticateCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            Guid token = Guid.Parse(_args.Args[0]);
            var now = DateTime.Now;

            var tokenExists = DataRepositories.TokenRepository.Get(token);
            if (tokenExists != null)
            {
                DataRepositories.TokenRepository.Remove(tokenExists);

                if ((now - tokenExists.CreatedAt).Seconds < 5)
                {
                    var authToken = Guid.NewGuid();

                    ClientsManager.Instance.AuthenticateClient(_args.ClientId, authToken);
                    ClientsManager.Instance.SetUserId(_args.ClientId, tokenExists.UserId);

                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_Authenticate,
                        Json = JsonConvert.SerializeObject(authToken)
                    };
                    ret.Succeeded = true;

                    Console.WriteLine($"Token [{token}] authenticated.");
                }
            }
            else
            {
                Console.WriteLine($"token not matching");

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
