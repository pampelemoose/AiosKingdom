using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombinaisonListItem : MonoBehaviour
{
    public Text Content;

    public void SetDatas(JsonObjects.Combinaison combinaison)
    {
        var itemId = combinaison.ItemId(JsonObjects.Items.ItemQuality.Common);

        if (itemId == null)
            itemId = combinaison.ItemId(JsonObjects.Items.ItemQuality.Uncommon);
        if (itemId == null)
            itemId = combinaison.ItemId(JsonObjects.Items.ItemQuality.Rare);
        if (itemId == null)
            itemId = combinaison.ItemId(JsonObjects.Items.ItemQuality.Epic);
        if (itemId == null)
            itemId = combinaison.ItemId(JsonObjects.Items.ItemQuality.Legendary);

        var item = DatasManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(itemId));

        Content.text = string.Format("{0} * [{1}-{2}]", item.Name, combinaison.MinQuantity, combinaison.MaxQuantity);
    }
}
