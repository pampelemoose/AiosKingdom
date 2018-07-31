using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DispatchServer
{
    public class CommandManager
    {
        private List<Commands.ACommand> _pendingCommands;

        public CommandManager()
        {
            _pendingCommands = new List<Commands.ACommand>();
        }

        public void SendCommand(Commands.ACommand command)
        {
            _pendingCommands.Add(command);
        }

        public bool HasCommandLeft => _pendingCommands.Count() > 0;

        public Commands.CommandResult ExecuteNextCommand()
        {
            if (_pendingCommands.Count > 0)
            {
                var command = _pendingCommands.First();

                if (command != null)
                {
                    _pendingCommands.Remove(command);

                    var ret = command.Execute();
                    return ret;
                }
            }

            return new Commands.CommandResult
            {
                Succeeded = false,
                ClientId = Guid.Empty
            };
        }
    }
}
