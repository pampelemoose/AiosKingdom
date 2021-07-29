using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        Items = new List<JsonObjects.Items.Item>();
        Books = new List<JsonObjects.Skills.Book>();

        Monsters = new List<JsonObjects.Monsters.Monster>();
        Enemies = new List<JsonObjects.Adventures.Enemy>();
        Taverns = new List<JsonObjects.Adventures.Tavern>();
        Npcs = new List<JsonObjects.Adventures.Npc>();
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
    public List<JsonObjects.Adventures.Enemy> Enemies { get; set; }
    public List<JsonObjects.Adventures.Tavern> Taverns { get; set; }
    public List<JsonObjects.Adventures.Npc> Npcs { get; set; }

    public JsonObjects.AdventureState Adventure { get; set; }
}
