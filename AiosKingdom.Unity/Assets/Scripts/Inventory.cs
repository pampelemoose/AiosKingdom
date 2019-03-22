using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, ICallbackHooker
{
    [Space(10)]
    [Header("Content")]
    public GameObject Items;
    public GameObject ItemListItem;
    public GameObject ItemDetails;

    [Space(10)]
    [Header("Pagination")]
    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private List<JsonObjects.InventorySlot> _inventory;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.Inventory, (message) =>
        {
            if (message.Success)
            {
                var inventory = JsonConvert.DeserializeObject<List<JsonObjects.InventorySlot>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _updateItems(inventory);
                });
            }
            else
            {
                Debug.Log("Inventory error : " + message.Json);
            }
        });

        InputController.This.AddCallback("Inventory", (direction) =>
        {
            if (gameObject.activeSelf)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    if (direction == SwipeDirection.Down)
                        GetComponent<Page>().CloseAction();
                });
            }
        });
    }

    private void _updateItems(List<JsonObjects.InventorySlot> inventory)
    {
        _inventory = inventory;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _inventory.Count, SetItems);

        SetItems();
    }

    private void SetItems()
    {
        foreach (Transform child in Items.transform)
        {
            Destroy(child.gameObject);
        }

        var inventoryList = _inventory.OrderByDescending(i => i.LootedAt).Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();
        var inventoryIds = inventoryList.Select(s => s.ItemId).ToList();
        var items = DatasManager.Instance.Items.Where(a => inventoryIds.Contains(a.Id)).ToList();

        foreach (var slot in inventoryList)
        {
            var item = items.FirstOrDefault(m => m.Id.Equals(slot.ItemId));

            if (item == null) continue;

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
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<InventoryListItem>();
                        itemScript.Initialize(item, slot);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            ItemDetails.GetComponent<ItemDetails>().ShowDetails(item);
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.Consumable:
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<InventoryListItem>();
                        itemScript.Initialize(item, slot);

                        itemScript.Action.onClick.AddListener(() =>
                        {
                            ItemDetails.GetComponent<ItemDetails>().ShowDetails(item);
                        });
                    }
                    break;
                case JsonObjects.Items.ItemType.CraftingMaterial:
                case JsonObjects.Items.ItemType.Enchant:
                case JsonObjects.Items.ItemType.Gem:
                    {
                        var itemObj = Instantiate(ItemListItem, Items.transform);
                        var itemScript = itemObj.GetComponent<InventoryListItem>();
                        itemScript.Initialize(item, slot);
                    }
                    break;
            }
        }

        _pagination.SetIndicator((_inventory.Count / ItemPerPage) + (_inventory.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
