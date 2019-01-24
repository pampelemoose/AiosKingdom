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

        public bool IsUnlocked(Guid dungeonId, List<Network.AdventureUnlocked> unlocks)
        {
            var dungeon = DataManager.Instance.Dungeons.FirstOrDefault(d => d.Id.Equals(dungeonId));
            if (dungeon.Locks.Count > 0)
            {
                foreach (var locks in dungeon.Locks)
                {
                    if (!unlocks.Any(u => u.AdventureId.Equals(locks.LockedId)))
                    {
                        return false;
                    }
                }
                return true;
            }

            return true;
        }

        public Adventure Start(Guid soulId, Network.SoulDatas datas, Guid dungeonId, List<Network.Knowledge> knowledges, List<Network.AdventureState.BagItem> bagItems = null)
        {
            Log.Instance.Write(Log.Level.Infos, $"AdventureManager().OpenRoom({soulId}, {dungeonId})");
            var dungeon = DataManager.Instance.Dungeons.FirstOrDefault(d => d.Id.Equals(dungeonId));

            if (!_adventures.ContainsKey(soulId) || 
                (_adventures.ContainsKey(soulId) && !_adventures[soulId].AdventureId.Equals(dungeonId)))
            {
                if (datas.Level > dungeon.MaxLevelAuthorized) return null;

                var adventure = new Adventure(dungeon, datas, knowledges, bagItems);
                _adventures.Add(soulId, adventure);

                return adventure;
            }

            return null;
        }

        public Adventure OpenRoom(Guid soulId, Guid dungeonId)
        {
            Log.Instance.Write(Log.Level.Infos, $"AdventureManager().OpenRoom({soulId}, {dungeonId})");
            var dungeon = DataManager.Instance.Dungeons.FirstOrDefault(d => d.Id.Equals(dungeonId));

            if (_adventures.ContainsKey(soulId) && _adventures[soulId].AdventureId.Equals(dungeonId))
            {
                var adventure = _adventures[soulId];

                if (adventure.IsCleared)
                {
                    adventure.OpenNextRoom();
                    _adventures[soulId] = adventure;
                    return adventure;
                }
            }

            return null;
        }

        public void ExitRoom(Guid soulId)
        {
            Log.Instance.Write(Log.Level.Infos, $"AdventureManager().ExitRoom({soulId})");
            if (_adventures.ContainsKey(soulId))
            {
                var adventure = _adventures[soulId];
                //DataRepositories.AdventureRepository.SaveProgress(soulId, adventure.DungeonId, adventure.RoomNumber);
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
