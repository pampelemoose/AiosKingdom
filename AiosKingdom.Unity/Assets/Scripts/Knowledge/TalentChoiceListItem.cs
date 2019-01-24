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
        Name.text = string.Format(": {0}", name);
        Type.text = string.Format(": {0}", talent.Type);
        Value.text = string.Format(": [{0}]", talent.Value);
        Cost.text = string.Format(": [{0}]", talent.TalentPointsRequired);
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
