using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Market : MonoBehaviour, ICallbackHooker
{
    public Dropdown ItemTypeDropdown;

    public Text FirstFilterLabel;
    public Dropdown FirstFilterDropdown;
    public Button Specials;
    public Button Normals;

    [Space(10)]
    [Header("Content")]
    public GameObject Items;
    public GameObject ItemListItem;
    public ItemDetails ItemDetails;

    [Header("Pagination")]
    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    [Space(10)]
    [Header("BuyBox")]
    public GameObject BuyBox;
    public Text Price;
    public Button BuyItemButton;
    public Button CloseBuyBox;

    private JsonObjects.Items.ItemType? _itemType;
    private JsonObjects.Items.ItemSlot? _itemSlot;
    private JsonObjects.Items.EffectType? _effectType;

    private Pagination _pagination;
    private List<JsonObjects.MarketSlot> _slots;

    private bool _init = false;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Listing.Market, (message) =>
        {
            if (message.Success)
            {
                var items = JsonConvert.DeserializeObject<List<JsonObjects.MarketSlot>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _updateItems(items);
                });
            }
            else
            {
                Debug.Log("Market error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Listing.SpecialsMarket, (message) =>
        {
            if (message.Success)
            {
                var items = JsonConvert.DeserializeObject<List<JsonObjects.MarketSlot>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _updateSpecialItems(items);
                });
            }
            else
            {
                Debug.Log("Specials Market error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.Market_PlaceOrder, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                NetworkManager.This.AskCurrencies();
                NetworkManager.This.AskMarketItems();
            }
            else
            {
                Debug.Log("Market Place Order error : " + message.Json);
            }
        });
    }

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

                _resetItems();
            });

            Specials.onClick.AddListener(() =>
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    NetworkManager.This.AskSpecialMarketItems();
                    Specials.gameObject.SetActive(false);
                    Normals.gameObject.SetActive(true);
                });
            });

            Normals.onClick.AddListener(() =>
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    NetworkManager.This.AskMarketItems();
                    Normals.gameObject.SetActive(false);
                    Specials.gameObject.SetActive(true);
                });
            });

            CloseBuyBox.onClick.AddListener(() =>
            {
                BuyBox.SetActive(false);
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

        if (_itemType != null)
        {
            switch (_itemType)
            {
                case JsonObjects.Items.ItemType.Armor:
                    {
                        FirstFilterLabel.gameObject.SetActive(true);
                        FirstFilterDropdown.gameObject.SetActive(true);

                        FirstFilterLabel.text = "Slot";
                        FirstFilterDropdown.ClearOptions();
                        FirstFilterDropdown.AddOptions(new List<string> {
                            "All",
                            JsonObjects.Items.ItemSlot.Belt.ToString(),
                            JsonObjects.Items.ItemSlot.Feet.ToString(),
                            JsonObjects.Items.ItemSlot.Hand.ToString(),
                            JsonObjects.Items.ItemSlot.Head.ToString(),
                            JsonObjects.Items.ItemSlot.Leg.ToString(),
                            JsonObjects.Items.ItemSlot.Pants.ToString(),
                            JsonObjects.Items.ItemSlot.Shoulder.ToString(),
                            JsonObjects.Items.ItemSlot.Torso.ToString()
                        });

                        FirstFilterDropdown.onValueChanged.AddListener((value) =>
                        {
                            _itemSlot = null;
                            if (value > 0)
                            {
                                _itemSlot = (JsonObjects.Items.ItemSlot)Enum.Parse(typeof(JsonObjects.Items.ItemSlot), FirstFilterDropdown.options.ElementAt(value).text);
                            }

                            _resetItems();
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
                    {
                        FirstFilterLabel.gameObject.SetActive(true);
                        FirstFilterDropdown.gameObject.SetActive(true);

                        FirstFilterLabel.text = "Slot";
                        FirstFilterDropdown.ClearOptions();
                        FirstFilterDropdown.AddOptions(new List<string> {
                            "All",
                            JsonObjects.Items.ItemSlot.OneHand.ToString(),
                            JsonObjects.Items.ItemSlot.TwoHand.ToString(),
                        });

                        FirstFilterDropdown.onValueChanged.AddListener((value) =>
                        {
                            _itemSlot = null;
                            if (value > 0)
                            {
                                _itemSlot = (JsonObjects.Items.ItemSlot)Enum.Parse(typeof(JsonObjects.Items.ItemSlot), FirstFilterDropdown.options.ElementAt(value).text);
                            }

                            _resetItems();
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

                            _resetItems();
                        });
                    }
                    break;
            }
        }
    }

    private void _updateItems(List<JsonObjects.MarketSlot> items)
    {
        _slots = items;

        Normals.gameObject.SetActive(false);
        Specials.gameObject.SetActive(true);
        
        _resetItems();
    }

    private void _updateSpecialItems(List<JsonObjects.MarketSlot> items)
    {
        _slots = items;

        Specials.gameObject.SetActive(false);
        Normals.gameObject.SetActive(true);

        _resetItems();
    }

    private void _resetItems()
    {
        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _slots.Count, _setItems);

        _setItems();
    }

    private void _setItems()
    {
        foreach (Transform child in Items.transform)
        {
            Destroy(child.gameObject);
        }

        var marketList = _slots.ToList();
        var marketListIds = marketList.Select(s => s.ItemId).ToList();
        var items = DatasManager.Instance.Items.Where(a => marketListIds.Contains(a.Id)).ToList();

        if (_itemType != null)
        {
            items = items.Where(m => m.Type == _itemType).ToList();
        }
        else
        {
            items = items.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();
        }

        foreach (var item in items)
        {
            var slot = marketList.FirstOrDefault(m => m.ItemId.Equals(item.Id));
            switch (item.Type)
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
                case JsonObjects.Items.ItemType.Bag:
                    if (_itemSlot == null || (_itemSlot != null && item.Slot == _itemSlot))
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<MarketListItem>();
                        itemScript.Initialize(item, slot);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            ItemDetails.ShowDetails(item);
                        });

                        itemScript.Buy.onClick.AddListener(() =>
                        {
                            _bindItemToBuyBox(slot);
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Consumable:
                    if (_effectType == null || (_effectType != null && item.Effects.Where(e => e.Type == _effectType).Any()))
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<MarketListItem>();
                        itemScript.Initialize(item, slot);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            ItemDetails.ShowDetails(item);
                        });

                        itemScript.Buy.onClick.AddListener(() =>
                        {
                            _bindItemToBuyBox(slot);
                        });
                    }
                    break;
            }
        }

        _pagination.SetIndicator((_slots.Count / ItemPerPage) + (_slots.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void _bindItemToBuyBox(JsonObjects.MarketSlot slot)
    {
        BuyBox.SetActive(true);
        Price.text = string.Format("[{0}]", slot.Price);
        BuyItemButton.onClick.RemoveAllListeners();
        BuyItemButton.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowLoading();
                NetworkManager.This.OrderMarketItem(slot.Id);
            });

            BuyBox.SetActive(false);
        });
    }
}
