using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagSelectionListItem : MonoBehaviour
{
    public Text Name;
    public Button Action;
    public Button AddToBag;

    public void Initialize(JsonObjects.Items.Item item, JsonObjects.InventorySlot slot)
    {
        SetName(item, slot);
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
}
