using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.DispatchServer
{
    public class GameServerManager
    {
        private static GameServerManager _instance;
        public static GameServerManager Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new GameServerManager();
                }

                return _instance;
            }
        }

        private struct GameServerLink
        {
            public bool Online { get; set; }
            public TcpClient Client { get; set; }
            public Guid Token { get; set; }
            public Network.GameServerInfos Infos { get; set; }
        }

        private List<string> _serverKeys;
        private Dictionary<string, GameServerLink> _servers;

        private GameServerManager()
        {
            _serverKeys = ConfigurationManager.AppSettings.AllKeys.Where(n => n.Contains("Server")).ToList();
            _servers = new Dictionary<string, GameServerLink>();
        }

        public void SetupGameServers()
        {
            foreach (var key in _serverKeys)
            {
                var address = ConfigurationManager.AppSettings.Get(key);
                GameServerLink link;

                Console.WriteLine($"[{key}] = {address}");

                if (_servers.ContainsKey(key))
                {
                    Console.WriteLine($"Need to update.");
                    link = _servers[key];
                }
                else
                {
                    Console.WriteLine($"Need to connect.");

                    link = new GameServerLink();
                    var connectionInfos = address.Split(':');
                    link.Client = new TcpClient();
                    link.Online = true;
                    try
                    {
                        link.Client.Connect(connectionInfos[0], int.Parse(connectionInfos[1]));
                        Console.WriteLine($"Connected !");

                        var retMess = new Network.Message
                        {
                            Code = Network.CommandCodes.Gameserver_Connect
                        };
                        var encoder = new ASCIIEncoding();
                        var bytes = encoder.GetBytes(JsonConvert.SerializeObject(retMess) + '|');

                        link.Client.Client.Send(bytes);
                        Console.WriteLine($"Asking server authToken.");
                        _servers.Add(key, link);
                    }
                    catch (SocketException sockEx)
                    {
                        link.Online = false;
                        Console.WriteLine($"Server is not online... [{sockEx.Message}]");
                    }
                }

                if (link.Online)
                {
                    if (!Guid.Empty.Equals(link.Token))
                    {
                        try
                        {
                            var retMess = new Network.Message
                            {
                                Code = Network.CommandCodes.Gameserver_Infos,
                                Token = link.Token
                            };
                            var encoder = new ASCIIEncoding();
                            var bytes = encoder.GetBytes(JsonConvert.SerializeObject(retMess) + '|');

                            link.Client.Client.Send(bytes);
                            Console.WriteLine($"Asking server Infos.");
                        }
                        catch (SocketException sockEx)
                        {
                            link.Online = false;
                            Console.WriteLine($"Server is not online... [{sockEx.Message}]");
                        }
                    }
                }
            }
        }

        public void ReceiveServerInfos()
        {
            foreach (var key in _serverKeys)
            {
                if (!_servers.ContainsKey(key))
                    continue;

                var link = _servers[key];

                if (link.Online)
                {
                    try
                    {
                        if (link.Client.Available > 0)
                        {
                            var bufferSize = link.Client.Available;
                            var client = link.Client.Client;

                            byte[] buffer = new byte[bufferSize];
                            client.Receive(buffer);
                            var messageStr = Encoding.UTF8.GetString(buffer);

                            var message = JsonConvert.DeserializeObject<Network.Message>(messageStr);
                            if (message.Code == Network.CommandCodes.Gameserver_Connect)
                            {
                                var token = JsonConvert.DeserializeObject<Guid>(message.Json);
                                link.Token = token;
                                Console.WriteLine($"received token [{token}]");
                            }
                            if (message.Code == Network.CommandCodes.Gameserver_Infos)
                            {
                                var gameInfos = JsonConvert.DeserializeObject<Network.GameServerInfos>(message.Json);
                                link.Infos = gameInfos;
                                Console.WriteLine($"received infos [{gameInfos}]");
                            }
                        }
                    }
                    catch (SocketException sockEx)
                    {
                        link.Online = false;
                        Console.WriteLine($"Server is not online... [{sockEx.Message}]");
                    }

                    _servers[key] = link;
                }
            }
        }

        public List<Network.GameServerInfos> ServerInfos
        {
            get
            {
                return _servers.Select(s => s.Value.Infos).ToList();
            }
        }
    }
}
