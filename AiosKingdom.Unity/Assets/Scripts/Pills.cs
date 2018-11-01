using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pills : MonoBehaviour
{
    public NetworkManager Network;

    public Button Stamina;
    public Button Energy;
    public Button Strength;
    public Button Agility;
    public Button Intelligence;
    public Button Wisdom;

    public Button Minus;
    public Text QuantityAvailable;
    public Text Quantity;
    public Button Plus;
    public Button Buy;

    private int _quantity;
    private JsonObjects.Stats _currentStat;

    void Start()
    {
        Stamina.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Stamina);
        });

        Energy.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Energy);
        });

        Strength.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Strength);
        });

        Agility.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Agility);
        });

        Intelligence.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Intelligence);
        });

        Wisdom.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Wisdom);
        });

        Minus.onClick.AddListener(() =>
        {
            SetQuantity(_quantity - 1);
        });

        Plus.onClick.AddListener(() =>
        {
            SetQuantity(_quantity + 1);
        });

        Buy.onClick.AddListener(() =>
        {
            Network.UseSpiritPills(_currentStat, _quantity);
        });

        SelectStat(JsonObjects.Stats.Stamina);
    }

    public void UpdateCurrencies()
    {
        SelectStat(_currentStat);
    }

    private void SelectStat(JsonObjects.Stats stat)
    {
        SetQuantity(0);

        _currentStat = stat;
    }

    private void SetQuantity(int quantity)
    {
        QuantityAvailable.text = DatasManager.Instance.Currencies.Spirits.ToString();

        Minus.interactable = false;
        Plus.interactable = false;
        Buy.interactable = false;

        if (quantity > DatasManager.Instance.Currencies.Spirits)
            quantity = 0;

        _quantity = quantity;
        Quantity.text = quantity.ToString();

        if (_quantity > 0)
        {
            Minus.interactable = true;
            Buy.interactable = true;
        }

        if (_quantity < DatasManager.Instance.Currencies.Spirits)
            Plus.interactable = true;
    }
}
