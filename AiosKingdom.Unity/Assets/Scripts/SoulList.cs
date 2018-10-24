using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulList : MonoBehaviour
{
    public NetworkManager Network;

    public GameObject List;
    public GameObject SoulListItem;
    public GameObject SoulListCreateItem;

    public void SetSouls(List<JsonObjects.SoulInfos> souls)
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var soul in souls)
        {
            var soulItem = Instantiate(SoulListItem, List.transform);

            var script = soulItem.GetComponent<SoulListItem>();
            script.Network = Network;
            script.SetDatas(soul);
        }

        var createItem = Instantiate(SoulListCreateItem, List.transform);
        var createScript = createItem.GetComponent<SoulListCreateItem>();
        createScript.Network = Network;
    }
}
