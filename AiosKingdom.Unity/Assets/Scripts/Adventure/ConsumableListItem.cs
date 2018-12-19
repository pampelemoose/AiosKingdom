using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableListItem : MonoBehaviour
{
    public Text Name;
    public Button Use;
    public Button Action;

    public void SetDatas(JsonObjects.AdventureState.BagItem bagItem)
    {
        var item = DatasManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(bagItem.ItemId));

        Name.text = string.Format("{0} * [{1}]", item.Name, bagItem.Quantity);
    }
}
