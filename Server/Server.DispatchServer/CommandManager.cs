using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DispatchServer
{
    public class CommandManager
    {
        private Object _thisLock = new Object();
        private List<Commands.ACommand> _pendingCommands;

        public CommandManager()
        {
            lock (_thisLock)
            {
                _pendingCommands = new List<Commands.ACommand>();
            }
        }

        public void SendCommand(Commands.ACommand command)
        {
            lock (_thisLock)
            {

                _pendingCommands.Add(command);
            }
        }

        public bool HasCommandLeft
        {
            get
            {
                lock (_thisLock)
                {
                    return _pendingCommands.Count() > 0;
                }
            }
        }

        public Commands.CommandResult ExecuteNextCommand()
        {
            lock (_thisLock)
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
}
