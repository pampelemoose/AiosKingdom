using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Server
{
    public class AuthenticateCommand : ACommand
    {
        public AuthenticateCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            Guid token = Guid.Parse(_args.Args[0]);
            var now = DateTime.Now;

            var tokenExists = DataRepositories.AuthTokenRepository.Get(token);
            if (tokenExists != null)
            {
                DataRepositories.AuthTokenRepository.Remove(tokenExists);

                if ((now - tokenExists.CreatedAt).Seconds < 5)
                {
                    var authToken = Guid.NewGuid();
                    var user = DataRepositories.AppUserRepository.GetByIdentifier(tokenExists.UserId);

                    ClientsManager.Instance.AuthenticateClient(_args.ClientId, authToken);
                    ClientsManager.Instance.SetUserId(_args.ClientId, user.Id);

                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Game_Authenticate,
                        Json = JsonConvert.SerializeObject(authToken),
                        Success = true
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
                    Code = Network.CommandCodes.Game_Authenticate,
                    Json = "Failure in authentication. Try again later.",
                    Success = false
                };
                ret.Succeeded = true;
            }

            return ret;
        }
    }
}
