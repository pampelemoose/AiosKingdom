using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSelection : MonoBehaviour
{
    public GameObject Items;
    public GameObject BagItemListPrefab;

    public ItemDetails ItemDetails;

    [Space(10)]
    [Header("Pagination")]
    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;

    private List<JsonObjects.InventorySlot> _inventory;
    public List<JsonObjects.InventorySlot> Inventory { get { return _inventory; } }

    private Action<JsonObjects.Items.Item, int> _addItemToBag;
    private List<JsonObjects.Items.ItemType> _allowedTypes;

    public void Initialize(Action<JsonObjects.Items.Item, int> addItemToBag, List<JsonObjects.Items.ItemType> allowedTypes)
    {
        _addItemToBag = addItemToBag;
        _allowedTypes = allowedTypes;
        _inventory = DatasManager.Instance.Inventory.ToList();

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _inventory.Count, SetItems);

        SetItems();

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
        foreach (Transform child in Items.transform)
        {
            Destroy(child.gameObject);
        }

        var inventoryIds = _inventory.Select(i => i.ItemId).ToList();
        var consumables = DatasManager.Instance.Items.Where(i => inventoryIds.Contains(i.Id) && _allowedTypes.Contains(i.Type)).ToList();

        foreach (var item in consumables)
        {
            var slot = DatasManager.Instance.Inventory.FirstOrDefault(i => i.ItemId.Equals(item.Id));
            var itemObj = Instantiate(BagItemListPrefab, Items.transform);
            var itemScript = itemObj.GetComponent<SelectionListItem>();
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
