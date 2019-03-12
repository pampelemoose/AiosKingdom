using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

        private DataModels.Town _town;

        private Object _commandManagerLock = new Object();
        private Object _responsesLock = new Object();

        private Thread _marketThread;

        private Dictionary<IPAddress, int> _failedCounts;
        private List<IPAddress> _blockedIps;

        public Server()
        {
            ThreadStart del = new ThreadStart(Run);
            _thread = new Thread(del);

            ThreadStart mar = new ThreadStart(RunMarket);
            _marketThread = new Thread(mar);

            _flushCommands = new System.Timers.Timer(10);
            _flushCommands.Elapsed += (sender, e) =>
            {
                FlushCommands();
            };
            _flushCommands.AutoReset = true;
            _flushCommands.Enabled = true;

            _commandManager = new CommandManager();

            _responses = new List<Commands.CommandResult>();

            _failedCounts = new Dictionary<IPAddress, int>();
            _blockedIps = new List<IPAddress>();

            SetupDelegates();
        }

        public void Start()
        {
            var townId = Guid.Parse(ConfigurationManager.AppSettings.Get("TownId"));
            _town = DataRepositories.TownRepository.GetById(townId);

            if (_town == null || (_town != null && _town.Online))
            {
                Log.Instance.Write(Log.Level.Error, $"Town id {townId} isn't pointing to any config or is not offline.");
                Console.WriteLine("Wrong Config Id or Server already running... Specify a valid GameServer Id present in DB and a server isnt already running.");
                return;
            }

            _town.Online = true;
            DataRepositories.TownRepository.Update(_town);

            Log.Instance.Write(Log.Level.Infos, $"Starting TCPListener at address : {_town.Host}:{_town.Port} ...");
            Console.WriteLine($"Starting TCPListener at address : {_town.Host}:{_town.Port} ...");

            DataManager.Instance.Initialize(_town);

            _listener = new TcpListener(IPAddress.Parse(_town.Host), _town.Port);

            _thread.Start();
            _marketThread.Start();

            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;

            _town.Online = false;
            DataRepositories.TownRepository.Update(_town);
        }

        public void SendMessageToAll(int code, string message)
        {
            foreach (var client in ClientsManager.Instance.Clients)
            {
                _responses.Add(new Commands.CommandResult
                {
                    ClientId = client.Key,
                    ClientResponse = new Network.Message
                    {
                        Code = code,
                        Json = message,
                        Success = true
                    },
                    Succeeded = true
                });
            }
        }

        private bool _refuseNewConnection = false;
        public void RefuseNewConnection()
        {
            _refuseNewConnection = true;
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
            SetupJobDelegates();
        }

        private void SetupServerDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Game_Authenticate, 1);
            _commandArgCount.Add(Network.CommandCodes.Server.SoulList, 0);
            _commandArgCount.Add(Network.CommandCodes.Server.CreateSoul, 1);
            _commandArgCount.Add(Network.CommandCodes.Server.ConnectSoul, 1);
            _commandArgCount.Add(Network.CommandCodes.Server.DisconnectSoul, 0);

            _delegates.Add(Network.CommandCodes.Game_Authenticate, (args) => { return new Commands.Server.AuthenticateCommand(args); });
            _delegates.Add(Network.CommandCodes.Server.SoulList, (args) => { return new Commands.Server.SoulListCommand(args); });
            _delegates.Add(Network.CommandCodes.Server.CreateSoul, (args) => { return new Commands.Server.CreateSoulCommand(args, _town); });
            _delegates.Add(Network.CommandCodes.Server.ConnectSoul, (args) => { return new Commands.Server.ConnectSoulCommand(args, _town); });
            _delegates.Add(Network.CommandCodes.Server.DisconnectSoul, (args) => { return new Commands.Server.DisconnectSoulCommand(args); });
        }

        private void SetupListingDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Listing.Item, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Book, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Monster, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Dungeon, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Market, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.SpecialsMarket, 0);
            _commandArgCount.Add(Network.CommandCodes.Listing.Recipes, 0);

            _delegates.Add(Network.CommandCodes.Listing.Item, (args) => { return new Commands.Listing.ItemCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Book, (args) => { return new Commands.Listing.BookCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Monster, (args) => { return new Commands.Listing.MonsterCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Dungeon, (args) => { return new Commands.Listing.DungeonCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Market, (args) => { return new Commands.Listing.MarketCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.SpecialsMarket, (args) => { return new Commands.Listing.SpecialMarketCommand(args); });
            _delegates.Add(Network.CommandCodes.Listing.Recipes, (args) => { return new Commands.Listing.RecipeCommand(args); });
        }

        private void SetupPlayerDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Player.CurrentSoulDatas, 0);
            _commandArgCount.Add(Network.CommandCodes.Player.Market_PlaceOrder, 1);
            _commandArgCount.Add(Network.CommandCodes.Player.EquipItem, 1);
            _commandArgCount.Add(Network.CommandCodes.Player.SellItem, 1);
            _commandArgCount.Add(Network.CommandCodes.Player.UseSpiritPills, 2);
            _commandArgCount.Add(Network.CommandCodes.Player.LearnSkill, 1);
            _commandArgCount.Add(Network.CommandCodes.Player.LearnTalent, 1);

            _commandArgCount.Add(Network.CommandCodes.Player.Currencies, 0);
            _commandArgCount.Add(Network.CommandCodes.Player.Inventory, 0);
            _commandArgCount.Add(Network.CommandCodes.Player.Knowledges, 0);
            _commandArgCount.Add(Network.CommandCodes.Player.Equipment, 0);
            _commandArgCount.Add(Network.CommandCodes.Player.AdventureUnlocked, 0);

            _delegates.Add(Network.CommandCodes.Player.CurrentSoulDatas, (args) => { return new Commands.Player.CurrentSoulDatasCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.Market_PlaceOrder, (args) => { return new Commands.Player.MarketPlaceOrderCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.EquipItem, (args) => { return new Commands.Player.EquipItemCommand(args, _town); });
            _delegates.Add(Network.CommandCodes.Player.SellItem, (args) => { return new Commands.Player.SellItemCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.UseSpiritPills, (args) => { return new Commands.Player.UseSpiritPillsCommand(args, _town); });
            _delegates.Add(Network.CommandCodes.Player.LearnSkill, (args) => { return new Commands.Player.LearnSkillCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.LearnTalent, (args) => { return new Commands.Player.LearnTalentCommand(args); });

            _delegates.Add(Network.CommandCodes.Player.Currencies, (args) => { return new Commands.Player.CurrenciesCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.Inventory, (args) => { return new Commands.Player.InventoryCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.Knowledges, (args) => { return new Commands.Player.KnowledgeCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.Equipment, (args) => { return new Commands.Player.EquipmentCommand(args); });
            _delegates.Add(Network.CommandCodes.Player.AdventureUnlocked, (args) => { return new Commands.Player.AdventureUnlockedCommand(args); });
        }

        private void SetupDungeonDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Dungeon.Enter, 2);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.EnterRoom, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.UpdateRoom, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.Exit, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.EnemyTurn, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.UseSkill, 2);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.UseConsumable, 2);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.GetLoots, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.LootItem, 1);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.LeaveFinishedRoom, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.DoNothingTurn, 0);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.BuyShopItem, 2);
            _commandArgCount.Add(Network.CommandCodes.Dungeon.PlayerRest, 0);

            _delegates.Add(Network.CommandCodes.Dungeon.Enter, (args) => { return new Commands.Dungeon.EnterCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.EnterRoom, (args) => { return new Commands.Dungeon.EnterRoomCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.UpdateRoom, (args) => { return new Commands.Dungeon.UpdateRoomCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.Exit, (args) => { return new Commands.Dungeon.ExitCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.EnemyTurn, (args) => { return new Commands.Dungeon.EnemyTurnCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.UseSkill, (args) => { return new Commands.Dungeon.UseSkillCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.UseConsumable, (args) => { return new Commands.Dungeon.UseConsumableCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.GetLoots, (args) => { return new Commands.Dungeon.GetLootsCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.LootItem, (args) => { return new Commands.Dungeon.LootDungeonItemCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.LeaveFinishedRoom, (args) => { return new Commands.Dungeon.LeaveFinishedRoomCommand(args, _town); });
            _delegates.Add(Network.CommandCodes.Dungeon.DoNothingTurn, (args) => { return new Commands.Dungeon.DoNothingTurnCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.BuyShopItem, (args) => { return new Commands.Dungeon.BuyShopItemCommand(args); });
            _delegates.Add(Network.CommandCodes.Dungeon.PlayerRest, (args) => { return new Commands.Dungeon.PlayerRestCommand(args); });
        }

        private void SetupJobDelegates()
        {
            _commandArgCount.Add(Network.CommandCodes.Job.Get, 0);
            _commandArgCount.Add(Network.CommandCodes.Job.Learn, 1);
            _commandArgCount.Add(Network.CommandCodes.Job.Craft, 2);

            _delegates.Add(Network.CommandCodes.Job.Get, (args) => { return new Commands.Jobs.GetCommand(args); });
            _delegates.Add(Network.CommandCodes.Job.Learn, (args) => { return new Commands.Jobs.LearnCommand(args); });
            _delegates.Add(Network.CommandCodes.Job.Craft, (args) => { return new Commands.Jobs.CraftCommand(args); });
        }

        private void Run()
        {
            _listener.Start();
            Console.WriteLine($"TCP Server is running.");

            _flushCommands.Start();

            while (_isRunning)
            {
                if (_listener.Pending() && !_refuseNewConnection)
                {
                    Socket newClient = _listener.AcceptSocket();
                    var endPoint = newClient.RemoteEndPoint as IPEndPoint;

                    if (!_blockedIps.Contains(endPoint.Address))
                    {
                        var clientId = Guid.NewGuid();
                        ClientsManager.Instance.AddClient(clientId, newClient);

                        // TODO : Pinger only in PVP, or check that players are both still live
                        //AddPingerToClient(clientId);

                        Log.Instance.Write(Log.Level.Infos, $"New client [{newClient.RemoteEndPoint}] given id ({clientId})");
                    }
                }

                List<Guid> disconnectedClients = ClientsManager.Instance.DisconnectedClientList;
                foreach (var client in ClientsManager.Instance.Clients)
                {
                    // TODO : Pinger needed only for PVP
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

                            try
                            {
                                var json = JsonConvert.DeserializeObject<Network.Message>(message);
                                var commandArgs = Server.ToCommandArgs(client.Key, json);

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
                            catch (JsonReaderException jsonReaderException)
                            {
                                Log.Instance.Write(Log.Level.Error, $"Received {message} but exception : {jsonReaderException.Message}");
                                AddIpWarning(client.Key);
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

                    if (SoulManager.Instance.DisconnectSoul(disc))
                    {
                        Console.WriteLine($"{disc} Soul disconnected.");
                    }

                    if (ClientsManager.Instance.RemoveClient(disc))
                    {
                        Log.Instance.Write(Log.Level.Warning, $"Client [{socket.RemoteEndPoint}] closed");
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                }
            }

            Log.Instance.Write(Log.Level.Infos, $"Stopping server.");

            foreach (var client in ClientsManager.Instance.Clients)
            {
                Log.Instance.Write(Log.Level.Infos, $"Disconnecting {client.Value.RemoteEndPoint}");
                if (SoulManager.Instance.DisconnectSoul(client.Key))
                {
                    try
                    {
                        SendPacketOnSocket(client.Value, new Network.Message
                        {
                            Code = Network.CommandCodes.Server.DisconnectSoul,
                            Success = true,
                            Json = "Soul Disconnected"
                        });
                    }
                    catch (SocketException sockEx)
                    {
                        Log.Instance.Write(Log.Level.Error, $"Tried to send packet to {client.Key} but exception : {sockEx.Message}");
                        Console.WriteLine($"Server is not online(2)... [{sockEx.Message}]");
                        ClientsManager.Instance.DisconnectClient(client.Key);
                    }
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

            Log.Instance.Write(Log.Level.Infos, $"Server Stopped.");
        }

        private void SendPacketOnSocket(Socket socket, Network.Message message)
        {
            var jsonObj = JsonConvert.SerializeObject(message);
            var encoder = new ASCIIEncoding();
            var mess = encoder.GetBytes(jsonObj + "|");

            if (message.Code >= 0)
                Log.Instance.Write(Log.Level.Infos, $"Sending[{message.Code}] to {socket.RemoteEndPoint} : {jsonObj}");

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
                        AddIpWarning(ret.ClientId);

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
                case Network.CommandCodes.Game_Authenticate:
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
                case Network.CommandCodes.Server.DisconnectSoul:
                    break;

                // PLAYER
                case Network.CommandCodes.Player.CurrentSoulDatas:
                    break;
                case Network.CommandCodes.Player.Market_PlaceOrder:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Player.EquipItem:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Player.SellItem:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Player.UseSpiritPills:
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;
                case Network.CommandCodes.Player.LearnSkill:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Player.LearnTalent:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Player.Currencies:
                    break;
                case Network.CommandCodes.Player.Inventory:
                    break;
                case Network.CommandCodes.Player.Knowledges:
                    break;
                case Network.CommandCodes.Player.Equipment:
                    break;
                case Network.CommandCodes.Player.AdventureUnlocked:
                    break;

                // LISTING
                case Network.CommandCodes.Listing.Item:
                    break;
                case Network.CommandCodes.Listing.Book:
                    break;
                case Network.CommandCodes.Listing.Monster:
                    break;
                case Network.CommandCodes.Listing.Dungeon:
                    break;
                case Network.CommandCodes.Listing.Market:
                    break;
                case Network.CommandCodes.Listing.SpecialsMarket:
                    break;
                case Network.CommandCodes.Listing.Recipes:
                    break;

                // DUNGEON
                case Network.CommandCodes.Dungeon.Enter:
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;
                case Network.CommandCodes.Dungeon.EnterRoom:
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
                case Network.CommandCodes.Dungeon.GetLoots:
                    break;
                case Network.CommandCodes.Dungeon.LootItem:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Dungeon.LeaveFinishedRoom:
                    break;
                case Network.CommandCodes.Dungeon.DoNothingTurn:
                    break;
                case Network.CommandCodes.Dungeon.BuyShopItem:
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;
                case Network.CommandCodes.Dungeon.PlayerRest:
                    break;

                // JOB
                case Network.CommandCodes.Job.Get:
                    break;
                case Network.CommandCodes.Job.Learn:
                    retVal.Args = new string[1] { args[0] };
                    break;
                case Network.CommandCodes.Job.Craft:
                    retVal.Args = new string[2] { args[0], args[1] };
                    break;

                default:
                    Log.Instance.Write(Log.Level.Warning, $"Message code {message.Code} is not accepted by the server.");
                    retVal.IsValid = false;
                    break;
            }

            return retVal;
        }

        private void RunMarket()
        {
            Market.Instance.Initialize(_town);

            while (_isRunning)
            {
                if (Market.Instance.OrderCount > 0)
                {
                    lock (_responsesLock)
                    {
                        _responses.Add(Market.Instance.ProcessOrder());
                    }
                }
            }

            _marketThread.Abort();
        }

        private void AddIpWarning(Guid clientId)
        {
            var socket = ClientsManager.Instance.Clients[clientId];
            var endPoint = socket.RemoteEndPoint as IPEndPoint;

            if (!_failedCounts.ContainsKey(endPoint.Address))
            {
                _failedCounts.Add(endPoint.Address, 0);
            }

            _failedCounts[endPoint.Address] += 1;

            if (_failedCounts[endPoint.Address] > 100)
            {
                ClientsManager.Instance.DisconnectClient(clientId);
                _blockedIps.Add(endPoint.Address);

                Log.Instance.Write(Log.Level.Warning, $"{clientId} [{endPoint.Address}] blocked until next flush.");
            }
        }
    }
}
