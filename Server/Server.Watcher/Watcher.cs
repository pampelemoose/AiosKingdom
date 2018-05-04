using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Watcher
{
    public class Watcher
    {
        private System.Timers.Timer _updateGameserverInfos;
        private bool _running;

        private object _wLock = new object();

        public Watcher()
        {
            _updateGameserverInfos = new System.Timers.Timer(1000);
            _updateGameserverInfos.Elapsed += (sender, e) =>
            {
                PingGameServers();
            };
            _updateGameserverInfos.AutoReset = true;
            _updateGameserverInfos.Enabled = true;
        }

        public void Start()
        {
            _running = true;
            _updateGameserverInfos.Start();
        }

        public void PrintStatus()
        {
            lock (_wLock)
            {
                foreach (var config in DataRepositories.GameServerRepository.GetAll())
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Checking [");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"{config.Name}");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("] : ");

                    if (config.Online)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Online\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Offline\n");
                    }
                }
            }
        }

        public bool IsRunning => _running;

        public void Stop()
        {
            _updateGameserverInfos.Stop();
            _running = false;
        }

        private void PingGameServers()
        {
            lock (_wLock)
            {
                foreach (var config in DataRepositories.GameServerRepository.GetAll())
                {
                    try
                    {
                        using (var tcp = new TcpClient())
                        {
                            tcp.Connect(config.Host, config.Port);
                            config.Online = true;
                        }
                    }
                    catch (SocketException sockEx)
                    {
                        config.Online = false;
                        //Console.WriteLine($"Server is not online(1)... [{sockEx.Message}]");
                    }

                    DataRepositories.GameServerRepository.Update();
                }
            }
        }
    }
}
