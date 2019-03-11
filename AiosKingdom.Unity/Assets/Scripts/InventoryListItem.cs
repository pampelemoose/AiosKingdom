using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryListItem : MonoBehaviour
{
    public Text Slot;
    public Text Name;
    public Button Action;
    public Button More;
    public GameObject MoreBox;
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
                Slot.text = "Cons.";
                break;
            case JsonObjects.Items.ItemType.Armor:
                Slot.text = item.Slot.ToString();
                break;
            case JsonObjects.Items.ItemType.CraftingMaterial:
                Slot.text = "Cft.";
                break;
            case JsonObjects.Items.ItemType.Enchant:
                Slot.text = "Ect.";
                break;
            case JsonObjects.Items.ItemType.Gem:
                Slot.text = "Gem";
                break;
            default:
                Slot.text = string.Format("{0}.{1}", item.Slot == JsonObjects.Items.ItemSlot.OneHand ? "1H" : "2H", item.Type);
                break;
        }

        More.onClick.RemoveAllListeners();
        More.onClick.AddListener(() =>
        {
            MoreBox.SetActive(!MoreBox.activeSelf);
        });

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
        if (slot.Quantity > 0)
        {
            Name.text = string.Format("{0} * [{1}]", item.Name, slot.Quantity);
        }
        else
        {
            Name.text = string.Format("{0}", item.Name);
        }
    }

    private bool IsEquipable(JsonObjects.Items.Item item)
    {
        switch (item.Type)
        {
            case JsonObjects.Items.ItemType.Consumable:
            case JsonObjects.Items.ItemType.Junk:
            case JsonObjects.Items.ItemType.CraftingMaterial:
            case JsonObjects.Items.ItemType.Enchant:
            case JsonObjects.Items.ItemType.Gem:
                return false;
        }

        return true;
    }
}
