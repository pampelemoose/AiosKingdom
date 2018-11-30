using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pills : MonoBehaviour
{
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
    private Text _selectedText;
    private JsonObjects.Stats _currentStat;

    void Start()
    {
        Stamina.onClick.RemoveAllListeners();
        Stamina.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Stamina);
            SetSelectedButtonColor(Stamina.GetComponentInChildren<Text>(), new Color(1, 0, 0));
        });

        Energy.onClick.RemoveAllListeners();
        Energy.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Energy);
            SetSelectedButtonColor(Energy.GetComponentInChildren<Text>(), new Color(0.9f, 1, 0));
        });

        Strength.onClick.RemoveAllListeners();
        Strength.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Strength);
            SetSelectedButtonColor(Strength.GetComponentInChildren<Text>(), new Color(1, 0, 0.8f));
        });

        Agility.onClick.RemoveAllListeners();
        Agility.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Agility);
            SetSelectedButtonColor(Agility.GetComponentInChildren<Text>(), new Color(0, 1, 0.3f));
        });

        Intelligence.onClick.RemoveAllListeners();
        Intelligence.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Intelligence);
            SetSelectedButtonColor(Intelligence.GetComponentInChildren<Text>(), new Color(0, 0.5f, 1));
        });

        Wisdom.onClick.RemoveAllListeners();
        Wisdom.onClick.AddListener(() =>
        {
            SelectStat(JsonObjects.Stats.Wisdom);
            SetSelectedButtonColor(Wisdom.GetComponentInChildren<Text>(), new Color(0, 1, 1));
        });

        Minus.onClick.RemoveAllListeners();
        Minus.onClick.AddListener(() =>
        {
            SetQuantity(_quantity - 1);
        });

        Plus.onClick.RemoveAllListeners();
        Plus.onClick.AddListener(() =>
        {
            SetQuantity(_quantity + 1);
        });

        Buy.onClick.RemoveAllListeners();
        Buy.onClick.AddListener(() =>
        {
            NetworkManager.This.UseSpiritPills(_currentStat, _quantity);
        });

        SelectStat(JsonObjects.Stats.Stamina);
        SetSelectedButtonColor(Stamina.GetComponentInChildren<Text>(), new Color(1, 0, 0));
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

    private void SetSelectedButtonColor(Text selected, Color color)
    {
        if (_selectedText != null)
        {
            _selectedText.color = Color.white;
        }

        _selectedText = selected;
        _selectedText.color = color;
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
