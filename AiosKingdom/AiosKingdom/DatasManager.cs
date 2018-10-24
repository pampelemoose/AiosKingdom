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

        public Network.SoulDatas Datas { get; set; }

        public Network.Currencies Currencies { get; set; }
        public List<Network.InventorySlot> Inventory { get; set; }
        public List<Network.Knowledge> Knowledges { get; set; }
        public Network.Equipment Equipment { get; set; }

        public List<Network.Items.Armor> Armors { get; set; }
        public List<Network.Items.Consumable> Consumables { get; set; }
        public List<Network.Items.Bag> Bags { get; set; }
        public List<Network.Items.Weapon> Weapons { get; set; }

        public List<Network.MarketSlot> MarketItems { get; set; }

        public List<Network.Skills.Book> Books { get; set; }

        public List<Network.Monsters.Monster> Monsters { get; set; }
        public List<Network.Adventures.Dungeon> Dungeons { get; set; }

        public Network.AdventureState Adventure { get; set; }
    }
}
