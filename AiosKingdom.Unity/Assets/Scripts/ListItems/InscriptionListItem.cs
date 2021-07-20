using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InscriptionListItem : MonoBehaviour
{
    public Text Type;
    public Text BaseValue;
    public Image StatBorder;
    public Text Stat;
    public Text Ratio;
    public Text Duration;
    public Text Current;

    public void SetDatas(JsonObjects.Skills.Inscription inscription, List<JsonObjects.Skills.Talent> talents)
    {
        int value = 0;

        switch (inscription.StatType)
        {
            case JsonObjects.Stats.Stamina:
                value = DatasManager.Instance.Datas.TotalStamina;
                Stat.text = "STR:";
                break;
            case JsonObjects.Stats.Energy:
                value = DatasManager.Instance.Datas.TotalEnergy;
                Stat.text = "ENE:";
                break;
            case JsonObjects.Stats.Strength:
                value = DatasManager.Instance.Datas.TotalStrength;
                Stat.text = "STR:";
                break;
            case JsonObjects.Stats.Agility:
                value = DatasManager.Instance.Datas.TotalAgility;
                Stat.text = "AGI:";
                break;
            case JsonObjects.Stats.Intelligence:
                value = DatasManager.Instance.Datas.TotalIntelligence;
                Stat.text = "INT:";
                break;
            case JsonObjects.Stats.Wisdom:
                value = DatasManager.Instance.Datas.TotalWisdom;
                Stat.text = "WIS:";
                break;
        }

        var talBaseValue = talents.Where(t => t.Type == JsonObjects.Skills.TalentType.BaseValue).Sum(t => t.Value);
        var talRatio = talents.Where(t => t.Type == JsonObjects.Skills.TalentType.Ratio).Sum(t => t.Value);
        var talStatValue = talents.Where(t => t.Type == JsonObjects.Skills.TalentType.StatValue).Sum(t => t.Value);
        //var talCooldown = talents.Where(t => t.Type == JsonObjects.Skills.TalentType.BaseValue).Sum(t => t.Value);
        var talDuration = talents.Where(t => t.Type == JsonObjects.Skills.TalentType.Duration).Sum(t => t.Value);

        Type.text = inscription.Type.ToString();
        StatBorder.color = UIManager.StatColors[inscription.StatType];
        Stat.color = UIManager.StatColors[inscription.StatType];
        Ratio.color = UIManager.StatColors[inscription.StatType];
        BaseValue.text = $"{inscription.BaseValue} (+{talBaseValue})";
        Ratio.text = $"{inscription.Ratio} (+{talRatio})";
        Duration.text = $"{inscription.Duration} (+{talDuration})";
        Current.text = $"{inscription.BaseValue + talBaseValue + ((inscription.Ratio + talRatio) * (value + talStatValue))}";
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

        Type.text = string.Format(": {0}", inscription.Type);
        BaseValue.text = string.Format(": [{0}-{1}]", inscription.BaseMinValue, inscription.BaseMaxValue);
        Stat.text = string.Format(": {0}", inscription.StatType);
        Ratio.text = string.Format(": {0}", inscription.Ratio);
        Duration.text = string.Format(": {0}", inscription.Duration);
        Current.text = string.Format(": [{0}-{1}]", inscription.BaseMinValue + (inscription.Ratio * value), inscription.BaseMaxValue + (inscription.Ratio * value));
    }
}
