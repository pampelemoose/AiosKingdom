using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Server
{
    public class DisconnectSoulCommand : ACommand
    {
        public DisconnectSoulCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            if (SoulManager.Instance.DisconnectSoul(ret.ClientId))
            {
                Console.WriteLine($"{ret.ClientId} Soul disconnected.");

                ClientsManager.Instance.DisconnectClient(ret.ClientId);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Server.DisconnectSoul,
                    Success = true,
                    Json = "Soul Disconnected"
                };
                ret.Succeeded = true;
                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Server.DisconnectSoul,
                Success = false,
                Json = "Could not disconnect soul"
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
