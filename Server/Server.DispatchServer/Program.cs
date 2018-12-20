using System;
using System.Linq;
using System.Security;
using System.Text;

namespace Server.DispatchServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            var sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("                                DISPATCH SERVER                                 ");
            sb.AppendLine("================================================================================\n");
            Console.WriteLine(sb.ToString());

            //RequireAdminAuth(); // TODO : Needed or not ?

            Server server = new Server();
            server.Start();

            while (server.IsRunning)
            {
                var command = Console.ReadLine();
                var cmdArgs = command.Split(' ');

                if (cmdArgs.Length >= 2 && cmdArgs[0].Equals("msgall"))
                {
                    var msg = string.Join(" ", cmdArgs.Skip(1).ToArray());
                    server.SendMessageToAll(msg);
                }

                if (command.Equals("exit"))
                {
                    server.Stop();
                }
            }

            Console.ReadLine();
        }

        public static void RequireAdminAuth()
        {
            while (true)
            {
                Console.Write("Username : ");
                var username = Console.ReadLine();
                Console.Write("Password : ");
                var pwd = new StringBuilder();
                while (true)
                {
                    ConsoleKeyInfo i = Console.ReadKey(true);
                    if (i.Key == ConsoleKey.Enter)
                    {
                        Console.Write("\n");
                        break;
                    }
                    else if (i.Key == ConsoleKey.Backspace)
                    {
                        if (pwd.Length > 0)
                        {
                            pwd.Remove(pwd.Length - 1, 1);
                            Console.Write("\b \b");
                        }
                    }
                    else
                    {
                        pwd.Append(i.KeyChar);
                        Console.Write("*");
                    }
                }

                var password = DataModels.User.EncryptPassword(pwd.ToString());
                var user = DataRepositories.UserRepository.GetByCredentials(username, password);
                if (user != null)
                {
                    if (user.Roles?.FirstOrDefault(r => r.Equals("SuperAdmin")) != null)
                    {
                        break;
                    }
                }
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exc = e.ExceptionObject as Exception;

            Log.Instance.Write(Log.Level.Error, $"Unhandled exception from {exc.Source} : {exc.Message} ({exc.StackTrace}).");
        }
    }
}
