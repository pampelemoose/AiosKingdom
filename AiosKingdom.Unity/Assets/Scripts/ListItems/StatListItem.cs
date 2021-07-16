using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatListItem : MonoBehaviour
{
    public Text CurrentAmountText;
    public Text AddingAmountText;

    public Button MinusAmountButton;
    public Button PlusAmountButton;

    private int _currentStat = 0;
    private int _availablePoints = 0;
    private int _currentAddedAmount = 0;

    private Action _minusPressed;
    private Action _plusPressed;

    public int CurrentAddedAmount => _currentAddedAmount;

    public void Init(Action minusCallback, Action plusCallback)
    {
        _minusPressed = minusCallback;
        _plusPressed = plusCallback;

        MinusAmountButton.gameObject.SetActive(false);
        MinusAmountButton.onClick.RemoveAllListeners();
        MinusAmountButton.onClick.AddListener(() =>
        {
            if (_minusPressed != null)
            {
                _minusPressed();
            }
            --_currentAddedAmount;
            AddingAmountText.text = $"{_currentAddedAmount}";
            if (_currentAddedAmount == 0)
            {
                MinusAmountButton.gameObject.SetActive(false);
            }
        });

        PlusAmountButton.gameObject.SetActive(false);
        PlusAmountButton.onClick.RemoveAllListeners();
        PlusAmountButton.onClick.AddListener(() =>
        {
            if (_plusPressed != null)
            {
                _plusPressed();
            }
            ++_currentAddedAmount;
            AddingAmountText.text = $"{_currentAddedAmount}";
            if (_currentAddedAmount >= 0)
            {
                MinusAmountButton.gameObject.SetActive(true);
            }
        });


        AddingAmountText.text = $"{_currentAddedAmount}";
    }

    public void SetCurrentStat(int points)
    {
        _currentStat = points;
        CurrentAmountText.text = $"{_currentStat}";
    }

    public void SetAvailablePoints(int points)
    {
        _availablePoints = points;

        if (_availablePoints > 0)
        {
            PlusAmountButton.gameObject.SetActive(true);
        }
        else
        {
            PlusAmountButton.gameObject.SetActive(false);
        }
    }
}
