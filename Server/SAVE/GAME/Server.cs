﻿using Newtonsoft.Json;
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

            _commandManager = new CommandManager();

            _delegates = new Dictionary<int, Func<Commands.CommandArgs, Commands.ACommand>>();
            _commandArgCount = new Dictionary<int, int>();
            _responses = new List<Commands.CommandResult>();

            _commandArgCount.Add(Network.CommandCodes.Gameserver_Connect, 1);
            _commandArgCount.Add(Network.CommandCodes.Gameserver_Infos, 0);

            _delegates.Add(Network.CommandCodes.Gameserver_Connect, (args) => { return new Commands.DispatchServerConnectCommand(args); });
            _delegates.Add(Network.CommandCodes.Gameserver_Infos, (args) => { return new Commands.DispatchServerInfosCommand(args); });
        }

        public void Start()
        {
            string host = ConfigurationManager.AppSettings.Get("Host");
            string portStr = ConfigurationManager.AppSettings.Get("Port");
            int port = int.Parse(portStr);

            Console.WriteLine($"Starting TCPListener at address : {host}.{port} ...");

            _listener = new TcpListener(IPAddress.Parse(host), port);

            _thread.Start();

            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
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
                    Console.WriteLine($"New connection from {newClient.RemoteEndPoint}");

                    ClientsManager.Instance.AddClient(Guid.NewGuid(), newClient);

                    /*var pingMess = new Network.Message
                    {
                        Code = Network.ClientCodes.Ping,
                        Json = JsonConvert.SerializeObject(0)
                    };
                    var pingStr = DependencyService.Get<IRMessageEncoder>().GetBytes(JsonConvert.SerializeObject(pingMess));
                    var sentSize = newClient.Send(pingStr); // TODO : Check sent size for missed datas

                    var id = Guid.NewGuid();
                    _pings.Add(id, DateTime.Now.TimeOfDay);

                    ClientsManager.Instance.AddClient(id, newClient);*/
                }

                /*List<Guid> disconnectedClients = new List<Guid>();
                Dictionary<Guid, TimeSpan> updatedPings = new Dictionary<Guid, TimeSpan>();
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

                _server?.TimeoutPlayers(disconnectedClients);

                foreach (var disc in disconnectedClients)
                {
                    if (ClientsManager.Instance.RemoveClient(disc))
                    {
                        _pings.Remove(disc);
                        Server.Runtime.Console.Instance.AddMessage($"{disc} Disconnected.");
                    }
                }*/

                foreach (var client in ClientsManager.Instance.Clients)
                {
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
                                AddCommand(commandArgs);
                            }
                        }
                    }
                }

                while (_responses.Count > 0)
                {
                    var response = TakeResponse();

                    if (!Guid.Empty.Equals(response.ClientId)
                        && ClientsManager.Instance.Clients.ContainsKey(response.ClientId))
                    {
                        var socket = ClientsManager.Instance.Clients[response.ClientId];

                        var jsonObj = JsonConvert.SerializeObject(response.ClientResponse);
                        var encoder = new ASCIIEncoding();
                        var mess = encoder.GetBytes(jsonObj);
                        var sentSize = socket.Send(mess); // TODO : Check sent size for missed datas
                    }
                }
            }

            foreach (var client in ClientsManager.Instance.Clients)
            {
                client.Value.Shutdown(SocketShutdown.Both);
            }

            _flushCommands.Stop();

            _listener.Stop();
            Console.WriteLine($"TCP Server stopped.");

            _thread.Abort();
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
                case Network.CommandCodes.Gameserver_Connect:
                    retVal.IsValid = true;
                    retVal.Args = new string[1] { clientId.ToString() };
                    break;
                case Network.CommandCodes.Gameserver_Infos:

                    break;
                default:
                    retVal.IsValid = false;
                    break;
            }

            return retVal;
        }
    }
}
