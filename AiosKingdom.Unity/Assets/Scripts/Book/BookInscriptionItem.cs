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
    public Text Calculated;

    public void SetDatas(JsonObjects.Skills.Inscription inscription)
    {
        Type.text = inscription.Type.ToString();
        BaseValue.text = inscription.BaseValue.ToString();
        StatType.text = inscription.StatType.ToString();
        Ratio.text = inscription.Ratio.ToString();
        Duration.text = inscription.Duration.ToString();

        int value = 0;
        switch (inscription.StatType)
        {
            case JsonObjects.Stats.Stamina:
                value = DatasManager.Instance.Datas.TotalStamina;
                break;
            case JsonObjects.Stats.Energy:
                value = DatasManager.Instance.Datas.TotalEnergy;
                break;
            case JsonObjects.Stats.Strength:
                value = DatasManager.Instance.Datas.TotalStrength;
                break;
            case JsonObjects.Stats.Agility:
                value = DatasManager.Instance.Datas.TotalAgility;
                break;
            case JsonObjects.Stats.Intelligence:
                value = DatasManager.Instance.Datas.TotalIntelligence;
                break;
            case JsonObjects.Stats.Wisdom:
                value = DatasManager.Instance.Datas.TotalWisdom;
                break;
        }

        Calculated.text = (inscription.BaseValue + (inscription.Ratio * value)).ToString("0");
    }
}
