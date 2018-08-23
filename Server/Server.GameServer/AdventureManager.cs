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

        public Adventure OpenRoom(DataModels.Soul soul, Network.SoulDatas datas, Guid dungeonId, List<Network.AdventureState.BagItem> bagItems = null)
        {
            Log.Instance.Write(Log.Level.Infos, $"AdventureManager().OpenRoom({soul.Id}, {dungeonId})");
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
                if (datas.Level > dungeon.MaxLevelAuthorized) return null;

                foreach (var bagItem in bagItems)
                {
                    var exists = soul.Inventory.FirstOrDefault(i => i.Id.Equals(bagItem.InventoryId));
                    if (exists != null)
                    {
                        soul.Inventory.Remove(exists);
                        exists.Quantity -= bagItem.Quantity;
                        if (exists.Quantity > 0)
                        {
                            soul.Inventory.Add(exists);
                        }
                    }
                }

                var adventure = new Adventure(dungeon, bagItems);
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
