using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Market : MonoBehaviour
{
    public Dropdown ItemTypeDropdown;

    public Text FirstFilterLabel;
    public Dropdown FirstFilterDropdown;

    public Text SecondFilterLabel;
    public Dropdown SecondFilterDropdown;

    [Space(10)]
    [Header("Content")]
    public GameObject Content;
    public GameObject Items;
    public GameObject ItemListItem;
    public GameObject ItemDetails;

    [Space(10)]
    [Header("BuyBox")]
    public GameObject BuyBox;
    public Text ShardPrice;
    public Text BitPrice;
    public Button BuyItemButton;

    private GameObject _itemDetails;
    private JsonObjects.Items.ItemType? _itemType;
    private JsonObjects.Items.ArmorPart? _armorPart;
    private JsonObjects.Items.HandlingType? _handlingType;
    private JsonObjects.Items.WeaponType? _weaponType;
    private JsonObjects.Items.EffectType? _effectType;

    void Awake()
    {
        if (_itemDetails == null)
        {
            _itemDetails = Instantiate(ItemDetails, Content.transform);

            ItemTypeDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Items.ItemType)).ToList());
            ItemTypeDropdown.onValueChanged.AddListener((value) =>
            {
                _itemType = null;
                if (value > 0)
                {
                    _itemType = (JsonObjects.Items.ItemType)Enum.Parse(typeof(JsonObjects.Items.ItemType), ItemTypeDropdown.options.ElementAt(value).text);
                    SetupFilters();
                }

                UpdateItems();
            });
        }

        NetworkManager.This.AskMarketItems();
    }

    private void SetupFilters()
    {
        FirstFilterLabel.gameObject.SetActive(false);
        FirstFilterDropdown.onValueChanged.RemoveAllListeners();
        FirstFilterDropdown.gameObject.SetActive(false);

        SecondFilterLabel.gameObject.SetActive(false);
        SecondFilterDropdown.onValueChanged.RemoveAllListeners();
        SecondFilterDropdown.gameObject.SetActive(false);

        if (_itemType != null)
        {
            switch (_itemType)
            {
                case JsonObjects.Items.ItemType.Armor:
                    {
                        FirstFilterLabel.gameObject.SetActive(true);
                        FirstFilterDropdown.gameObject.SetActive(true);

                        FirstFilterLabel.text = "Part";
                        FirstFilterDropdown.ClearOptions();
                        FirstFilterDropdown.AddOptions(new List<string> { "All" });
                        FirstFilterDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Items.ArmorPart)).ToList());

                        FirstFilterDropdown.onValueChanged.AddListener((value) =>
                        {
                            _armorPart = null;
                            if (value > 0)
                            {
                                _armorPart = (JsonObjects.Items.ArmorPart)Enum.Parse(typeof(JsonObjects.Items.ArmorPart), FirstFilterDropdown.options.ElementAt(value).text);
                            }

                            UpdateItems();
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Weapon:
                    {
                        FirstFilterLabel.gameObject.SetActive(true);
                        FirstFilterDropdown.gameObject.SetActive(true);

                        FirstFilterLabel.text = "Handling";
                        FirstFilterDropdown.ClearOptions();
                        FirstFilterDropdown.AddOptions(new List<string> { "All" });
                        FirstFilterDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Items.HandlingType)).ToList());

                        FirstFilterDropdown.onValueChanged.AddListener((value) =>
                        {
                            _handlingType = null;
                            if (value > 0)
                            {
                                _handlingType = (JsonObjects.Items.HandlingType)Enum.Parse(typeof(JsonObjects.Items.HandlingType), FirstFilterDropdown.options.ElementAt(value).text);
                            }

                            UpdateItems();
                        });

                        SecondFilterLabel.gameObject.SetActive(true);
                        SecondFilterDropdown.gameObject.SetActive(true);

                        SecondFilterLabel.text = "Type";
                        SecondFilterDropdown.ClearOptions();
                        SecondFilterDropdown.AddOptions(new List<string> { "All" });
                        SecondFilterDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Items.WeaponType)).ToList());

                        SecondFilterDropdown.onValueChanged.AddListener((value) =>
                        {
                            _weaponType = null;
                            if (value > 0)
                            {
                                _weaponType = (JsonObjects.Items.WeaponType)Enum.Parse(typeof(JsonObjects.Items.WeaponType), SecondFilterDropdown.options.ElementAt(value).text);
                            }

                            UpdateItems();
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Consumable:
                    {
                        FirstFilterLabel.gameObject.SetActive(true);
                        FirstFilterDropdown.gameObject.SetActive(true);

                        FirstFilterLabel.text = "Type";
                        FirstFilterDropdown.ClearOptions();
                        FirstFilterDropdown.AddOptions(new List<string> { "All" });
                        FirstFilterDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Items.EffectType)).ToList());

                        FirstFilterDropdown.onValueChanged.AddListener((value) =>
                        {
                            _effectType = null;
                            if (value > 0)
                            {
                                _effectType = (JsonObjects.Items.EffectType)Enum.Parse(typeof(JsonObjects.Items.EffectType), FirstFilterDropdown.options.ElementAt(value).text);
                            }

                            UpdateItems();
                        });
                    }
                    break;
            }
        }
    }

    public void UpdateItems()
    {
        foreach (Transform child in Items.transform)
        {
            Destroy(child.gameObject);
        }

        var marketList = new List<JsonObjects.MarketSlot>();
        marketList.AddRange(DatasManager.Instance.MarketItems);

        if (_itemType != null)
        {
            marketList = marketList.Where(m => m.Type == _itemType).ToList();
        }

        foreach (var slot in marketList)
        {
            switch (slot.Type)
            {
                case JsonObjects.Items.ItemType.Armor:
                    var armor = DatasManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(slot.ItemId));
                    if (_armorPart == null || (_armorPart != null && armor.Part != _armorPart))
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<ItemSlot>();
                        itemScript.InitializeAsArmor(armor);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            _itemDetails.GetComponent<ItemDetails>().ShowArmorDetails(armor);
                            BindItemToBuyBox(slot);
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Weapon:
                    var weapon = DatasManager.Instance.Weapons.FirstOrDefault(a => a.Id.Equals(slot.ItemId));
                    bool canAdd = true;
                    if (_handlingType != null && weapon.HandlingType != _handlingType)
                    {
                        canAdd = false;
                    }
                    if (_weaponType != null && weapon.WeaponType != _weaponType)
                    {
                        canAdd = false;
                    }
                    if (canAdd)
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<ItemSlot>();
                        itemScript.InitializeAsWeapon(weapon);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            _itemDetails.GetComponent<ItemDetails>().ShowWeaponsDetails(weapon);
                            BindItemToBuyBox(slot);
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Bag:
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<ItemSlot>();
                        var bag = DatasManager.Instance.Bags.FirstOrDefault(a => a.Id.Equals(slot.ItemId));
                        itemScript.InitializeAsBag(bag);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            _itemDetails.GetComponent<ItemDetails>().ShowBagDetails(bag);
                            BindItemToBuyBox(slot);
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Consumable:
                    var consumable = DatasManager.Instance.Consumables.FirstOrDefault(a => a.Id.Equals(slot.ItemId));
                    if (_effectType == null || (_effectType != null && consumable.Effects.Where(e => e.Type == _effectType).Any()))
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<ItemSlot>();
                        itemScript.InitializeAsConsumable(consumable);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            _itemDetails.GetComponent<ItemDetails>().ShowConsumableDetails(consumable);
                            BindItemToBuyBox(slot);
                        });
                    }
                    break;
            }
        }
    }

    private void BindItemToBuyBox(JsonObjects.MarketSlot slot)
    {
        BuyBox.SetActive(true);
        ShardPrice.text = string.Format("[{0}]", slot.ShardPrice);
        BitPrice.text = string.Format("[{0}]", slot.BitPrice);
        BuyItemButton.onClick.AddListener(() =>
        {

        });
    }
}
