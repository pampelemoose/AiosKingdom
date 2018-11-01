using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public AiosKingdom Main;
    public NetworkManager Network;

    [Header("TopBar")]
    public Text Level;
    public Text Name;
    public Slider Experience;
    public Button ShowSummaryButton;

    [Space(10)]
    [Header("Summary")]
    public GameObject Summary;

    [Space(2)]
    [Header("General")]
    public Text Armor;
    public Text ItemLevel;

    [Space(2)]
    [Header("Attributes")]
    public Text Stamina;
    public Text Energy;
    public Text Strength;
    public Text Agility;
    public Text Intelligence;
    public Text Wisdom;

    [Space(2)]
    [Header("Currencies")]
    public Text Spirits;
    public Text Embers;
    public Text Bits;
    public Text Shards;

    [Space(5)]
    [Header("Equipments")]
    public GameObject Equipment;
    public GameObject ItemSlot;

    [Space(10)]
    [Header("Menus")]
    public Button News;
    public Button Adventures;
    public Button Pvp;
    public Button Inventory;
    public Button Knowledges;
    public Button Pills;
    public Button Bookstore;
    public Button Market;

    [Space(2)]
    [Header("Content")]
    public GameObject Content;
    public GameObject AdventuresContent;
    public GameObject InventoryContent;
    public GameObject KnowledgesContent;
    public GameObject PillsContent;
    public GameObject BookstoreContent;

    void Start()
    {
        Network.AskSoulCurrentDatas();
        Network.AskCurrencies();
        Network.AskEquipment();
        Network.AskKnowledges();

        ShowSummaryButton.onClick.AddListener(() =>
        {
            Summary.SetActive(!Summary.activeSelf);
        });

        News.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                LoadingScreen.Loading.Show();

                ShowNews();
            });
        });

        Adventures.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                LoadingScreen.Loading.Show();

                ShowAdventures();
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

        Pills.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                ShowPills();
            });
        });

        Bookstore.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                ShowBookstore();
            });
        });
    }

    void Update()
    {

    }

    public void UpdatePlayerDatas()
    {
        Level.text = DatasManager.Instance.Datas.Level.ToString();
        Name.text = DatasManager.Instance.Datas.Name;

        Experience.maxValue = DatasManager.Instance.Datas.RequiredExperience;
        Experience.value = DatasManager.Instance.Datas.CurrentExperience;

        Armor.text = DatasManager.Instance.Datas.Armor.ToString();
        ItemLevel.text = DatasManager.Instance.Datas.ItemLevel.ToString();

        Stamina.text = DatasManager.Instance.Datas.TotalStamina.ToString();
        Energy.text = DatasManager.Instance.Datas.TotalEnergy.ToString();
        Strength.text = DatasManager.Instance.Datas.TotalStrength.ToString();
        Agility.text = DatasManager.Instance.Datas.TotalAgility.ToString();
        Intelligence.text = DatasManager.Instance.Datas.TotalIntelligence.ToString();
        Wisdom.text = DatasManager.Instance.Datas.TotalWisdom.ToString();
    }

    public void UpdateCurrencies()
    {
        Spirits.text = DatasManager.Instance.Currencies.Spirits.ToString();
        Embers.text = DatasManager.Instance.Currencies.Embers.ToString();
        Bits.text = DatasManager.Instance.Currencies.Bits.ToString();
        Shards.text = DatasManager.Instance.Currencies.Shards.ToString();

        var script = transform.GetComponentInChildren<Pills>();

        if (script != null)
        {
            script.UpdateCurrencies();
        }
    }

    public void UpdateEquipment()
    {
        foreach (Transform child in Equipment.transform)
        {
            Destroy(child);
        }

        var equipment = DatasManager.Instance.Equipment;
        if (!Guid.Empty.Equals(equipment.Head))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsArmor(DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Head)));
        }
        if (!Guid.Empty.Equals(equipment.Shoulder))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsArmor(DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Shoulder)));
        }
        if (!Guid.Empty.Equals(equipment.Torso))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsArmor(DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Torso)));
        }
        if (!Guid.Empty.Equals(equipment.Belt))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsArmor(DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Belt)));
        }
        if (!Guid.Empty.Equals(equipment.Pants))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsArmor(DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Pants)));
        }
        if (!Guid.Empty.Equals(equipment.Leg))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsArmor(DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Leg)));
        }
        if (!Guid.Empty.Equals(equipment.Feet))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsArmor(DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Feet)));
        }
        if (!Guid.Empty.Equals(equipment.Hand))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsArmor(DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Hand)));
        }
        if (!Guid.Empty.Equals(equipment.WeaponLeft))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsWeapon(DatasManager.Instance.Weapons.FirstOrDefault(b => b.Id.Equals(equipment.WeaponLeft)));
        }
        if (!Guid.Empty.Equals(equipment.WeaponRight))
        {
            var newObj = Instantiate(ItemSlot, Equipment.transform);
            var script = newObj.GetComponent<ItemSlot>();
            script.InitializeAsWeapon(DatasManager.Instance.Weapons.FirstOrDefault(b => b.Id.Equals(equipment.WeaponRight)));
        }
        if (!Guid.Empty.Equals(equipment.Bag))
        {
            var bagObj = Instantiate(ItemSlot, Equipment.transform);
            var script = bagObj.GetComponent<ItemSlot>();
            script.InitializeAsBag(DatasManager.Instance.Bags.FirstOrDefault(b => b.Id.Equals(equipment.Bag)));
        }

        UIHelper.SetScrollviewVerticalSize(Equipment);
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

    private void ShowNews()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        LoadingScreen.Loading.Hide();
    }

    private void ShowAdventures()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(AdventuresContent, Content.transform);
        var script = advObj.GetComponent<Adventures>();
        script.Network = Network;
    }

    private void ShowInventory()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(InventoryContent, Content.transform);
        var script = advObj.GetComponent<Inventory>();
        script.Network = Network;
    }

    private void ShowKnowledges()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(KnowledgesContent, Content.transform);
        var script = advObj.GetComponent<Knowledges>();
        script.Network = Network;
    }

    private void ShowPills()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(PillsContent, Content.transform);
        var script = advObj.GetComponent<Pills>();
        script.Network = Network;
    }

    private void ShowBookstore()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var advObj = Instantiate(BookstoreContent, Content.transform);
        var script = advObj.GetComponent<Bookstore>();
        script.Network = Network;
    }
}
