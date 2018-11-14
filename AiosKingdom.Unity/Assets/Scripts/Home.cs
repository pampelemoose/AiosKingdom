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
    public Button MainPage;
    public Button Equipment;
    public Button Inventory;
    public Button Knowledges;
    public Button Adventures;
    public Button Pvp;
    public Button Bookstore;
    public Button Market;

    [Space(2)]
    [Header("Content")]
    public GameObject Content;
    public GameObject PillsContent;
    public GameObject EquipmentContent;
    public GameObject AdventuresContent;
    public GameObject InventoryContent;
    public GameObject KnowledgesContent;
    public GameObject BookstoreContent;

    void Start()
    {
        NetworkManager.This.AskSoulCurrentDatas();
        NetworkManager.This.AskCurrencies();
        NetworkManager.This.AskEquipment();
        NetworkManager.This.AskKnowledges();

        MainPage.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                foreach (Transform child in Content.transform)
                {
                    Destroy(child.gameObject);
                }
            });
        });

        PointsAvailable.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                ShowPills();
            });
        });

        Equipment.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                ShowEquipment();
            });
        });

        Inventory.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                ShowInventory();
            });
        });

        Knowledges.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                ShowKnowledges();
            });
        });

        Adventures.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowLoading();

                ShowAdventures();
            });
        });

        Bookstore.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                ShowBookstore();
            });
        });

        Market.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                SceneManager.LoadScene(2);
            });
        });
    }

    public void UpdatePlayerDatas()
    {
        Level.text = DatasManager.Instance.Datas.Level.ToString();
        Name.text = DatasManager.Instance.Datas.Name;

        Experience.text = string.Format("[{0}/{1}]", DatasManager.Instance.Datas.CurrentExperience, DatasManager.Instance.Datas.RequiredExperience);

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

    public void UpdateEquipment()
    {
        var script = transform.GetComponentInChildren<Equipment>();

        if (script != null)
        {
            script.UpdateEquipment();
        }
    }

    public void UpdateInventory()
    {
        var script = transform.GetComponentInChildren<Inventory>();

        if (script != null)
        {
            script.UpdateItems();
        }
    }

    public void UpdateKnowledges()
    {
        var script = transform.GetComponentInChildren<Knowledges>();

        if (script != null)
        {
            script.LoadKnowledges();
        }
    }

    private void ShowPills()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(PillsContent, Content.transform);
        var script = advObj.GetComponent<Pills>();
        script.UpdateCurrencies();
    }

    private void ShowEquipment()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(EquipmentContent, Content.transform);
        var script = advObj.GetComponent<Equipment>();
        script.UpdateEquipment();
    }

    private void ShowAdventures()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(AdventuresContent, Content.transform);
        var script = advObj.GetComponent<Adventures>();
    }

    private void ShowInventory()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(InventoryContent, Content.transform);
        var script = advObj.GetComponent<Inventory>();
    }

    private void ShowKnowledges()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(KnowledgesContent, Content.transform);
        var script = advObj.GetComponent<Knowledges>();
    }

    private void ShowBookstore()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(BookstoreContent, Content.transform);
        var script = advObj.GetComponent<Bookstore>();
    }
}
