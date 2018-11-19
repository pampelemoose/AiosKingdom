using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Text Slot;
    public Text Name;
    public Button Action;

    public void InitializeAsArmor(JsonObjects.Items.Armor item)
    {
        Name.text = item.Name;
        Slot.text = item.Part.ToString();
    }

    public void InitializeAsBag(JsonObjects.Items.Bag item)
    {
        Name.text = item.Name;
        Slot.text = "Bag";
    }

    public void InitializeAsWeapon(JsonObjects.Items.Weapon item)
    {
        Name.text = item.Name;
        Slot.text = item.HandlingType.ToString();
    }

    public void InitializeAsConsumable(JsonObjects.Items.Consumable item)
    {
        Name.text = item.Name;
        Slot.text = "Cons.";
    }
}
