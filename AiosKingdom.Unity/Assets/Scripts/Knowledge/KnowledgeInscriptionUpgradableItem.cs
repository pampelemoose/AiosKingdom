using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeInscriptionUpgradableItem : MonoBehaviour
{
    public Text Type;
    public Text BaseValue;
    public Text BaseValueUpgraded;
    public Text StatType;
    public Text Ratio;
    public Text RatioUpgraded;
    public Text Duration;
    public Text DurationUpgraded;
    public Text Calculated;
    public Text CalculatedUpgraded;

    public void SetDatas(JsonObjects.Skills.Inscription inscription, JsonObjects.Skills.Inscription upgraded)
    {
        Type.text = inscription.Type.ToString();
        BaseValue.text = inscription.BaseValue.ToString();
        BaseValueUpgraded.text = (upgraded.BaseValue - inscription.BaseValue).ToString("0.00");
        StatType.text = inscription.StatType.ToString();
        Ratio.text = inscription.Ratio.ToString();
        RatioUpgraded.text = (upgraded.Ratio - inscription.Ratio).ToString("0.00");
        Duration.text = inscription.Duration.ToString();
        DurationUpgraded.text = (upgraded.Duration - inscription.Duration).ToString("0.00");

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
        CalculatedUpgraded.text = ((upgraded.BaseValue - inscription.BaseValue) + ((upgraded.Ratio - inscription.Ratio) * value)).ToString("0");
    }
}
