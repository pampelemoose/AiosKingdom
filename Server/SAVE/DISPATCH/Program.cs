using System;
using System.Linq;
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

            while (DataRepositories.UserRepository.GetAll().FirstOrDefault(u => u.Username.Equals("admin")) == null)
            {
                Console.WriteLine("Enter a password for ADMIN : ");
                var password = Console.ReadLine();
                var user = new DataModels.User
                {
                    Username = "admin",
                    Password = password
                };

                DataRepositories.UserRepository.Create(user);
            }

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
