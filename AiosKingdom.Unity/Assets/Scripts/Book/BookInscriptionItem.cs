using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookInscriptionItem : MonoBehaviour
{
    public Text Text;

    public void SetDatas(JsonObjects.Skills.Inscription inscription)
    {
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

        Text.text = string.Format("* {0} ({1}+([{2}]*{3}) over {4}) <{5}>",
            inscription.Type,
            inscription.BaseValue, inscription.StatType, inscription.Ratio,
            inscription.Duration,
            inscription.BaseValue + (inscription.Ratio * value));
    }

    public void SetBuiltDatas(JsonObjects.Skills.BuiltInscription inscription)
    {
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

        Text.text = string.Format("* {0} ([{1}-{2}]+([{3}]*{4}) over {5}) <{6}-{7}>",
            inscription.Type,
            inscription.BaseMinValue, inscription.BaseMaxValue, inscription.StatType, inscription.Ratio,
            inscription.Duration,
            inscription.BaseMinValue + (inscription.Ratio * value),
            inscription.BaseMaxValue + (inscription.Ratio * value));
    }
}
