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

        private DataModels.Config _config;

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

            _responses = new List<Commands.CommandResult>();

            SetupDelegates();
        }

        public void Start()
        {
            var configId = Guid.Parse(ConfigurationManager.AppSettings.Get("ConfigId"));
            _config = DataRepositories.ConfigRepository.GetById(configId);

            if (_config == null || (_config != null && _config.Online))
            {
                Console.WriteLine("Wrong Config Id or Server already running... Specify a valid GameServer Id present in DB and a server isnt already running.");
                return;
            }

            _config.Online = true;
            DataRepositories.ConfigRepository.Update(_config);

            Console.WriteLine($"Starting TCPListener at address : {_config.Host}:{_config.Port} ...");

            _listener = new TcpListener(IPAddress.Parse(_config.Host), _config.Port);

            _thread.Start();

            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;

            _config.Online = false;
            DataRepositories.ConfigRepository.Update(_config);
        }

        private void SetupDelegates()
        {
            _commandArgCount = new Dictionary<int, int>();
            _delegates = new Dictionary<int, Func<Commands.CommandArgs, Commands.ACommand>>();

            _commandArgCount.Add(Network.CommandCodes.Ping, 0);
            _delegates.Add(Network.CommandCodes.Ping, (args) => { return new Commands.PingCommand(args); });

            SetupServerDelegates();
            SetupListingDelegates();
            SetupPlayerDelegates();
            SetupDungeonDelegates();
        }

        private void SetupServerDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Client_Authenticate, 1);
            _commandArgCount.Add(Network.CommandCodes.Server.SoulList, 0);
            _commandArgCount.Add(Network.CommandCodes.Server.CreateSoul, 1);
            _commandArgCount.Add(Network.CommandCodes.Server.ConnectSoul, 1);

            _delegates.Add(Network.CommandCodes.Client_Authenticate, (args) => { return new Commands.Server.AuthenticateCommand(args); });
            _delegates.Add(Network.CommandCodes.Server.SoulList, (args) => { return new Commands.Server.SoulListCommand(args); });
            _delegates.Add(Network.CommandCodes.Server.CreateSoul, (args) => { return new Commands.Server.CreateSoulCommand(args, _config); });
            _delegates.Add(Network.CommandCodes.Server.ConnectSoul, (args) => { return new Commands.Server.ConnectSoulCommand(args, _config); });
        }

        private void SetupListingDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Listing.Armor, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Consumable, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Bag, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Book, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Monster, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Dungeon, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Market, 0);

            _delegates.Add(Network.CommandCodes.Listing.Armor, (args) => { return new Commands.Listing.ArmorCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Consumable, (args) => { return new Commands.Listing.ConsumableCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Bag, (args) => { return new Commands.Listing.BagCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Book, (args) => { return new Commands.Listing.BookCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Monster, (args) => { return new Commands.Listing.MonsterCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Dungeon, (args) => { return new Commands.Listing.DungeonCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Market, (args) => { return new Commands.Listing.MarketCommand(args); });
        }

        private void SetupPlayerDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Player.SoulDatas, 0);
            _commandArgCount.Add(Network.CommandCodes.Player.CurrentSoulDatas, 0);
            _commandArgCount.Add(Network.CommandCodes.Player.BuyMarketItem, 3);
            _commandArgCount.Add(Network.CommandCodes.Player.EquipItem, 1);
            _commandArgCount.Add(Network.CommandCodes.Player.UseSpiritPills, 2);
            _commandArgCount.Add(Network.CommandCodes.Player.LearnSkill, 2);

            _delegates.Add(Network.CommandCodes.Player.SoulDatas, (args) => { return new Commands.Player.SoulDatasCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.CurrentSoulDatas, (args) => { return new Commands.Player.CurrentSoulDatasCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.BuyMarketItem, (args) => { return new Commands.Player.BuyMarketItemCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.EquipItem, (args) => { return new Commands.Player.EquipItemCommand(args, _config); });
            _delegates.Add(Network.CommandCodes.Player.UseSpiritPills, (args) => { return new Commands.Player.UseSpiritPillsCommand(args, _config); });
            _delegates.Add(Network.CommandCodes.Player.LearnSkill, (args) => { return new Commands.Player.LearnSkillCommand(args); });
        }

        private void SetupDungeonDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Dungeon.EnterRoom, 1);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.UpdateRoom, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.Exit, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.EnemyTurn, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.UseSkill, 2);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.UseConsumable, 2);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.LootRoom, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.LeaveFinishedRoom, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.DoNothingTurn, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.BuyShopItem, 2);

            _delegates.Add(Network.CommandCodes.Dungeon.EnterRoom, (args) => { return new Commands.Dungeon.EnterRoomCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.UpdateRoom, (args) => { return new Commands.Dungeon.UpdateRoomCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.Exit, (args) => { return new Commands.Dungeon.ExitCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.EnemyTurn, (args) => { return new Commands.Dungeon.EnemyTurnCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.UseSkill, (args) => { return new Commands.Dungeon.UseSkillCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.UseConsumable, (args) => { return new Commands.Dungeon.UseConsumableCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.LootRoom, (args) => { return new Commands.Dungeon.LootRoomCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.LeaveFinishedRoom, (args) => { return new Commands.Dungeon.LeaveFinishedRoomCommand(args, _config); });
            _delegates.Add(Network.CommandCodes.Dungeon.DoNothingTurn, (args) => { return new Commands.Dungeon.DoNothingTurnCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.BuyShopItem, (args) => { return new Commands.Dungeon.BuyShopItemCommand(args); });
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

                List<Guid> disconnectedClients = new List<Guid>();
                foreach (var client in ClientsManager.Instance.Clients)
                {
                    if (ClientsManager.Instance.GetPing(client.Key) > 10)
                    {
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
                case Network.CommandCodes.Ping:
                    retVal.IsValid = true;
                    break;
                case Network.CommandCodes.Client_Authenticate:
                    retVal.IsValid = true;
                    retVal.Args = new string[1] { args[0] };
                    break;

                // SERVER
                case Network.CommandCodes.Server.CreateSoul:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Server.SoulList:
                    break;
                case Network.CommandCodes.Server.ConnectSoul:
                    retVal.Args = new string[1] { args[0] };
                    break;

                // PLAYER
                case Network.CommandCodes.Player.SoulDatas:
                    break;
                case Network.CommandCodes.Player.CurrentSoulDatas:
                    break;
                case Network.CommandCodes.Player.BuyMarketItem:
                    retVal.Args = new string[3] { args[0], args[1], args[2] };
                    break;
                case Network.CommandCodes.Player.EquipItem:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Player.UseSpiritPills:
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;
                case Network.CommandCodes.Player.LearnSkill:
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;

                // LISTING
                case Network.CommandCodes.Listing.Armor:
                    break;
                case Network.CommandCodes.Listing.Consumable:
                    break;
                case Network.CommandCodes.Listing.Bag:
                    break;
                case Network.CommandCodes.Listing.Book:
                    break;
                case Network.CommandCodes.Listing.Monster:
                    break;
                case Network.CommandCodes.Listing.Dungeon:
                    break;
                case Network.CommandCodes.Listing.Market:
                    break;

                // DUNGEON
                case Network.CommandCodes.Dungeon.EnterRoom:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Dungeon.UpdateRoom:
                    break;
                case Network.CommandCodes.Dungeon.Exit:
                    break;
                case Network.CommandCodes.Dungeon.EnemyTurn:
                    break;
                case Network.CommandCodes.Dungeon.UseSkill:
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;
                case Network.CommandCodes.Dungeon.UseConsumable:
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;
                case Network.CommandCodes.Dungeon.LootRoom:
                    break;
                case Network.CommandCodes.Dungeon.LeaveFinishedRoom:
                    break;
                case Network.CommandCodes.Dungeon.DoNothingTurn:
                    break;
                case Network.CommandCodes.Dungeon.BuyShopItem:
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;

                default:
                    retVal.IsValid = false;
                    break;
            }

            return retVal;
        }
    }
}
