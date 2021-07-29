using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public class AdventureLog
    {
        private StreamWriter _file;

        public AdventureLog(Guid soulId, Guid adventureId)
        {
            Directory.CreateDirectory("AdventureLogs/");
            Directory.CreateDirectory($"AdventureLogs/Soul-{soulId}/");

            _file = new StreamWriter($"AdventureLogs/Soul-{soulId}/Adventure-{adventureId}.log", true);
            _file.WriteLine("\n\n=========================================================\n\n");
            _file.AutoFlush = true;
        }

        ~AdventureLog()
        {
            try
            {
                if (_file != null)
                {
                    _file.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error closing log files : {e.Message}");
            }
        }

        private object _writeLock = new object();
        public void Write(string message)
        {
            lock (_writeLock)
            {
                var stackTrace = new StackTrace(true);
                var id = Guid.NewGuid();

                _file.WriteLine($"[{DateTime.Now.ToLongTimeString()}][{id}]: {message}");
                _file.Flush();
            }
        }

        public void Close()
        {
            try
            {
                if (_file != null)
                {
                    _file.Close();
                }

                _file = null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error closing log files : {e.Message}");
            }
        }
    }
}
