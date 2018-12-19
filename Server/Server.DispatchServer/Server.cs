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

        private Object _commandManagerLock = new Object();
        private Object _responsesLock = new Object();

        public Server()
        {
            ThreadStart del = new ThreadStart(Run);
            _thread = new Thread(del);

            _flushCommands = new System.Timers.Timer(10);
            _flushCommands.Elapsed += (sender, e) =>
            {
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

            Log.Instance.Write(Log.Level.Infos, $"Starting TCPListener at address : {host}:{port} ...");
            Console.WriteLine($"Starting TCPListener at address : {host}:{port} ...");

            _listener = new TcpListener(IPAddress.Parse(host), port);

            _thread.Start();

            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void SendMessageToAll(string message)
        {
            foreach (var client in ClientsManager.Instance.Clients)
            {
                _responses.Add(new Commands.CommandResult
                {
                    ClientId = client.Key,
                    ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.GlobalMessage,
                        Json = message,
                        Success = true
                    },
                    Succeeded = true
                });
            }
        }

        private void SetupClientDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Ping, 0);

            _commandArgCount.Add(Network.CommandCodes.Client_CreateAccount, 0);
            _commandArgCount.Add(Network.CommandCodes.Client_Authenticate, 1);
            _commandArgCount.Add(Network.CommandCodes.Client_ServerList, 0);
            _commandArgCount.Add(Network.CommandCodes.Client_AnnounceGameConnection, 1);
            _commandArgCount.Add(Network.CommandCodes.Client_AnnounceDisconnection, 0);
            _commandArgCount.Add(Network.CommandCodes.Client_RetrieveAccount, 1);

            _delegates.Add(Network.CommandCodes.Ping, (args) => { return new Commands.PingCommand(args); });

            _delegates.Add(Network.CommandCodes.Client_CreateAccount, (args) => { return new Commands.ClientCreateAccountCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_Authenticate, (args) => { return new Commands.ClientAuthenticateCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_ServerList, (args) => { return new Commands.ClientServerListCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_AnnounceGameConnection, (args) => { return new Commands.ClientAnnounceGameConnectionCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_AnnounceDisconnection, (args) => { return new Commands.ClientAnnounceDisconnectionCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_RetrieveAccount, (args) => { return new Commands.ClientRetrieveAccountCommand(args); });
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

                    // TODO : Pinger not used in this server anymore ?
                    //AddPingerToClient(clientId);

                    Log.Instance.Write(Log.Level.Infos, $"New client [{newClient.RemoteEndPoint}] given id ({clientId})");
                }

                List<Guid> disconnectedClients = ClientsManager.Instance.DisconnectedClientList;
                foreach (var client in ClientsManager.Instance.Clients)
                {
                    // TODO : Pinger not needed, i think ?
                    //if (ClientsManager.Instance.GetPing(client.Key) > 10)
                    //{
                    //    Log.Instance.Write(Log.Level.Warning, $"Client [{client.Value.RemoteEndPoint}],id ({client.Key}) timed out");
                    //    Console.WriteLine($"{client.Key} Timed out.");
                    //    disconnectedClients.Add(client.Key);
                    //    continue;
                    //}

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
                                if (commandArgs.CommandCode >= 0)
                                    Log.Instance.Write(Log.Level.Infos, $"Received [{commandArgs.CommandCode}] from [{socket.RemoteEndPoint}] : ({string.Join(", ", commandArgs.Args)})");

                                ClientsManager.Instance.Ping(commandArgs.ClientId);

                                AddCommand(commandArgs);
                            }
                            else
                            {
                                Log.Instance.Write(Log.Level.Warning, $"Wrong command Received [{commandArgs.CommandCode}] from [{socket.RemoteEndPoint}] : ({string.Join(", ", commandArgs.Args)})");
                            }
                        }
                    }
                }

                lock (_responsesLock)
                {
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
                                Log.Instance.Write(Log.Level.Error, $"Tried to send packet to {response.ClientId} but exception : {sockEx.Message}");
                                Console.WriteLine($"Server is not online(1)... [{sockEx.Message}]");
                                ClientsManager.Instance.DisconnectClient(response.ClientId);
                            }
                        }
                    }
                }

                foreach (var disc in disconnectedClients)
                {
                    var socket = ClientsManager.Instance.Clients[disc];

                    if (ClientsManager.Instance.RemoveClient(disc))
                    {
                        Log.Instance.Write(Log.Level.Warning, $"Client [{socket.RemoteEndPoint}] closed");
                        Console.WriteLine($"Client [{socket.RemoteEndPoint}] closed");
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                }
            }

            Log.Instance.Write(Log.Level.Infos, $"Stopping server.");

            foreach (var client in ClientsManager.Instance.Clients)
            {
                Log.Instance.Write(Log.Level.Infos, $"Disconnecting {client.Value.RemoteEndPoint}");

                try
                {
                    SendPacketOnSocket(client.Value, new Network.Message
                    {
                        Code = Network.CommandCodes.Client_AnnounceDisconnection,
                        Success = true,
                        Json = "Disconnected"
                    });
                }
                catch (SocketException sockEx)
                {
                    Log.Instance.Write(Log.Level.Error, $"Tried to send packet to {client.Key} but exception : {sockEx.Message}");
                    Console.WriteLine($"Server is not online(1)... [{sockEx.Message}]");
                    ClientsManager.Instance.DisconnectClient(client.Key);
                }

                Console.WriteLine($"client [{client.Key}] socket shutdown.");
                client.Value.Shutdown(SocketShutdown.Both);
                client.Value.Close();
            }

            _flushCommands.Stop();

            _listener.Stop();
            Console.WriteLine($"TCP Server stopped.");

            _thread.Abort();
            Log.Instance.Write(Log.Level.Infos, $"Server Stopped.");
        }

        private void SendPacketOnSocket(Socket socket, Network.Message message)
        {
            var jsonObj = JsonConvert.SerializeObject(message);
            var encoder = new ASCIIEncoding();
            var mess = encoder.GetBytes(jsonObj + "|");

            if (message.Code >= 0)
                Log.Instance.Write(Log.Level.Infos, $"Sending[{message.Code}] to {socket.RemoteEndPoint} : {jsonObj}");

            object tmpLock = new object();
            int offset = 0;
            int sent = 0;

            do
            {
                sent = socket.Send(mess, offset, (mess.Length - offset <= 256 ? mess.Length - offset : 256), SocketFlags.None);
                offset += sent;
            } while (sent == 256);
        }

        private void AddPingerToClient(Guid clientId)
        {
            Log.Instance.Write(Log.Level.Infos, $"Adding pinger to [{clientId}]");

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
            lock (_commandManagerLock)
            {
                while (_commandManager.HasCommandLeft)
                {
                    Commands.CommandResult ret = _commandManager.ExecuteNextCommand();

                    if (!ret.Succeeded)
                    {
                        Log.Instance.Write(Log.Level.Warning, $"Couldn't execute command[{ret.ClientResponse.Code}] for {ret.ClientId}");
                        Console.WriteLine("[ERROR] - Command failed to execute.");
                        return;
                    }

                    if (!Guid.Empty.Equals(ret.ClientId))
                    {
                        if (ret.ClientResponse.Code >= 0)
                            Log.Instance.Write(Log.Level.Infos, $"Add response[{ret.ClientId}]: {ret.ClientResponse.Code}, {ret.ClientResponse.Json}");

                        lock (_responsesLock)
                        {
                            _responses.Add(ret);
                        }
                    }
                }
            }
        }

        public bool IsRunning { get { return _isRunning; } }

        public void AddCommand(Commands.CommandArgs args)
        {
            if (args.CommandCode >= 0)
                Log.Instance.Write(Log.Level.Infos, $"{args.ClientId} creating command[{args.CommandCode}]->({string.Join(", ", args.Args)})");

            var command = CreateCommand(args);

            if (command != null)
            {
                if (_isRunning)
                {
                    lock (_commandManagerLock)
                    {
                        _commandManager.SendCommand(command);
                    }
                }
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

            Log.Instance.Write(Log.Level.Warning, $"Taking response but stack is empty ?..");
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
                case Network.CommandCodes.Client_CreateAccount:
                    retVal.IsValid = true;
                    break;
                case Network.CommandCodes.Client_Authenticate:
                    retVal.IsValid = true;
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Client_ServerList:
                    break;
                case Network.CommandCodes.Client_AnnounceGameConnection:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Client_AnnounceDisconnection:
                    break;
                case Network.CommandCodes.Client_RetrieveAccount:
                    retVal.IsValid = true;
                    retVal.Args = new string[1] { args[0] };
                    break;
                default:
                    Log.Instance.Write(Log.Level.Warning, $"Message code {message.Code} is not accepted by the server.");
                    retVal.IsValid = false;
                    break;
            }

            return retVal;
        }
    }
}
