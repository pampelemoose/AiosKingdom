using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pills : MonoBehaviour, ICallbackHooker
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
    private JsonObjects.Currencies _currencies;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.Currencies, (message) =>
        {
            if (message.Success)
            {
                var currencies = JsonConvert.DeserializeObject<JsonObjects.Currencies>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _updateCurrencies(currencies);
                });
            }
        });

        InputController.This.AddCallback("Pills", (direction) =>
        {
            if (gameObject.activeSelf)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    if (direction == SwipeDirection.Down)
                        GetComponent<Page>().CloseAction();
                });
            }
        });
    }

    void Start()
    {
        Stamina.onClick.AddListener(() =>
        {
            _selectStat(JsonObjects.Stats.Stamina);
            _setSelectedButtonColor(Stamina.GetComponentInChildren<Text>(), new Color(1, 0, 0));
        });

        Energy.onClick.AddListener(() =>
        {
            _selectStat(JsonObjects.Stats.Energy);
            _setSelectedButtonColor(Energy.GetComponentInChildren<Text>(), new Color(0.9f, 1, 0));
        });

        Strength.onClick.AddListener(() =>
        {
            _selectStat(JsonObjects.Stats.Strength);
            _setSelectedButtonColor(Strength.GetComponentInChildren<Text>(), new Color(1, 0, 0.8f));
        });

        Agility.onClick.AddListener(() =>
        {
            _selectStat(JsonObjects.Stats.Agility);
            _setSelectedButtonColor(Agility.GetComponentInChildren<Text>(), new Color(0, 1, 0.3f));
        });

        Intelligence.onClick.AddListener(() =>
        {
            _selectStat(JsonObjects.Stats.Intelligence);
            _setSelectedButtonColor(Intelligence.GetComponentInChildren<Text>(), new Color(0, 0.5f, 1));
        });

        Wisdom.onClick.AddListener(() =>
        {
            _selectStat(JsonObjects.Stats.Wisdom);
            _setSelectedButtonColor(Wisdom.GetComponentInChildren<Text>(), new Color(0, 1, 1));
        });

        Minus.onClick.AddListener(() =>
        {
            _setQuantity(_quantity - 1);
        });

        Plus.onClick.AddListener(() =>
        {
            _setQuantity(_quantity + 1);
        });

        Buy.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            NetworkManager.This.UseSpiritPills(_currentStat, _quantity);
        });
    }

    void Awake()
    {
        _selectStat(JsonObjects.Stats.Stamina);
        _setSelectedButtonColor(Stamina.GetComponentInChildren<Text>(), new Color(1, 0, 0));
    }

    private void _updateCurrencies(JsonObjects.Currencies currency)
    {
        _currencies = currency;

        _selectStat(_currentStat);
    }

    private void _selectStat(JsonObjects.Stats stat)
    {
        _setQuantity(0);

        _currentStat = stat;
    }

    private void _setSelectedButtonColor(Text selected, Color color)
    {
        if (_selectedText != null)
        {
            _selectedText.color = Color.white;
        }

        _selectedText = selected;
        _selectedText.color = color;
    }

    private void _setQuantity(int quantity)
    {
        QuantityAvailable.text = _currencies.Spirits.ToString();

        Minus.interactable = false;
        Plus.interactable = false;
        Buy.interactable = false;

        if (quantity > _currencies.Spirits)
            quantity = 0;

        _quantity = quantity;
        Quantity.text = quantity.ToString();

        if (_quantity > 0)
        {
            Minus.interactable = true;
            Buy.interactable = true;
        }

        if (_quantity < _currencies.Spirits)
            Plus.interactable = true;
    }
}
