using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    [Header("TopBar")]
    public Text Level;
    public Text Name;
    public Text Experience;

    [Space(2)]
    [Header("General")]
    public Text Health;
    public Text Mana;
    public Text Armor;
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

    [Space(10)]
    [Header("Menus")]
    public Button Equipment;
    public Button Inventory;
    public Button Knowledges;
    public Button Adventures;
    public Button Pvp;
    public Button Bookstore;
    public Button Market;

    void Start()
    {
        NetworkManager.This.AskSoulCurrentDatas();
        NetworkManager.This.AskCurrencies();
        NetworkManager.This.AskEquipment();
        NetworkManager.This.AskKnowledges();
        NetworkManager.This.AskInventory();

        PointsAvailable.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowPills();
            });
        });

        Equipment.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowEquipment();
            });
        });

        Inventory.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowInventory();
            });
        });

        Knowledges.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowKnowledges();
            });
        });

        Adventures.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowAdventures();
            });
        });

        Bookstore.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowBookstore();
            });
        });

        Market.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowMarket();
            });
        });
    }

    public void UpdatePlayerDatas()
    {
        Level.text = DatasManager.Instance.Datas.Level.ToString();
        Name.text = DatasManager.Instance.Datas.Name;

        Experience.text = string.Format("[{0} / {1}]", DatasManager.Instance.Datas.CurrentExperience, DatasManager.Instance.Datas.RequiredExperience);

        Health.text = string.Format("[{0}]", DatasManager.Instance.Datas.MaxHealth);
        Mana.text = string.Format("[{0}]", DatasManager.Instance.Datas.MaxMana);
        Armor.text = string.Format("[{0}]", DatasManager.Instance.Datas.Armor);
        ItemLevel.text = string.Format("[{0}]", DatasManager.Instance.Datas.ItemLevel);

        Stamina.text = string.Format("[{0}]", DatasManager.Instance.Datas.TotalStamina);
        Energy.text = string.Format("[{0}]", DatasManager.Instance.Datas.TotalEnergy);
        Strength.text = string.Format("[{0}]", DatasManager.Instance.Datas.TotalStrength);
        Agility.text = string.Format("[{0}]", DatasManager.Instance.Datas.TotalAgility);
        Intelligence.text = string.Format("[{0}]", DatasManager.Instance.Datas.TotalIntelligence);
        Wisdom.text = string.Format("[{0}]", DatasManager.Instance.Datas.TotalWisdom);

        MinDamages.text = string.Format("[{0}]", DatasManager.Instance.Datas.MinDamages);
        MaxDamages.text = string.Format("[{0}]", DatasManager.Instance.Datas.MaxDamages);
    }

    public void UpdateCurrencies()
    {
        Spirits.text = string.Format("[{0}]", DatasManager.Instance.Currencies.Spirits);
        Embers.text = string.Format("[{0}]", DatasManager.Instance.Currencies.Embers);
        Bits.text = string.Format("[{0}]", DatasManager.Instance.Currencies.Bits);
        Shards.text = string.Format("[{0}]", DatasManager.Instance.Currencies.Shards);

        PointsAvailable.gameObject.SetActive(false);
        if (DatasManager.Instance.Currencies.Spirits > 0)
        {
            PointsAvailable.gameObject.SetActive(true);
        }

        var script = transform.GetComponentInChildren<Pills>();

        if (script != null)
        {
            script.UpdateCurrencies();
        }
    }
}
