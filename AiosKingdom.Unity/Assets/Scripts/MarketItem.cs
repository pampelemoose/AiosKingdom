using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    public Button BuyItemButton;

    private JsonObjects.MarketSlot _slot;

    public void SetDatas(JsonObjects.MarketSlot slot)
    {
        _slot = slot;

        BuyItemButton.interactable = true;
        BuyItemButton.onClick.AddListener(() =>
        {
            Debug.Log(_slot.ShardPrice);
        });
    }
}
