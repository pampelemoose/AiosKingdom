using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketListItem : MonoBehaviour
{
    [Header("Item")]
    public Image Border;
    public Text Slot;
    public Text Name;
    public Button Action;

    [Header("Market")]
    public Image PriceBorder;
    public Text Quantity;
    public Text Shard;
    public Button Buy;

    public void Initialize(JsonObjects.Items.Item item, JsonObjects.MarketSlot slot)
    {
        Border.color = UIManager.ItemQualityColor[item.Quality];
        PriceBorder.color = UIManager.ItemQualityColor[item.Quality];

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
        Shard.text = slot.Price.ToString();
    }

    private void SetName(JsonObjects.Items.Item item, JsonObjects.MarketSlot slot)
    {
        Name.text = item.Name;

        if (slot.Quantity > 0)
        {
            Quantity.text = $"{slot.Quantity}";
        }
        else
        {
            Quantity.gameObject.SetActive(false);
        }
    }
}
