using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DatasManager
{
    public class Watchable<T>
    {
        public event Action<T> Changed;

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (Changed != null)
                {
                    Changed.Invoke(value);
                }
                _value = value;
            }
        }
    }

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

    public JsonObjects.SoulDatas Datas { get; set; }

    public JsonObjects.Currencies Currencies { get; set; }
    public List<JsonObjects.InventorySlot> Inventory { get; set; }
    public List<JsonObjects.Knowledge> Knowledges { get; set; }
    public JsonObjects.Equipment Equipment { get; set; }

    public List<JsonObjects.Items.Item> Items { get; set; }

    public List<JsonObjects.MarketSlot> MarketItems { get; set; }
    public List<JsonObjects.MarketSlot> SpecialMarketItems { get; set; }

    public List<JsonObjects.Skills.Book> Books { get; set; }

    public List<JsonObjects.Monsters.Monster> Monsters { get; set; }
    public List<JsonObjects.Adventures.Adventure> Dungeons { get; set; }

    public JsonObjects.AdventureState Adventure { get; set; }

    public List<JsonObjects.Recipe> Recipes { get; set; }

    private Watchable<JsonObjects.Job> _job = new Watchable<JsonObjects.Job>();
    public Watchable<JsonObjects.Job> Job { get { return _job; } }
}
