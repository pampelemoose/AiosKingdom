using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatSelectionController : MonoBehaviour
{
    public Text AvailablePointsText;

    public StatListItem Stamina;
    public StatListItem Energy;
    public StatListItem Strength;
    public StatListItem Agility;
    public StatListItem Intelligence;
    public StatListItem Wisdom;

    public Button CloseButton;
    public Button ApplyButton;

    private int _spirits = 0;

    void Awake()
    {
        _updateSpiritValue(DatasManager.Instance.Currencies.StatPoints);

        Stamina.Init(() =>
        {
            _updateSpiritValue(_spirits + 1);
        }, () =>
        {
            _updateSpiritValue(_spirits - 1);
        });
        Stamina.SetCurrentStat(DatasManager.Instance.Datas.TotalStamina);
        Stamina.SetAvailablePoints(_spirits);

        Energy.Init(() =>
        {
            _updateSpiritValue(_spirits + 1);
        }, () =>
        {
            _updateSpiritValue(_spirits - 1);
        });
        Energy.SetCurrentStat(DatasManager.Instance.Datas.TotalEnergy);
        Energy.SetAvailablePoints(_spirits);

        Strength.Init(() =>
        {
            _updateSpiritValue(_spirits + 1);
        }, () =>
        {
            _updateSpiritValue(_spirits - 1);
        });
        Strength.SetCurrentStat(DatasManager.Instance.Datas.TotalStrength);
        Strength.SetAvailablePoints(_spirits);

        Agility.Init(() =>
        {
            _updateSpiritValue(_spirits + 1);
        }, () =>
        {
            _updateSpiritValue(_spirits - 1);
        });
        Agility.SetCurrentStat(DatasManager.Instance.Datas.TotalAgility);
        Agility.SetAvailablePoints(_spirits);

        Intelligence.Init(() =>
        {
            _updateSpiritValue(_spirits + 1);
        }, () =>
        {
            _updateSpiritValue(_spirits - 1);
        });
        Intelligence.SetCurrentStat(DatasManager.Instance.Datas.TotalIntelligence);
        Intelligence.SetAvailablePoints(_spirits);

        Wisdom.Init(() =>
        {
            _updateSpiritValue(_spirits + 1);
        }, () =>
        {
            _updateSpiritValue(_spirits - 1);
        });
        Wisdom.SetCurrentStat(DatasManager.Instance.Datas.TotalWisdom);
        Wisdom.SetAvailablePoints(_spirits);

        ApplyButton.onClick.RemoveAllListeners();
        ApplyButton.onClick.AddListener(() =>
        {
            Dictionary<JsonObjects.Stats, int> Stats = new Dictionary<JsonObjects.Stats, int>();

            if (Stamina.CurrentAddedAmount > 0)
            {
                Stats.Add(JsonObjects.Stats.Stamina, Stamina.CurrentAddedAmount);
            }

            if (Energy.CurrentAddedAmount > 0)
            {
                Stats.Add(JsonObjects.Stats.Energy, Energy.CurrentAddedAmount);
            }

            if (Strength.CurrentAddedAmount > 0)
            {
                Stats.Add(JsonObjects.Stats.Strength, Strength.CurrentAddedAmount);
            }

            if (Agility.CurrentAddedAmount > 0)
            {
                Stats.Add(JsonObjects.Stats.Agility, Agility.CurrentAddedAmount);
            }

            if (Intelligence.CurrentAddedAmount > 0)
            {
                Stats.Add(JsonObjects.Stats.Intelligence, Intelligence.CurrentAddedAmount);
            }

            if (Wisdom.CurrentAddedAmount > 0)
            {
                Stats.Add(JsonObjects.Stats.Wisdom, Wisdom.CurrentAddedAmount);
            }

            if (Stats.Count > 0)
            {
                UIManager.This.ShowLoading();
                NetworkManager.This.UseSpiritPills(Stats);
            }
        });

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(() =>
        {
            UIManager.This.ShowMain();
        });
    }

    private void _updateSpiritValue(int value)
    {
        _spirits = value;
        AvailablePointsText.text = $"{_spirits}";

        Stamina.SetAvailablePoints(_spirits);
        Energy.SetAvailablePoints(_spirits);
        Strength.SetAvailablePoints(_spirits);
        Agility.SetAvailablePoints(_spirits);
        Intelligence.SetAvailablePoints(_spirits);
        Wisdom.SetAvailablePoints(_spirits);
    }
}
