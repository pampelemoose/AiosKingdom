using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class GameServerConnection
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public Guid Token { get; set; }
    }
}
