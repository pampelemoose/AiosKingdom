using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopListItem : MonoBehaviour
{
    [Header("Item")]
    public Text Slot;
    public Text Name;
    public Button Action;

    [Header("Market")]
    public Text Shard;
    public Button Buy;

    public void Initialize(JsonObjects.Items.Item item, JsonObjects.AdventureState.ShopState slot)
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
        Shard.text = slot.ShardPrice.ToString();
    }

    private void SetName(JsonObjects.Items.Item item, JsonObjects.AdventureState.ShopState slot)
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
