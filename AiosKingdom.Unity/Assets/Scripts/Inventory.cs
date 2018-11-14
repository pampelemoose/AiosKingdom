using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Button Armors;
    public Button Weapons;
    public Button Bags;
    public Button Consumables;

    public GameObject Content;
    public GameObject ItemSlot;

    void Start()
    {
        NetworkManager.This.AskInventory();

        Armors.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            LoadInventory(JsonObjects.Items.ItemType.Armor);
        });

        Weapons.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            LoadInventory(JsonObjects.Items.ItemType.Weapon);
        });

        Bags.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            LoadInventory(JsonObjects.Items.ItemType.Bag);
        });

        Consumables.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            LoadInventory(JsonObjects.Items.ItemType.Consumable);
        });
    }

    public void UpdateItems()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        Armors.interactable = true;
        Weapons.interactable = true;
        Bags.interactable = true;
        Consumables.interactable = true;
    }

    private void LoadInventory(JsonObjects.Items.ItemType type)
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var slotList = DatasManager.Instance.Inventory.Where(s => s.Type == type).Select(s => s.ItemId).ToList();
        switch (type)
        {
            case JsonObjects.Items.ItemType.Armor:
                {
                    var items = DatasManager.Instance.Armors.Where(a => slotList.Contains(a.Id)).ToList();

                    foreach (var item in items)
                    {
                        var armObj = Instantiate(ItemSlot, Content.transform);

                        var script = armObj.GetComponent<ItemSlot>();
                        script.InitializeAsArmor(item);
                    }
                }
                break;
            case JsonObjects.Items.ItemType.Weapon:
                {
                    var items = DatasManager.Instance.Weapons.Where(a => slotList.Contains(a.Id)).ToList();

                    foreach (var item in items)
                    {
                        var armObj = Instantiate(ItemSlot, Content.transform);

                        var script = armObj.GetComponent<ItemSlot>();
                        script.InitializeAsWeapon(item);
                    }
                }
                break;
            case JsonObjects.Items.ItemType.Bag:
                {
                    var items = DatasManager.Instance.Bags.Where(a => slotList.Contains(a.Id)).ToList();

                    foreach (var item in items)
                    {
                        var armObj = Instantiate(ItemSlot, Content.transform);

                        var script = armObj.GetComponent<ItemSlot>();
                        script.InitializeAsBag(item);
                    }
                }
                break;
            case JsonObjects.Items.ItemType.Consumable:
                {
                    var items = DatasManager.Instance.Consumables.Where(a => slotList.Contains(a.Id)).ToList();

                    foreach (var item in items)
                    {
                        var armObj = Instantiate(ItemSlot, Content.transform);

                        var script = armObj.GetComponent<ItemSlot>();
                        script.InitializeAsConsumable(item);
                    }
                }
                break;
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(Content));

        UIManager.This.HideLoading();
    }
}
