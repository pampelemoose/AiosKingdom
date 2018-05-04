using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class GameServerInfos
    {
        public Guid Id { get; set; }
        public bool Online { get; set; }
        public string Name { get; set; }
        public int SoulCount { get; set; }
        public int SlotsLimit { get; set; }
        public int SlotsAvailable { get; set; }
        public string Difficulty { get; set; }

        public string Availability
        {
            get { return $"{SlotsAvailable} / {SlotsLimit}"; }
        }
    }
}
