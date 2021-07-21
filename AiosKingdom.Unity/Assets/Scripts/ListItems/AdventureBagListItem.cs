using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureBagListItem : MonoBehaviour
{
    public Text Name;
    public Text Quantity;
    public Button Action;
    public Button Remove;

    public void Initialize(JsonObjects.Items.Item item, JsonObjects.AdventureState.BagItem slot)
    {
        Name.text = item.Name;
        Quantity.text = $"{slot.Quantity}";
    }
}
