using System;
using System.Text;

namespace Server.GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("                                 GAME SERVER                                    ");
            sb.AppendLine("================================================================================\n");
            Console.WriteLine(sb.ToString());

            Server server = new Server();
            server.Start();

            while (server.IsRunning)
            {
                var command = Console.ReadLine();

                if (command.Equals("exit"))
                {
                    server.Stop();
                }
            }

            Console.ReadLine();
        }
    }
}
