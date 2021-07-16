using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    public int MaxStamina = 10;

    public int Health = 100;
    public int Mana = 100;

    public int Stamina => _currentStamina;
    private int _currentStamina;

    private int _currentStaminaConsumption;

    void Update()
    {
        UIHandler.This.UpdateCharacterStats(_currentStamina);
    }

    public void SetZoneConsumption(int val)
    {
        _currentStaminaConsumption = val;
    }

    public void Move()
    {
        _currentStamina -= _currentStaminaConsumption;
    }

    public void ConsumeStamina(int amount)
    {
        _currentStamina -= amount;
    }

    public void RestoreStamina(int amount)
    {
        _currentStamina = amount;
    }
}
