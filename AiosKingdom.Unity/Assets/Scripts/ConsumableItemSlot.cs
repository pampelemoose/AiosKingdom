using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableItemSlot : MonoBehaviour
{
    private JsonObjects.Items.ItemType _type;
    private JsonObjects.Items.AItem _item;

    [Header("Main")]
    public Text Name;
    public Text Type;
    public Text Quality;

    [Space(10)]
    [Header("General Attributes")]
    public Text ItemLevel;
    public Text RequiredLevel;

    public void Initialize(JsonObjects.Items.Consumable item)
    {
        _type = JsonObjects.Items.ItemType.Consumable;
        _item = item;

        InitAitem(_item);

        var rectTrans = transform.GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, 300);
    }

    private void InitAitem(JsonObjects.Items.AItem item)
    {
        Name.text = item.Name;
        Quality.text = item.Quality.ToString();

        ItemLevel.text = item.ItemLevel.ToString();
        RequiredLevel.text = item.UseLevelRequired.ToString();
    }
}
