using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdventureBagSetup : PaginationBox
{
    [Space(10)]
    [Header("Bag Setup")]
    public Text BagSlotCount;
    public Button AddItemButton;

    public Button EnterDungeonButton;
    public Button CloseButton;

    public ItemDetails ItemDetails;

    public BagItemSelection BagItemSelection;

    private List<JsonObjects.AdventureState.BagItem> _bag;

    private int _bagSize;
    private int _currentBagSlotCount = 0;

    private void Awake()
    {
        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void SetDatas(JsonObjects.Adventures.Adventure adventure)
    {
        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(5, 0, SetItems);

        AddItemButton.onClick.RemoveAllListeners();
        AddItemButton.onClick.AddListener(() =>
        {
            BagItemSelection.Initialize(AddItem);
        });

        _bagSize = DatasManager.Instance.Datas.BagSpace;
        _bag = new List<JsonObjects.AdventureState.BagItem>();

        BagSlotCount.text = $"{_currentBagSlotCount} / {_bagSize}";

        EnterDungeonButton.onClick.RemoveAllListeners();
        //EnterDungeonButton.onClick.AddListener(() =>
        //{
        //    gameObject.SetActive(false);

        //    UIManager.This.ShowLoading();

        //    NetworkManager.This.EnterDungeon(adventure.Id, _bag);
        //});        
        EnterDungeonButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);

            UIManager.This.ShowAdventureWorld();
        });

        SetItems();

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    private void AddItem(JsonObjects.Items.Item item, int quantity)
    {
        if (_currentBagSlotCount + (quantity * item.Space) <= _bagSize)
        {
            var inventorySlot = BagItemSelection.Inventory.FirstOrDefault(i => i.ItemId.Equals(item.Id));
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

            BagItemSelection.TakeItem(item, quantity);

            _bag.Add(exists);

            _currentBagSlotCount += (quantity * item.Space);
            BagSlotCount.text = $"{_currentBagSlotCount} / {_bagSize}";

            SetItems();
        }
    }

    private void SetItems()
    {
        foreach (Transform child in List.transform)
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
            var itemObj = Instantiate(ListItemPrefab, List.transform);
            var itemScript = itemObj.GetComponent<AdventureBagListItem>();
            itemScript.Initialize(item, slot);

            itemScript.Action.onClick.AddListener(() =>
            {
                ItemDetails.ShowDetails(item);
            });

            itemScript.Remove.onClick.AddListener(() =>
            {
                _currentBagSlotCount -= (slot.Quantity * item.Space);
                BagSlotCount.text = $"{_currentBagSlotCount} / {_bagSize}";

                BagItemSelection.PutBack(slot.InventoryId, slot.Quantity);
                _bag.Remove(slot);
                Destroy(itemObj);
            });

        }

        _pagination.SetIndicator((_bag.Count / ItemPerPage) + (_bag.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
