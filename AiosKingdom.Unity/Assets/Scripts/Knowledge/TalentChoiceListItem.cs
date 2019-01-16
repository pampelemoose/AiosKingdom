using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalentChoiceListItem : MonoBehaviour
{
    public Text Name;
    public Text Type;
    public Text Value;
    public Text Cost;
    public GameObject Selected;

    public Button Action;

    public void SetDatas(JsonObjects.Skills.Talent talent, string name)
    {
        Name.text = name;
        Type.text = talent.Type.ToString();
        Value.text = talent.Value.ToString();
        Cost.text = talent.TalentPointsRequired.ToString();
    }

    public void Select()
    {
        Selected.SetActive(true);
    }

    public void Unselect()
    {
        Selected.SetActive(false);
    }
}
