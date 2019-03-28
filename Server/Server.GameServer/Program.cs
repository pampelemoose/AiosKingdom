using System;
using System.Linq;
using System.Text;

namespace Server.GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

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

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exc = e.ExceptionObject as Exception;

            Log.Instance.Write(Log.Type.Log, Log.Level.Error, $"Unhandled exception from {exc.Source} : {exc.Message} ({exc.StackTrace}).");
        }
    }
}
