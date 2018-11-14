using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Market : MonoBehaviour
{
    public Button Home;

    [Header("Armors")]
    public Button Armors;
    public GameObject ArmorTypes;
    public Button Head;
    public Button Shoulder;
    public Button Torso;
    public Button Belt;
    public Button Pants;
    public Button Legs;
    public Button Feet;
    public Button Hands;

    [Space(5)]
    [Header("Weapons")]
    public Button Weapons;
    public GameObject WeaponTypes;
    public Button Fist;
    public Button Dagger;
    public Button Sword;
    public Button Axe;
    public Button Mace;
    public Button Polearm;
    public Button Staff;
    public Button Shield;
    public Button Wand;
    public Button Bow;
    public Button Gun;
    public Button Crossbow;
    public Button Book;
    public Button Whip;

    [Space(5)]
    [Header("Bags")]
    public Button Bags;

    [Space(5)]
    [Header("Consumables")]
    public Button Consumables;

    [Space(10)]
    [Header("Content")]
    public GameObject Items;
    public GameObject ItemListItem;
    public GameObject ItemDetails;

    private JsonObjects.Items.ItemType? _itemType = null;

    void Start()
    {
        NetworkManager.This.AskMarketItems();

        Home.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                SceneManager.LoadScene(1);
            });
        });

        Armors.onClick.AddListener(() =>
        {
            _itemType = JsonObjects.Items.ItemType.Armor;
            ArmorTypes.SetActive(true);
            WeaponTypes.SetActive(false);

            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UpdateItems();
            });
        });

        Weapons.onClick.AddListener(() =>
        {
            _itemType = JsonObjects.Items.ItemType.Weapon;
            ArmorTypes.SetActive(false);
            WeaponTypes.SetActive(true);

            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UpdateItems();
            });
        });

        Bags.onClick.AddListener(() =>
        {
            _itemType = JsonObjects.Items.ItemType.Bag;
            ArmorTypes.SetActive(false);
            WeaponTypes.SetActive(false);

            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UpdateItems();
            });
        });

        Consumables.onClick.AddListener(() =>
        {
            _itemType = JsonObjects.Items.ItemType.Consumable;
            ArmorTypes.SetActive(false);
            WeaponTypes.SetActive(false);

            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UpdateItems();
            });
        });
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
            var itemObj = Instantiate(ItemListItem, Items.transform);

            var itemScript = itemObj.GetComponent<ItemSlot>();

            switch (slot.Type)
            {
                case JsonObjects.Items.ItemType.Armor:
                    itemScript.InitializeAsArmor(DatasManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(slot.ItemId)));
                    break;
                case JsonObjects.Items.ItemType.Weapon:
                    itemScript.InitializeAsWeapon(DatasManager.Instance.Weapons.FirstOrDefault(a => a.Id.Equals(slot.ItemId)));
                    break;
                case JsonObjects.Items.ItemType.Bag:
                    itemScript.InitializeAsBag(DatasManager.Instance.Bags.FirstOrDefault(a => a.Id.Equals(slot.ItemId)));
                    break;
                case JsonObjects.Items.ItemType.Consumable:
                    itemScript.InitializeAsConsumable(DatasManager.Instance.Consumables.FirstOrDefault(a => a.Id.Equals(slot.ItemId)));
                    break;
            }

            var marketScript = itemObj.GetComponent<MarketItem>();
            marketScript.SetDatas(slot);
        }
    }
}
