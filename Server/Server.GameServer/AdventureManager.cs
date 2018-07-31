using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public class AdventureManager
    {
        private static AdventureManager _instance;
        public static AdventureManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AdventureManager();
                }

                return _instance;
            }
        }

        private AdventureManager()
        {
            _adventures = new Dictionary<Guid, Adventure>();
        }

        private Dictionary<Guid, Adventure> _adventures;

        public Adventure GetAdventure(DataModels.Soul soul)
        {
            if (_adventures.ContainsKey(soul.Id))
            {
                var adventure = _adventures[soul.Id];

                return adventure;
            }

            return null;
        }

        public Adventure OpenRoom(DataModels.Soul soul, Guid dungeonId)
        {
            Log.Instance.Write(Log.Level.Infos, $"AdventureManager().OpenRoom({soul}, {dungeonId})");
            var dungeon = DataRepositories.DungeonRepository.GetById(dungeonId);

            if (_adventures.ContainsKey(soul.Id) && _adventures[soul.Id].DungeonId.Equals(dungeonId))
            {
                var adventure = _adventures[soul.Id];

                if (adventure.IsCleared)
                {
                    adventure.OpenNextRoom();
                    _adventures[soul.Id] = adventure;
                    return adventure;
                }
            }
            else
            {
                var adventure = new Adventure(dungeon);
                _adventures.Add(soul.Id, adventure);

                return adventure;
            }

            return null;
        }

        public void ExitRoom(Guid soulId)
        {
            Log.Instance.Write(Log.Level.Infos, $"AdventureManager().ExitRoom({soulId})");
            if (_adventures.ContainsKey(soulId))
            {
                var adventure = _adventures[soulId];
                DataRepositories.DungeonRepository.SaveProgress(soulId, adventure.DungeonId, adventure.RoomNumber);
                _adventures.Remove(soulId);
            }
        }
    }
}
