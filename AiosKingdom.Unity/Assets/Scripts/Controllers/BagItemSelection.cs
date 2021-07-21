using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BagItemSelection : PaginationBox
{
    [Space(10)]
    [Header("Item Selection")]
    public Button CloseButton;
    public ItemDetails ItemDetails;

    private List<JsonObjects.InventorySlot> _inventory;
    public List<JsonObjects.InventorySlot> Inventory { get { return _inventory; } }

    private Action<JsonObjects.Items.Item, int> _addItemToBag;

    public void Initialize(Action<JsonObjects.Items.Item, int> addItemToBag)
    {
        _addItemToBag = addItemToBag;
        _inventory = DatasManager.Instance.Inventory.ToList();

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _inventory.Count, SetItems);

        SetItems();

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public void TakeItem(JsonObjects.Items.Item item, int quantity)
    {
        var slot = _inventory.FirstOrDefault(i => i.ItemId.Equals(item.Id));
        if (slot != null)
        {
            _inventory.Remove(slot);
            slot.Quantity -= quantity;

            if (slot.Quantity > 0)
            {
                _inventory.Add(slot);
            }
        }

        SetItems();
    }

    public void PutBack(Guid inventoryId, int quantity)
    {
        var slot = _inventory.FirstOrDefault(i => i.Id.Equals(inventoryId));
        if (slot != null)
        {
            _inventory.Remove(slot);
            slot.Quantity += quantity;

            if (slot.Quantity > 0)
            {
                _inventory.Add(slot);
            }
        }
        else
        {
            slot = DatasManager.Instance.Inventory.FirstOrDefault(i => i.Id.Equals(inventoryId));
            slot.Quantity = quantity;
            _inventory.Add(slot);
        }

        SetItems();
    }

    private void SetItems()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var inventoryIds = _inventory.Select(i => i.ItemId).ToList();
        var consumables = DatasManager.Instance.Items.Where(i => inventoryIds.Contains(i.Id) && i.Type == JsonObjects.Items.ItemType.Consumable).ToList();

        foreach (var item in consumables)
        {
            var slot = DatasManager.Instance.Inventory.FirstOrDefault(i => i.ItemId.Equals(item.Id));
            var itemObj = Instantiate(ListItemPrefab, List.transform);
            var itemScript = itemObj.GetComponent<BagSelectionListItem>();
            itemScript.Initialize(item, slot);

            itemScript.Action.onClick.AddListener(() =>
            {
                ItemDetails.ShowDetails(item);
            });

            itemScript.AddToBag.onClick.AddListener(() =>
            {
                _addItemToBag(item, 1);
            });
        }

        _pagination.SetIndicator((_inventory.Count / ItemPerPage) + (_inventory.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
