using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public class Log
    {
        public enum Level
        {
            Infos,
            Warning,
            Error
        }

        private static Log _instance;
        public static Log Instance
        {
            get
            {
                if (_instance == null)
                {
                    var date = DateTime.Now.Date;

                    _instance = new Log($"Logs/{date.Year}-{date.Day}-{date.Month}");
                }

                return _instance;
            }
        }

        private StreamWriter _file;
        private StreamWriter _errorFile;
        private Log(string name)
        {
            Console.WriteLine($"Logging[{name}]");
            Directory.CreateDirectory("Logs/");
            _file = new StreamWriter(name + ".log", true);
            _file.WriteLine("\n\n=========================================================\n\n");
            _file.AutoFlush = true;
            _errorFile = new StreamWriter(name + "_error.log", true);
            _errorFile.WriteLine("\n\n=========================================================\n\n");
            _errorFile.AutoFlush = true;
        }

        ~Log()
        {
            try
            {
                if (_file != null)
                {
                    _file.Close();
                }

                if (_errorFile != null)
                {
                    _errorFile.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error closing log files : {e.Message}");
            }
        }

        private object _writeLock = new object();
        public void Write(Level level, string message)
        {
            lock (_writeLock)
            {
                var stackTrace = new StackTrace(true);
                var id = Guid.NewGuid();

                if (level == Level.Error)
                {
                    foreach (var r in stackTrace.GetFrames())
                    {
                        _errorFile.WriteLine($"[{DateTime.Now.ToLongTimeString()}][{level}][{id}]: Filename: {r.GetFileName()} Method: {r.GetMethod()} Line: {r.GetFileLineNumber()} Column: {r.GetFileColumnNumber()}");
                    }
                    _errorFile.Flush();
                }

                _file.WriteLine($"[{DateTime.Now.ToLongTimeString()}][{level}][{id}]: {message}");
                _file.Flush();
            }
        }
    }
}
