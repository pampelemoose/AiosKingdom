using System;
using System.Linq;
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
                var cmdArgs = command.Split(' ');

                if (cmdArgs.Length >= 2 && cmdArgs[0].Equals("msgall"))
                {
                    var msg = string.Join(" ", cmdArgs.Skip(1).ToArray());
                    server.SendMessageToAll(Network.CommandCodes.GlobalMessage, msg);
                }

                if (command.Equals("willrestart"))
                {
                    server.RefuseNewConnection();
                }

                if (command.Equals("exit"))
                {
                    server.Stop();
                }
            }

            Console.ReadLine();
        }
    }
}
