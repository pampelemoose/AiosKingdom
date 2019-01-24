using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagListItem : MonoBehaviour
{
    public Text Name;
    public Button Action;
    public Button Remove;

    public void Initialize(JsonObjects.Items.Item item, JsonObjects.AdventureState.BagItem slot)
    {
        Name.text = string.Format("{0} * [{1}]", item.Name, slot.Quantity);
    }
}
