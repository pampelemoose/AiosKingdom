using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.GameServer.Commands
{
    public class DispatchServerConnectCommand : ACommand
    {
        public DispatchServerConnectCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            Guid clientId = Guid.Parse(_args.Args[0]);

            if (ClientsManager.Instance.SetDispatchServer(clientId))
            {
                var token = Guid.NewGuid();
                ClientsManager.Instance.AuthenticateClient(clientId, token);
                ClientsManager.Instance.SetDispatchServer(clientId);

                Console.WriteLine($"Sending token [{token}]");

                return new CommandResult
                {
                    Succeeded = true,
                    ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Gameserver_Connect,
                        Json = JsonConvert.SerializeObject(token)
                    }
                };
            }

            return new CommandResult
            {
                Succeeded = false
            };
        }
    }
}
