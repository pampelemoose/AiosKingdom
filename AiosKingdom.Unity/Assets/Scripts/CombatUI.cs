using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public Text PlayerHealthLabel;
    public Slider PlayerHealth;
    public Text PlayerManaLabel;
    public Slider PlayerMana;

    public Transform EnnemyList;

    public GameObject EnnemyUIPrefab;

    public Button Attack;

    private Dictionary<string, Dictionary<string, double>> _ennemiesData;
    private List<GameObject> _ennemies;
    private string _selectedEnnemy;

    private Dictionary<string, double> _attacks;

    void Awake()
    {
        _ennemiesData = new Dictionary<string, Dictionary<string, double>>();

        // BASIC ENEMY
        _ennemiesData.Add("SomeMonster", new Dictionary<string, double>());
        _ennemiesData["SomeMonster"]["Level"] = 1f;
        _ennemiesData["SomeMonster"]["Health"] = 100f;
        _ennemiesData["SomeMonster"]["Mana"] = 100f;

        _ennemies = new List<GameObject>();

        _attacks = new Dictionary<string, double>();
        _attacks["One"] = 10f;
        _attacks["Two"] = 30f;

        Attack.onClick.AddListener(() =>
        {
            _attackEnnemy(_selectedEnnemy, "One");
        });
    }

    private void _attackEnnemy(string ennemy, string attack)
    {
        var attackPower = _attacks[attack];
        _ennemiesData[ennemy]["Health"] -= attackPower;

        if (_ennemiesData[ennemy]["Health"] == 0)
        {
            AdventureUIManager.This.EndCombat();
        }
    }

    public void SetPlayerData(int maxHealth, int health, int maxMana, int mana)
    {
        PlayerHealthLabel.text = $"{health} / {maxHealth}";
        PlayerManaLabel.text = $"{mana} / {maxMana}";

        PlayerHealth.maxValue = maxHealth;
        PlayerHealth.value = health;

        PlayerMana.maxValue = maxMana;
        PlayerMana.value = mana;
    }

    public void SetEnnemyData(string ennemyName)
    {
        if (_ennemiesData.ContainsKey(ennemyName))
        {
            _selectedEnnemy = ennemyName;

            var ennemyInstance = Instantiate(EnnemyUIPrefab, EnnemyList);

            var ennemyScript = ennemyInstance.GetComponent<EnnemyUI>();
            ennemyScript.SetEnnemyData(ennemyName, (int)_ennemiesData[ennemyName]["Health"]);
            ennemyScript.SelectEnnemy(true);

            _ennemies.Add(ennemyInstance);
        }
    }

    void Update()
    {
        foreach (var ennemy in _ennemies)
        {
            var ennemyScript = ennemy.GetComponent<EnnemyUI>();

            ennemyScript.SetHealth((int)_ennemiesData[ennemyScript.EnnemyName]["Health"]);
        }
    }
}
