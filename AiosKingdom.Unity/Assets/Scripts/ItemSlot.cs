using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Text Slot;
    public Text Name;
    public Button Action;

    public void InitializeAsArmor(JsonObjects.Items.Item item)
    {
        Name.text = item.Name;
        Slot.text = item.Slot.ToString();
    }

    public void InitializeAsBag(JsonObjects.Items.Item item)
    {
        Name.text = item.Name;
        Slot.text = "Bag";
    }

    public void InitializeAsWeapon(JsonObjects.Items.Item item)
    {
        Name.text = item.Name;
        Slot.text = item.Slot.ToString();
    }

    public void InitializeAsConsumable(JsonObjects.Items.Item item)
    {
        Name.text = item.Name;
        Slot.text = "Cons.";
    }
}
