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

        [Obsolete("Need to not send all the object.", true)]
        public DataModels.Soul Soul { get; set; }

        public Network.SoulDatas Datas { get; set; }

        public Network.Currencies Currencies { get; set; }
        public List<DataModels.InventorySlot> Inventory { get; set; }
        public List<DataModels.Knowledge> Knowledges { get; set; }
        public DataModels.Equipment Equipment { get; set; }

        public List<DataModels.Items.Armor> Armors { get; set; }
        public List<DataModels.Items.Consumable> Consumables { get; set; }
        public List<DataModels.Items.Bag> Bags { get; set; }
        public List<DataModels.Items.Weapon> Weapons { get; set; }

        public List<DataModels.MarketSlot> MarketItems { get; set; }

        public List<DataModels.Skills.Book> Books { get; set; }

        public List<DataModels.Monsters.Monster> Monsters { get; set; }
        public List<DataModels.Dungeons.Dungeon> Dungeons { get; set; }

        public Network.AdventureState Adventure { get; set; }
    }
}
