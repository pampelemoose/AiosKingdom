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
    public GameObject Items;
    public GameObject ItemListItem;
    public GameObject ItemDetails;
    public GameObject PaginationBox;
    public int ItemPerPage = 5;

    public GameObject PaginationPrefab;

    [Space(10)]
    [Header("BuyBox")]
    public GameObject BuyBox;
    public Text ShardPrice;
    public Text BitPrice;
    public Button BuyItemButton;

    private JsonObjects.Items.ItemType? _itemType;
    private JsonObjects.Items.ItemSlot? _itemSlot;
    private JsonObjects.Items.EffectType? _effectType;

    private Pagination _pagination;
    private int _currentPage = 1;
    private List<JsonObjects.MarketSlot> _slots;

    private bool _init = false;

    void Awake()
    {
        if (!_init)
        {
            ItemTypeDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Items.ItemType)).ToList());
            ItemTypeDropdown.onValueChanged.AddListener((value) =>
            {
                _itemType = null;
                if (value > 0)
                {
                    _itemType = (JsonObjects.Items.ItemType)Enum.Parse(typeof(JsonObjects.Items.ItemType), ItemTypeDropdown.options.ElementAt(value).text);
                }

                SetupFilters();

                UpdateItems();
            });

            _init = true;
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
                case JsonObjects.Items.ItemType.Axe:
                case JsonObjects.Items.ItemType.Book:
                case JsonObjects.Items.ItemType.Bow:
                case JsonObjects.Items.ItemType.Crossbow:
                case JsonObjects.Items.ItemType.Dagger:
                case JsonObjects.Items.ItemType.Fist:
                case JsonObjects.Items.ItemType.Gun:
                case JsonObjects.Items.ItemType.Mace:
                case JsonObjects.Items.ItemType.Polearm:
                case JsonObjects.Items.ItemType.Shield:
                case JsonObjects.Items.ItemType.Staff:
                case JsonObjects.Items.ItemType.Sword:
                case JsonObjects.Items.ItemType.Wand:
                case JsonObjects.Items.ItemType.Whip:
                    {
                        FirstFilterLabel.gameObject.SetActive(true);
                        FirstFilterDropdown.gameObject.SetActive(true);

                        FirstFilterLabel.text = "Slot";
                        FirstFilterDropdown.ClearOptions();
                        FirstFilterDropdown.AddOptions(new List<string> { "All" });
                        FirstFilterDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Items.ItemSlot)).ToList());

                        FirstFilterDropdown.onValueChanged.AddListener((value) =>
                        {
                            _itemSlot = null;
                            if (value > 0)
                            {
                                _itemSlot = (JsonObjects.Items.ItemSlot)Enum.Parse(typeof(JsonObjects.Items.ItemSlot), FirstFilterDropdown.options.ElementAt(value).text);
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
        _slots = DatasManager.Instance.MarketItems;

        SetupPagination();

        SetItems();
    }

    private void SetItems()
    {
        foreach (Transform child in Items.transform)
        {
            Destroy(child.gameObject);
        }

        var marketList = new List<JsonObjects.MarketSlot>();
        marketList.AddRange(_slots);

        if (_itemType != null)
        {
            marketList = marketList.Where(m => m.Type == _itemType).ToList();
        }
        else
        {
            marketList = marketList.Skip((_currentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();
        }

        foreach (var slot in marketList)
        {
            switch (slot.Type)
            {
                case JsonObjects.Items.ItemType.Armor:
                    var armor = DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(slot.ItemId));
                    if (_itemSlot == null || (_itemSlot != null && armor.Slot != _itemSlot))
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<ItemSlot>();
                        itemScript.InitializeAsArmor(armor);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(armor);
                            BindItemToBuyBox(slot);
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Axe:
                case JsonObjects.Items.ItemType.Book:
                case JsonObjects.Items.ItemType.Bow:
                case JsonObjects.Items.ItemType.Crossbow:
                case JsonObjects.Items.ItemType.Dagger:
                case JsonObjects.Items.ItemType.Fist:
                case JsonObjects.Items.ItemType.Gun:
                case JsonObjects.Items.ItemType.Mace:
                case JsonObjects.Items.ItemType.Polearm:
                case JsonObjects.Items.ItemType.Shield:
                case JsonObjects.Items.ItemType.Staff:
                case JsonObjects.Items.ItemType.Sword:
                case JsonObjects.Items.ItemType.Wand:
                case JsonObjects.Items.ItemType.Whip:
                    var weapon = DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(slot.ItemId));
                    bool canAdd = true;
                    if (_itemSlot != null && weapon.Slot != _itemSlot)
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
                            ItemDetails.GetComponent<ItemDetails>().ShowWeaponsDetails(weapon);
                            BindItemToBuyBox(slot);
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Bag:
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<ItemSlot>();
                        var bag = DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(slot.ItemId));
                        itemScript.InitializeAsBag(bag);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            ItemDetails.GetComponent<ItemDetails>().ShowBagDetails(bag);
                            BindItemToBuyBox(slot);
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Consumable:
                    var consumable = DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(slot.ItemId));
                    if (_effectType == null || (_effectType != null && consumable.Effects.Where(e => e.Type == _effectType).Any()))
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<ItemSlot>();
                        itemScript.InitializeAsConsumable(consumable);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            ItemDetails.GetComponent<ItemDetails>().ShowConsumableDetails(consumable);
                            BindItemToBuyBox(slot);
                        });
                    }
                    break;
            }
        }

        _pagination.SetIndicator(_currentPage, (_slots.Count / ItemPerPage) + (_slots.Count % ItemPerPage > 0 ? 1 : 0));
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

    private void SetupPagination()
    {
        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();

            _pagination.Prev.onClick.AddListener(() =>
            {
                if (_currentPage - 1 == 1)
                {
                    _pagination.Prev.gameObject.SetActive(false);
                }

                _pagination.Next.gameObject.SetActive(true);
                --_currentPage;

                SetItems();
            });

            _pagination.Next.onClick.AddListener(() =>
            {
                if ((_slots.Count - ((_currentPage + 1) * ItemPerPage)) <= 0)
                {
                    _pagination.Next.gameObject.SetActive(false);
                }

                _pagination.Prev.gameObject.SetActive(true);
                ++_currentPage;

                SetItems();
            });
        }

        _currentPage = 1;
        _pagination.Prev.gameObject.SetActive(false);
        _pagination.Next.gameObject.SetActive(_slots.Count > ItemPerPage);
    }
}
