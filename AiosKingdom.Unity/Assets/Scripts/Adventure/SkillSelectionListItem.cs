using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionListItem : MonoBehaviour
{
    public Text Name;
    public Text ManaCost;
    public Button Use;
    public Button Action;

    public void SetDatas(JsonObjects.Skills.BuiltSkill skill)
    {
        Name.text = string.Format("{0}", skill.Name);
        ManaCost.text = string.Format("[{0}]", skill.ManaCost);
    }
}
