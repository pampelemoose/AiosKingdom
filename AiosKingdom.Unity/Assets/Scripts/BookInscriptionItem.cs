using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookInscriptionItem : MonoBehaviour
{
    public Text Type;
    public Text BaseValue;
    public Text StatType;
    public Text Ratio;
    public Text Duration;

    public void SetDatas(JsonObjects.Skills.Inscription inscription)
    {
        Type.text = inscription.Type.ToString();
        BaseValue.text = inscription.BaseValue.ToString();
        StatType.text = inscription.StatType.ToString();
        Ratio.text = inscription.Ratio.ToString();
        Duration.text = inscription.Duration.ToString();
    }
}
