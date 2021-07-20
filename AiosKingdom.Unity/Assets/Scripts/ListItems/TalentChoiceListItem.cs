using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalentChoiceListItem : MonoBehaviour
{
    public Image BorderImage;
    public Image CostBorderImage;

    public Text Name;
    public Text Type;
    public Text Value;
    public Text Cost;

    public Button Action;

    public void SetDatas(JsonObjects.Skills.Talent talent, string name)
    {
        Name.text = name;
        Type.text = $"{talent.Type}:";
        Value.text = $"{talent.Value}";
        Cost.text = $"{talent.TalentPointsRequired}";
    }

    public void Select()
    {
        BorderImage.color = new Color(1, 0, 0);
        CostBorderImage.color = new Color(1, 0, 0);
    }

    public void Unselect()
    {
        BorderImage.color = new Color(0.8f, 0.8f, 0.8f);
        CostBorderImage.color = new Color(0.8f, 0.8f, 0.8f);
    }
}
