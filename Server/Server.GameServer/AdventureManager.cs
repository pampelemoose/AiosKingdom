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

        public Adventure GetAdventure(Guid soulId)
        {
            if (_adventures.ContainsKey(soulId))
            {
                var adventure = _adventures[soulId];

                return adventure;
            }

            return null;
        }

        public Adventure OpenRoom(Guid soulId, Network.SoulDatas datas, Guid dungeonId, List<Network.AdventureState.BagItem> bagItems = null)
        {
            Log.Instance.Write(Log.Level.Infos, $"AdventureManager().OpenRoom({soulId}, {dungeonId})");
            var dungeon = DataManager.Instance.Dungeons.FirstOrDefault(d => d.Id.Equals(dungeonId));

            if (_adventures.ContainsKey(soulId) && _adventures[soulId].DungeonId.Equals(dungeonId))
            {
                var adventure = _adventures[soulId];

                if (adventure.IsCleared)
                {
                    adventure.OpenNextRoom();
                    _adventures[soulId] = adventure;
                    return adventure;
                }
            }
            else
            {
                if (datas.Level > dungeon.MaxLevelAuthorized) return null;

                var adventure = new Adventure(dungeon, datas, bagItems);
                _adventures.Add(soulId, adventure);

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

        public void PlayerDied(Guid soulId)
        {
            Log.Instance.Write(Log.Level.Infos, $"AdventureManager().PlayerDied({soulId})");
            if (_adventures.ContainsKey(soulId))
            {
                var adventure = _adventures[soulId];
                _adventures.Remove(soulId);
            }
        }
    }
}
