using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Watcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("                                    WATCHER                                     ");
            sb.AppendLine("================================================================================\n");
            Console.WriteLine(sb.ToString());

            var watcher = new Watcher();
            watcher.Start();
            while (watcher.IsRunning)
            {
                var exit = Console.ReadLine();

                if (exit.Equals("exit"))
                {
                    watcher.Stop();
                }
                else
                {
                    watcher.PrintStatus();
                }
            }
        }
    }
}
