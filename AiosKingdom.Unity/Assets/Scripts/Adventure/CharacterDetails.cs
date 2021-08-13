using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetails : MonoBehaviour
{
    public Button Close;

    [Header("TopBar")]
    public Slider ExperienceBar;
    public Text ExperienceText;

    [Space(2)]
    [Header("General")]
    public Slider HealthBar;
    public Text HealthText;
    public Slider ManaBar;
    public Text ManaText;

    public Text Armor;
    public Text MagicArmor;

    [Space(2)]
    [Header("Offensive")]
    public Text MinDamages;
    public Text MaxDamages;

    [Space(2)]
    [Header("Attributes")]
    public Text Stamina;
    public Text Energy;
    public Text Strength;
    public Text Agility;
    public Text Intelligence;
    public Text Wisdom;

    void Awake()
    {
        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            WorldManager.This.SetCanMove(true);
            gameObject.SetActive(false);
        });
    }

    public void Open()
    {
        ExperienceBar.maxValue = DatasManager.Instance.Adventure.State.Experience;
        ExperienceBar.value = DatasManager.Instance.Adventure.State.Experience;
        ExperienceText.text = $"{DatasManager.Instance.Adventure.State.Experience}";

        HealthBar.maxValue = (float)DatasManager.Instance.Adventure.State.MaxHealth;
        HealthBar.value = (float)DatasManager.Instance.Adventure.State.CurrentHealth;
        HealthText.text = $"{DatasManager.Instance.Adventure.State.CurrentHealth} / {DatasManager.Instance.Adventure.State.MaxHealth}";
        ManaBar.maxValue = (float)DatasManager.Instance.Adventure.State.MaxMana;
        ManaBar.value = (float)DatasManager.Instance.Adventure.State.CurrentMana;
        ManaText.text = $"{DatasManager.Instance.Adventure.State.CurrentMana} / {DatasManager.Instance.Adventure.State.MaxMana}";

        //Armor.text = $"{DatasManager.Instance.Adventure.State.Armor}";
        //MagicArmor.text = $"{DatasManager.Instance.Adventure.State.MagicArmor}";

        Stamina.text = $"{DatasManager.Instance.Adventure.State.Stamina}";
        Energy.text = $"{DatasManager.Instance.Adventure.State.Energy}";
        Strength.text = $"{DatasManager.Instance.Adventure.State.Strength}";
        Agility.text = $"{DatasManager.Instance.Adventure.State.Agility}";
        Intelligence.text = $"{DatasManager.Instance.Adventure.State.Intelligence}";
        Wisdom.text = $"{DatasManager.Instance.Adventure.State.Wisdom}";

        MinDamages.text = $"{DatasManager.Instance.Adventure.State.MinDamages}";
        MaxDamages.text = $"{DatasManager.Instance.Adventure.State.MaxDamages}";

        WorldManager.This.SetCanMove(false);
        gameObject.SetActive(true);
    }
}
