using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Player
{
    public class CurrenciesCommand : ACommand
    {
        public CurrenciesCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var currencies = SoulManager.Instance.GetCurrencies(ret.ClientId);

            if (currencies != null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.Currencies,
                    Json = JsonConvert.SerializeObject(currencies),
                    Success = true
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.Currencies,
                Json = "No datas available.",
                Success = false
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
