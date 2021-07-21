using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryListItem : MonoBehaviour
{
    public Text Name;
    public Text Slot;
    public Text Quality;
    public Text Quantity;

    public Button Action;
    public Button Sell;
    public Button Equip;

    public void Initialize(JsonObjects.Items.Item item, JsonObjects.InventorySlot slot)
    {
        SetName(item, slot);

        switch (item.Type)
        {
            case JsonObjects.Items.ItemType.Bag:
                Slot.text = "Bag";
                break;
            case JsonObjects.Items.ItemType.Consumable:
                Slot.text = "Consumable";
                break;
            case JsonObjects.Items.ItemType.Armor:
                Slot.text = item.Slot.ToString();
                break;
            default:
                Slot.text = string.Format("{0}.{1}", item.Slot == JsonObjects.Items.ItemSlot.OneHand ? "1H" : "2H", item.Type);
                break;
        }

        Sell.onClick.RemoveAllListeners();
        Sell.onClick.AddListener(() =>
        {
            Debug.Log("Sell Item TODO");
        });

        Equip.gameObject.SetActive(IsEquipable(item));
        Equip.onClick.RemoveAllListeners();
        Equip.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                NetworkManager.This.EquipItem(slot.Id);
            });
        });
    }

    private void SetName(JsonObjects.Items.Item item, JsonObjects.InventorySlot slot)
    {
        Name.text = item.Name;
        Quality.text = item.Quality.ToString();

        if (slot.Quantity > 0)
        {
            Quantity.text = $"{slot.Quantity}";
        }
        else
        {
            Quantity.gameObject.SetActive(false);
        }
    }

    private bool IsEquipable(JsonObjects.Items.Item item)
    {
        switch (item.Type)
        {
            case JsonObjects.Items.ItemType.Consumable:
            case JsonObjects.Items.ItemType.Junk:
                return false;
        }

        return true;
    }
}
