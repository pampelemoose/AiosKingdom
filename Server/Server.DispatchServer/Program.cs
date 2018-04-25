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
            var sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("                                DISPATCH SERVER                                 ");
            sb.AppendLine("================================================================================\n");
            Console.WriteLine(sb.ToString());

            RequireAdminAuth();

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
                    if (user.Roles?.FirstOrDefault(r => r.Name.Equals("SuperAdmin")) != null)
                    {
                        break;
                    }
                }
            }
        }
    }
}
