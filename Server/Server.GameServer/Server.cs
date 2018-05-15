using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server.GameServer
{
    public class Server
    {
        private Thread _thread;
        private TcpListener _listener;
        private bool _isRunning;

        private System.Timers.Timer _flushCommands;

        private CommandManager _commandManager;
        private Dictionary<int, Func<Commands.CommandArgs, Commands.ACommand>> _delegates;
        private Dictionary<int, int> _commandArgCount;
        private List<Commands.CommandResult> _responses;

        private DataModels.GameServer _config;

        public Server()
        {
            ThreadStart del = new ThreadStart(Run);
            _thread = new Thread(del);

            _flushCommands = new System.Timers.Timer(1000);
            _flushCommands.Elapsed += (sender, e) =>
            {
                FlushCommands();
            };
            _flushCommands.AutoReset = true;
            _flushCommands.Enabled = true;

            _commandManager = new CommandManager();

            _commandArgCount = new Dictionary<int, int>();
            _delegates = new Dictionary<int, Func<Commands.CommandArgs, Commands.ACommand>>();
            _responses = new List<Commands.CommandResult>();

            SetupClientDelegates();
        }

        public void Start()
        {
            var configId = Guid.Parse(ConfigurationManager.AppSettings.Get("ConfigId"));
            _config = DataRepositories.GameServerRepository.GetById(configId);

            if (_config == null || (_config != null && _config.Online))
            {
                Console.WriteLine("Wrong Config Id or Server already running... Specify a valid GameServer Id present in DB and a server isnt already running.");
                return;
            }

            _config.Online = true;
            DataRepositories.GameServerRepository.Update(_config);

            Console.WriteLine($"Starting TCPListener at address : {_config.Host}:{_config.Port} ...");

            _listener = new TcpListener(IPAddress.Parse(_config.Host), _config.Port);

            _thread.Start();

            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;

            _config.Online = false;
            DataRepositories.GameServerRepository.Update(_config);
        }

        private void SetupClientDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Ping, 0);

            _commandArgCount.Add(Network.CommandCodes.Client_Authenticate, 1);
            _commandArgCount.Add(Network.CommandCodes.Client_SoulList, 0);
            _commandArgCount.Add(Network.CommandCodes.Client_CreateSoul, 1);
            _commandArgCount.Add(Network.CommandCodes.Client_ConnectSoul, 1);

            _commandArgCount.Add(Network.CommandCodes.Client_SoulDatas, 0);
            _commandArgCount.Add(Network.CommandCodes.Client_CurrentSoulDatas, 0);

            _commandArgCount.Add(Network.CommandCodes.Client_MarketList, 0);
            _commandArgCount.Add(Network.CommandCodes.Client_BuyMarketItem, 1);

            _commandArgCount.Add(Network.CommandCodes.ArmorList, 0);

            _delegates.Add(Network.CommandCodes.Ping, (args) => { return new Commands.PingCommand(args); });

            _delegates.Add(Network.CommandCodes.Client_Authenticate, (args) => { return new Commands.ClientAuthenticateCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_SoulList, (args) => { return new Commands.ClientSoulListCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_CreateSoul, (args) => { return new Commands.ClientCreateSoulCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_ConnectSoul, (args) => { return new Commands.ClientConnectSoulCommand(args, _config); });

            _delegates.Add(Network.CommandCodes.Client_SoulDatas, (args) => { return new Commands.ClientSoulDatasCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_CurrentSoulDatas, (args) => { return new Commands.ClientCurrentSoulDatasCommand(args); });

            _delegates.Add(Network.CommandCodes.Client_MarketList, (args) => { return new Commands.ClientMarketListCommand(args); });
            _delegates.Add(Network.CommandCodes.Client_BuyMarketItem, (args) => { return new Commands.ClientBuyMarketItemCommand(args); });

            _delegates.Add(Network.CommandCodes.ArmorList, (args) => { return new Commands.ArmorListCommand(args); });
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

                /*Dictionary<Guid, TimeSpan> updatedPings = new Dictionary<Guid, TimeSpan>();
                foreach (var ping in _pings)
                {
                    var socket = ClientsManager.Instance.Clients[ping.Key];

                    updatedPings.Add(ping.Key, ping.Value);
                    /*if (DateTime.Now.TimeOfDay - ping.Value > TimeSpan.FromSeconds(5))
                    {
                        var pingMess = new Network.Message
                        {
                            Code = Network.ServerCodes.Ping,
                            Json = JsonConvert.SerializeObject((int)(DateTime.Now.TimeOfDay - ping.Value).TotalMilliseconds)
                        };
                        var pingStr = DependencyService.Get<IRMessageEncoder>().GetBytes(JsonConvert.SerializeObject(pingMess));
                        socket.Send(pingStr);
                    }*//*
                    if (DateTime.Now.TimeOfDay - ping.Value > TimeSpan.FromSeconds(10))
                    {
                        disconnectedClients.Add(ping.Key);
                        continue;
                    }


                    int bufferSize = socket.Available;
                    if (bufferSize > 0)
                    {
                        byte[] a = new byte[bufferSize];
                        int receivedSize = socket.Receive(a);
                        var buffer = DependencyService.Get<IRMessageEncoder>().GetString(a);
                        var messages = buffer.Split('|');
                        foreach (var message in messages)
                        {
                            if (string.IsNullOrEmpty(message))
                                continue;

                            var json = JsonConvert.DeserializeObject<Network.Message>(message);
                            var commandArgs = Server.DispatchServer.ToCommandArgs(ping.Key, json);

                            if (commandArgs.IsValid)
                            {
                                _server.AddCommand(ping.Key, commandArgs.CommandCode, commandArgs.Args);
                            }
                            else if (commandArgs.CommandCode == Network.ServerCodes.Pong)
                            {
                                var currentTime = DateTime.Now.TimeOfDay;
                                var diff = currentTime - _pings[ping.Key];
                                updatedPings[ping.Key] = currentTime;
                                if (diff > TimeSpan.FromSeconds(10))
                                {
                                    disconnectedClients.Add(ping.Key);
                                    continue;
                                }

                                var pingMess = new Network.Message
                                {
                                    Code = Network.ClientCodes.Ping,
                                    Json = JsonConvert.SerializeObject((int)diff.TotalMilliseconds)
                                };
                                var pingStr = DependencyService.Get<IRMessageEncoder>().GetBytes(JsonConvert.SerializeObject(pingMess));
                                var sentSize = socket.Send(pingStr); // TODO : Check sent size for missed datas
                            }
                        }
                    }
                }

                _pings = updatedPings;

                _server?.TimeoutPlayers(disconnectedClients);*/

                List<Guid> disconnectedClients = new List<Guid>();
                foreach (var client in ClientsManager.Instance.Clients)
                {
                    if (ClientsManager.Instance.GetPing(client.Key) > 10)
                    {
                        disconnectedClients.Add(client.Key);
                        continue;
                    }

                    /*if (ClientsManager.Instance.GetPing(client.Key) > 8)
                    {
                        AddPingerToClient(client.Key);
                    }*/

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
                            var commandArgs = Server.ToCommandArgs(client.Key, json);

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

                    if (SoulManager.Instance.DisconnectSoul(disc))
                    {
                        Console.WriteLine($"{disc} Soul disconnected.");
                    }

                    if (ClientsManager.Instance.RemoveClient(disc))
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();

                        Console.WriteLine($"{disc} Timed out.");
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

                            var jsonObj = JsonConvert.SerializeObject(response.ClientResponse);
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

                                /*var result = socket.BeginSend(mess, offset, (mess.Length - offset <= 256 ? mess.Length - offset : 256), SocketFlags.None, (asyncResult) =>
                                {
                                    try
                                    {
                                        sent = socket.EndSend(asyncResult);
                                        offset += sent;
                                        Console.WriteLine($"(0)sent[{sent}]");
                                    }
                                    catch (SocketException sockE)
                                    {
                                        Console.WriteLine($"Socket is not connected : [{sockE.Message}]");
                                    }
                                }, null);

                                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                                {
                                    Console.WriteLine($"Socket is not connected..");
                                }*/

                            } while (sent == 256);
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
                if (SoulManager.Instance.DisconnectSoul(client.Key))
                {
                    Console.WriteLine($"{client.Key} Soul disconnected.");
                }

                Console.WriteLine($"client [{client.Key}] socket shutdown.");
                client.Value.Shutdown(SocketShutdown.Both);
                client.Value.Close();
            }

            ClientsManager.Instance.Clear();

            _flushCommands.Stop();

            _listener.Stop();
            Console.WriteLine($"TCP Server stopped.");

            _thread.Abort();
        }

        private void AddPingerToClient(Guid clientId)
        {
            /*var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (sender, e) => {
                AddCommand(new Commands.CommandArgs
                {
                    Args = new string[0],
                    ClientId = clientId,
                    CommandCode = Network.CommandCodes.Ping,
                    IsValid = true
                });
            };
            timer.AutoReset = true;
            timer.Enabled = true;
            _pingers.Add(clientId, timer);*/
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
                /*case Network.ServerCodes.Item_Armor_List_Command:
                    {
                        retVal.CommandCode = Network.ServerCodes.Item_Armor_List_Command;
                        retVal.Args = new string[1] { args[0] };
                    }
                    break;*/
                case Network.CommandCodes.Ping:
                    retVal.IsValid = true;
                    break;
                case Network.CommandCodes.Client_Authenticate:
                    retVal.IsValid = true;
                    retVal.Args = new string[1] { args[0] };
                    break;

                case Network.CommandCodes.Client_CreateSoul:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Client_SoulList:
                    break;
                case Network.CommandCodes.Client_ConnectSoul:
                    retVal.Args = new string[1] { args[0] };
                    break;

                case Network.CommandCodes.Client_SoulDatas:
                    break;
                case Network.CommandCodes.Client_CurrentSoulDatas:
                    break;

                case Network.CommandCodes.Client_MarketList:
                    break;
                case Network.CommandCodes.Client_BuyMarketItem:
                    retVal.Args = new string[1] { args[0] };
                    break;

                case Network.CommandCodes.ArmorList:
                    break;
                default:
                    retVal.IsValid = false;
                    break;
            }

            return retVal;
        }
    }
}
