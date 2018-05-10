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
        private bool _running;

        public Watcher()
        {
        }

        public void Start()
        {
            _running = true;
        }

        public void PrintStatus()
        {
            foreach (var config in DataRepositories.GameServerRepository.GetAll())
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Checking [");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"{config.Name}");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("] : ");

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

                DataRepositories.GameServerRepository.Update(config);

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

        public bool IsRunning => _running;

        public void Stop()
        {
            _running = false;
        }
    }
}
