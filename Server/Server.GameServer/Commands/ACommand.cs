using System;
using System.Collections.Generic;
using System.Text;

namespace Server.GameServer.Commands
{
    public struct CommandArgs
    {
        public Guid ClientId { get; set; }
        public bool IsValid { get; set; }
        public int CommandCode { get; set; }
        public string[] Args { get; set; }
    }

    public struct CommandResult
    {
        public Guid ClientId { get; set; }
        public bool Succeeded { get; set; }
        public Network.Message ClientResponse { get; set; }
    }

    public abstract class ACommand
    {
        protected CommandArgs _args;

        public ACommand(CommandArgs args)
        {
            _args = args;
        }

        public CommandResult Execute()
        {
            CommandResult ret = new CommandResult { Succeeded = false };

            ret.ClientId = _args.ClientId;

            ret = ExecuteLogic(ret);

            return ret;
        }

        protected abstract CommandResult ExecuteLogic(CommandResult ret);
    }
}
