using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    [Header("TopBar")]
    public Text Level;
    public Text Name;
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
    public Text ItemLevel;

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
    public Button PointsAvailable;

    [Space(2)]
    [Header("Currencies")]
    public Text Spirits;
    public Text Embers;
    public Text Bits;
    public Text Shards;

    [Space(2)]
    [Header("Menus")]
    public Button Inventory;
    public Button Equipment;
    public Button Knowledges;
    public Button Bookstore;
    public Button Adventures;
    public Button Market;

    void Start()
    {
        NetworkManager.This.AskSoulCurrentDatas();
        NetworkManager.This.AskCurrencies();
        NetworkManager.This.AskEquipment();
        NetworkManager.This.AskKnowledges();
        NetworkManager.This.AskInventory();

        Equipment.onClick.RemoveAllListeners();
        Equipment.onClick.AddListener(() =>
        {
            UIManager.This.ShowEquipment();
        });

        Inventory.onClick.RemoveAllListeners();
        Inventory.onClick.AddListener(() =>
        {
            UIManager.This.ShowInventory();
        });

        PointsAvailable.onClick.RemoveAllListeners();
        PointsAvailable.onClick.AddListener(() =>
        {
            UIManager.This.ShowStatSelection();
        });
    }

    public void UpdatePlayerDatas()
    {
        Level.text = DatasManager.Instance.Datas.Level.ToString();
        Name.text = DatasManager.Instance.Datas.Name;

        ExperienceBar.maxValue = DatasManager.Instance.Datas.RequiredExperience;
        ExperienceBar.value = DatasManager.Instance.Datas.CurrentExperience;
        ExperienceText.text = $"{DatasManager.Instance.Datas.CurrentExperience} / {DatasManager.Instance.Datas.RequiredExperience}";

        HealthBar.maxValue = DatasManager.Instance.Datas.MaxHealth;
        HealthBar.value = DatasManager.Instance.Datas.MaxHealth;
        HealthText.text = $"{DatasManager.Instance.Datas.MaxHealth} / {DatasManager.Instance.Datas.MaxHealth}";
        ManaBar.maxValue = DatasManager.Instance.Datas.MaxMana;
        ManaBar.value = DatasManager.Instance.Datas.MaxMana;
        ManaText.text = $"{DatasManager.Instance.Datas.MaxMana} / {DatasManager.Instance.Datas.MaxMana}";

        Armor.text = $"{DatasManager.Instance.Datas.Armor}";
        MagicArmor.text = $"{DatasManager.Instance.Datas.MagicArmor}";
        ItemLevel.text = $"{DatasManager.Instance.Datas.ItemLevel}";

        Stamina.text = $"{DatasManager.Instance.Datas.TotalStamina}";
        Energy.text = $"{DatasManager.Instance.Datas.TotalEnergy}";
        Strength.text = $"{DatasManager.Instance.Datas.TotalStrength}";
        Agility.text = $"{DatasManager.Instance.Datas.TotalAgility}";
        Intelligence.text = $"{DatasManager.Instance.Datas.TotalIntelligence}";
        Wisdom.text = $"{DatasManager.Instance.Datas.TotalWisdom}";

        MinDamages.text = $"{DatasManager.Instance.Datas.MinDamages}";
        MaxDamages.text = $"{DatasManager.Instance.Datas.MaxDamages}";
    }

    public void UpdateCurrencies()
    {
        Spirits.text = $"{DatasManager.Instance.Currencies.Spirits}";
        Embers.text = $"{DatasManager.Instance.Currencies.Embers}";
        Bits.text = $"{DatasManager.Instance.Currencies.Bits}";
        Shards.text = $"{DatasManager.Instance.Currencies.Shards}";

        PointsAvailable.gameObject.SetActive(false);
        if (DatasManager.Instance.Currencies.Spirits > 0)
        {
            PointsAvailable.gameObject.SetActive(true);
        }
    }
}
