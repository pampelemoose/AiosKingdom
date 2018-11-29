using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BagSetup : MonoBehaviour
{
    public Text BagSlot;
    public Button AddItemButton;
    public GameObject Items;
    public GameObject BagListPrefab;
    public Button EnterDungeonButton;
    public GameObject ItemDetails;

    public GameObject BagItemSelection;

    [Space(10)]
    [Header("Pagination")]
    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private List<JsonObjects.AdventureState.BagItem> _bag;

    private int _bagSize;
    private int _currentBagSlotCount = 0;

    void Awake()
    {
        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(5, 0, SetItems);

        AddItemButton.onClick.AddListener(() =>
        {
            BagItemSelection.SetActive(true);
            BagItemSelection.transform.SetAsLastSibling();
            BagItemSelection.GetComponent<BagItemSelection>().Initialize(AddItem);
        });

    }

    public void SetDatas(JsonObjects.Adventures.Adventure adventure)
    {
        _bagSize = DatasManager.Instance.Datas.BagSpace;
        _bag = new List<JsonObjects.AdventureState.BagItem>();

        BagSlot.text = string.Format("[{0} / {1}]", _currentBagSlotCount, _bagSize);

        EnterDungeonButton.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();

            NetworkManager.This.EnterDungeon(adventure.Id, _bag);
        });

        SetItems();
    }

    private void AddItem(JsonObjects.Items.Item item, int quantity)
    {
        if (_currentBagSlotCount + quantity < _bagSize)
        {
            var itemSelector = BagItemSelection.GetComponent<BagItemSelection>();
            var inventorySlot = itemSelector.Inventory.FirstOrDefault(i => i.ItemId.Equals(item.Id));
            var exists = _bag.FirstOrDefault(b => b.ItemId.Equals(item.Id));
            if (exists == null)
            {
                exists = new JsonObjects.AdventureState.BagItem
                {
                    InventoryId = inventorySlot.Id,
                    ItemId = item.Id,
                    Quantity = 0,
                };
            }
            else
                _bag.Remove(exists);

            exists.Quantity += quantity;

            itemSelector.TakeItem(item, quantity);

            _bag.Add(exists);

            _currentBagSlotCount += quantity;

            SetItems();
        }
    }

    private void SetItems()
    {
        foreach (Transform child in Items.transform)
        {
            Destroy(child.gameObject);
        }

        var bagList = _bag.ToList();
        var bagIds = bagList.Select(s => s.ItemId).ToList();
        var items = DatasManager.Instance.Items.Where(a => bagIds.Contains(a.Id)).ToList();

        items = items.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var item in items)
        {
            var slot = bagList.FirstOrDefault(m => m.ItemId.Equals(item.Id));
            var itemObj = Instantiate(BagListPrefab, Items.transform);
            var itemScript = itemObj.GetComponent<BagListItem>();
            itemScript.Initialize(item, slot);

            itemScript.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowDetails(item);
            });

            itemScript.Remove.onClick.AddListener(() =>
            {
                BagItemSelection.GetComponent<BagItemSelection>().PutBack(slot.InventoryId, slot.Quantity);
                _bag.Remove(slot);
                Destroy(itemObj);
            });

        }

        _pagination.SetIndicator((_bag.Count / ItemPerPage) + (_bag.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
