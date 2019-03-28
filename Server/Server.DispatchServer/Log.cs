using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DispatchServer
{
    public class Log
    {
        public enum Type
        {
            Network,
            Stacktrace,
            Log
        }

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

        private Dictionary<Type, StreamWriter> _files;
        private Log(string name)
        {
            Console.WriteLine($"Logging[{name}]");
            Directory.CreateDirectory(name);

            _files = new Dictionary<Type, StreamWriter>();
            foreach (Type type in Enum.GetValues(typeof(Type)))
            {
                var file = new StreamWriter($"{name}/{type}.log", true);
                file.WriteLine("\n\n=========================================================\n\n");
                file.AutoFlush = true;
                _files.Add(type, file);
            }
        }

        ~Log()
        {
            foreach (var file in _files)
            {
                file.Value.Close();
            }
        }

        private object _writeLock = new object();
        public void Write(Type type, Level level, string message)
        {
            lock (_writeLock)
            {
                var stackTrace = new StackTrace(true);
                var id = Guid.NewGuid();

                if (level == Level.Error)
                {
                    foreach (var r in stackTrace.GetFrames())
                    {
                        _files[Type.Stacktrace].WriteLine($"[{DateTime.Now.ToLongTimeString()}][{level}][{id}]: Filename: {r.GetFileName()} Method: {r.GetMethod()} Line: {r.GetFileLineNumber()} Column: {r.GetFileColumnNumber()}");
                    }
                    _files[Type.Stacktrace].Flush();
                }

                _files[type].WriteLine($"[{DateTime.Now.ToLongTimeString()}][{level}][{id}]: {message}");
                _files[type].Flush();
            }
        }
    }
}
