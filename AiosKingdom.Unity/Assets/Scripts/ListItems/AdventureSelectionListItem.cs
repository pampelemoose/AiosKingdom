using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureSelectionListItem : MonoBehaviour
{
    public Text Name;
    public Text MinLevel;
    public Text MaxLevel;
    public Button Action;

    public void SetDatas(JsonObjects.Adventures.Adventure adventure)
    {
        Name.text = adventure.Name;
        MinLevel.text = $"{adventure.RequiredLevel}";
        MaxLevel.text = $"{adventure.MaxLevelAuthorized}";
    }
}
