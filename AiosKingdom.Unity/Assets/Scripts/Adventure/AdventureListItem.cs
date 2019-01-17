using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureListItem : MonoBehaviour
{
    public Text Name;
    public Text Level;
    public Button Action;

    public void SetDatas(JsonObjects.Adventures.Adventure adventure)
    {
        Name.text = adventure.Name;
        Level.text = string.Format("{0}-{1}", adventure.RequiredLevel.ToString(), adventure.MaxLevelAuthorized.ToString());
    }
}
