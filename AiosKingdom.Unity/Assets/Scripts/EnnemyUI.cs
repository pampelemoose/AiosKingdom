using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnnemyUI : MonoBehaviour
{
    public Text Name;
    public Slider HealthBar;
    public Text HealthText;
    public GameObject Selected;

    private int _maxHealth;
    private int _currentHealth;

    public string EnnemyName => Name.text;

    public void SetEnnemyData(string name, int maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;

        Name.text = name;
        HealthBar.maxValue = _maxHealth;
        HealthBar.value = _currentHealth;
        HealthText.text = $"{_currentHealth} / {_maxHealth}";
    }

    public void Hit(int amount)
    {
        _currentHealth -= amount;

        HealthBar.value = _currentHealth;
        HealthText.text = $"{_currentHealth} / {_maxHealth}";
    }    
    
    public void SetHealth(int health)
    {
        _currentHealth = health;

        HealthBar.value = _currentHealth;
        HealthText.text = $"{_currentHealth} / {_maxHealth}";
    }

    public void SelectEnnemy(bool select)
    {
        Selected.SetActive(select);
    }
}
