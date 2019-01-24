using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootListItem : MonoBehaviour
{
    [Header("Item")]
    public Text Slot;
    public Text Name;
    public Button Action;

    [Header("Loot")]
    public Button Loot;

    public void Initialize(JsonObjects.Items.Item item, JsonObjects.LootItem slot)
    {
        switch (item.Type)
        {
            case JsonObjects.Items.ItemType.Bag:
                Slot.text = "Bag";
                break;
            case JsonObjects.Items.ItemType.Consumable:
                Slot.text = "Cons.";
                break;
            case JsonObjects.Items.ItemType.Armor:
                Slot.text = item.Slot.ToString();
                break;
            default:
                Slot.text = string.Format("{0}.{1}", item.Slot == JsonObjects.Items.ItemSlot.OneHand ? "1H" : "2H", item.Type);
                break;
        }
        SetName(item, slot);
    }

    private void SetName(JsonObjects.Items.Item item, JsonObjects.LootItem slot)
    {
        if (slot.Quantity > 0)
        {
            Name.text = string.Format("{0} * [{1}]", item.Name, slot.Quantity);
        }
        else
        {
            Name.text = string.Format("{0}", item.Name);
        }
    }
}
