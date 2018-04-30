using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom
{
    public class DatasManager
    {
        private static DatasManager _instance;
        public static DatasManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatasManager();
                }

                return _instance;
            }
        }

        private DatasManager()
        {
        }

        public List<Network.GameServerInfos> ServerInfos { get; set; }
    }
}
