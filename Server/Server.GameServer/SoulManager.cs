using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public class SoulManager
    {
        private static SoulManager _instance;
        public static SoulManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoulManager();
                }

                return _instance;
            }
        }

        private Dictionary<Guid, DataModels.Soul> _souls;
        private Dictionary<Guid, DateTime> _soulTime;

        private SoulManager()
        {
            _souls = new Dictionary<Guid, DataModels.Soul>();
            _soulTime = new Dictionary<Guid, DateTime>();
        }

        public bool ConnectSoul(Guid token, DataModels.Soul soul)
        {
            if (!_souls.ContainsKey(token))
            {
                _souls.Add(token, soul);
                _soulTime.Add(token, DateTime.Now);
            }

            return false;
        }

        public bool DisconnectSoul(Guid token)
        {
            if (_souls.ContainsKey(token))
            {
                var soul = _souls[token];
                _souls.Remove(token);

                var timePlayed = DateTime.Now - _soulTime[token];
                _soulTime.Remove(token);

                soul.TimePlayed += (float)timePlayed.TotalSeconds;
                DataRepositories.SoulRepository.Update(soul);

                return true;
            }

            return false;
        }

    }
}
