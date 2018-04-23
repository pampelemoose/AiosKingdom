using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public struct GameServerInfos
    {
        public string Name { get; set; }
        public int SlotsLimit { get; set; }
        public int SlotsAvailable { get; set; }
        public string Difficulty { get; set; }
    }
}
