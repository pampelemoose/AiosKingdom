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
        private Thread _thread;

        private bool _running;

        public Watcher()
        {
            ThreadStart del = new ThreadStart(Run);
            _thread = new Thread(del);
        }

        public void Start()
        {
            _running = true;

            _thread.Start();
        }

        public void PrintStatus()
        {
            foreach (var config in DataRepositories.TownRepository.GetAll())
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
                    //Console.WriteLine($"Server {config.Name} is not online... [{sockEx.Message}]");
                }

                DataRepositories.TownRepository.Update(config);

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

        public void Run()
        {
            while (_running)
            {
                PrintStatus();

                Thread.Sleep(30000);
            }

            _thread.Abort();
        }

        public void Stop()
        {
            _running = false;
        }
    }
}
