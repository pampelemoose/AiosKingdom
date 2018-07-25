using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.DispatchServer
{
    public class Server
    {
        private Thread _thread;
        private TcpListener _listener;
        private bool _isRunning;

        private System.Timers.Timer _flushCommands;
        private Dictionary<Guid, System.Timers.Timer> _pingers;

        private CommandManager _commandManager;
        private Dictionary<int, int> _commandArgCount;
        private Dictionary<int, Func<Commands.CommandArgs, Commands.ACommand>> _delegates;
        private List<Commands.CommandResult> _responses;

        public Server()
        {
            ThreadStart del = new ThreadStart(Run);
            _thread = new Thread(del);

            _flushCommands = new System.Timers.Timer(1000);
            _flushCommands.Elapsed += (sender, e) => {
                FlushCommands();
            };
            _flushCommands.AutoReset = true;
            _flushCommands.Enabled = true;

            _pingers = new Dictionary<Guid, System.Timers.Timer>();

            _commandManager = new CommandManager();

            _commandArgCount = new Dictionary<int, int>();
            _delegates = new Dictionary<int, Func<Commands.CommandArgs, Commands.ACommand>>();
            _responses = new List<Commands.CommandResult>();

            SetupClientDelegates();
        }

        public void Start()
        {
            string host = ConfigurationManager.AppSettings.Get("Host");
            string portStr = ConfigurationManager.AppSettings.Get("Port");
            int port = int.Parse(portStr);

            Console.WriteLine($"Starting TCPListener at address : {host}:{port} ...");

            _listener = new TcpListener(IPAddress.Parse(host), port);

            _thread.Start();

            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private void SetupClientDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Ping, 0);

            _commandArgCount.Add(Network.CommandCodes.Client_Authenticate, 2);
            _commandArgCount.Add(Network.CommandCodes.Client_ServerList, 0);
            _commandArgCount.Add(Network.CommandCodes.Client_AnnounceGameConnection, 1);
            _commandArgCount.Add(Network.CommandCodes.Client_AnnounceDisconnection, 0);

            _delegates.Add(Network.CommandCodes.Ping, (args) => { return new Commands.PingCommand(args); });

            _delegates.Add(Network.CommandCodes.Client_Authenticate, (args) => { return new Commands.ClientAuthenticateCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_ServerList, (args) => { return new Commands.ClientServerListCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_AnnounceGameConnection, (args) => { return new Commands.ClientAnnounceGameConnectionCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_AnnounceDisconnection, (args) => { return new Commands.ClientAnnounceDisconnectionCommand(args); });
        }

        private void Run()
        {
            _listener.Start();
            Console.WriteLine($"TCP Server is running.");
            
            _flushCommands.Start();

            while (_isRunning)
            {
                if (_listener.Pending())
                {
                    Socket newClient = _listener.AcceptSocket();

                    var clientId = Guid.NewGuid();
                    ClientsManager.Instance.AddClient(clientId, newClient);

                    AddPingerToClient(clientId);
                }

                List<Guid> disconnectedClients = ClientsManager.Instance.DisconnectedClientList;
                foreach (var client in ClientsManager.Instance.Clients)
                {
                    if (ClientsManager.Instance.GetPing(client.Key) > 10)
                    {
                        Console.WriteLine($"{client.Key} Timed out.");
                        disconnectedClients.Add(client.Key);
                        continue;
                    }

                    var socket = client.Value;

                    int bufferSize = socket.Available;
                    if (bufferSize > 0)
                    {
                        byte[] a = new byte[bufferSize];
                        int receivedSize = socket.Receive(a);
                        var encoder = new ASCIIEncoding();
                        var buffer = encoder.GetString(a);
                        var messages = buffer.Split('|');
                        foreach (var message in messages)
                        {
                            if (string.IsNullOrEmpty(message))
                                continue;

                            var json = JsonConvert.DeserializeObject<Network.Message>(message);
                            var commandArgs = ToCommandArgs(client.Key, json);

                            if (commandArgs.IsValid)
                            {
                                ClientsManager.Instance.Ping(commandArgs.ClientId);

                                AddCommand(commandArgs);
                            }
                        }
                    }
                }

                foreach (var disc in disconnectedClients)
                {
                    var socket = ClientsManager.Instance.Clients[disc];

                    if (ClientsManager.Instance.RemoveClient(disc))
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                }

                while (_responses.Count > 0)
                {
                    var response = TakeResponse();

                    if (!Guid.Empty.Equals(response.ClientId)
                        && ClientsManager.Instance.Clients.ContainsKey(response.ClientId))
                    {
                        try
                        {
                            var socket = ClientsManager.Instance.Clients[response.ClientId];
                            SendPacketOnSocket(socket, response.ClientResponse);
                        }
                        catch (SocketException sockEx)
                        {
                            Console.WriteLine($"Server is not online(1)... [{sockEx.Message}]");
                        }
                    }
                }
            }

            foreach (var client in ClientsManager.Instance.Clients)
            {
                SendPacketOnSocket(client.Value, new Network.Message
                {
                    Code = Network.CommandCodes.Client_AnnounceDisconnection,
                    Success = true,
                    Json = "Disconnected"
                });
                Console.WriteLine($"client [{client.Key}] socket shutdown.");
                client.Value.Shutdown(SocketShutdown.Both);
                client.Value.Close();
            }

            _flushCommands.Stop();

            _listener.Stop();
            Console.WriteLine($"TCP Server stopped.");

            _thread.Abort();
        }

        private void SendPacketOnSocket(Socket socket, Network.Message message)
        {
            var jsonObj = JsonConvert.SerializeObject(message);
            var encoder = new ASCIIEncoding();
            var mess = encoder.GetBytes(jsonObj + "|");

            object tmpLock = new object();
            int offset = 0;
            int sent = 0;

            do
            {
                try
                {
                    sent = socket.Send(mess, offset, (mess.Length - offset <= 256 ? mess.Length - offset : 256), SocketFlags.None);
                    offset += sent;
                }
                catch (SocketException sockE)
                {
                    Console.WriteLine($"Socket is not connected : [{sockE.Message}]");
                }
            } while (sent == 256);
        }

        private void AddPingerToClient(Guid clientId)
        {
            AddCommand(new Commands.CommandArgs
            {
                Args = new string[0],
                ClientId = clientId,
                CommandCode = Network.CommandCodes.Ping,
                IsValid = true
            });
        }

        private void FlushCommands()
        {
            while (_commandManager.HasCommandLeft)
            {
                Commands.CommandResult ret = _commandManager.ExecuteNextCommand();

                if (!ret.Succeeded)
                {
                    Console.WriteLine("[ERROR] - Command failed to execute.");
                    return;
                }

                if (!Guid.Empty.Equals(ret.ClientId))
                {
                    _responses.Add(ret);
                }
            }
        }

        public bool IsRunning { get { return _isRunning; } }

        public void AddCommand(Commands.CommandArgs args)
        {
            var command = CreateCommand(args);

            if (command != null)
            {
                if (_isRunning)
                    _commandManager.SendCommand(command);
            }
        }

        private Commands.ACommand CreateCommand(Commands.CommandArgs args) // TODO : Logs errors.
        {
            if (_delegates.ContainsKey(args.CommandCode) && _commandArgCount.ContainsKey(args.CommandCode))
            {
                if (args.Args.Length >= _commandArgCount[args.CommandCode])
                {
                    return _delegates[args.CommandCode]?.Invoke(args);
                }
            }

            return null;
        }

        private Commands.CommandResult TakeResponse()
        {
            if (_responses.Count > 0)
            {
                var ret = _responses[0];
                _responses.Remove(ret);
                return ret;
            }

            return new Commands.CommandResult
            {
                Succeeded = false,
                ClientId = Guid.Empty
            };
        }

        public static Commands.CommandArgs ToCommandArgs(Guid clientId, Network.Message message)
        {
            Commands.CommandArgs retVal = new Commands.CommandArgs
            {
                ClientId = clientId,
                CommandCode = message.Code,
                IsValid = ClientsManager.Instance.IsAuth(clientId, message.Token),
                Args = new string[0]
            };

            string[] args = new string[0];

            if (!string.IsNullOrEmpty(message.Json))
            {
                args = JsonConvert.DeserializeObject<string[]>(message.Json);
            }

            switch (message.Code)
            {
                case Network.CommandCodes.Ping:
                    retVal.IsValid = true;
                    break;
                case Network.CommandCodes.Client_Authenticate:
                    retVal.IsValid = true;
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;
                case Network.CommandCodes.Client_ServerList:
                    break;
                case Network.CommandCodes.Client_AnnounceGameConnection:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Client_AnnounceDisconnection:
                    break;
                default:
                    retVal.IsValid = false;
                    break;
            }

            return retVal;
        }
    }
}
