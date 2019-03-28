using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftPanel : MonoBehaviour, ICallbackHooker
{
    public Text CraftItem;
    public Button ShowRecipe;
    public RecipeDetails RecipeDetails;

    public Button AddItemButton;
    public ItemSelection BagItemSelection;

    public GameObject Items;
    public GameObject ItemListPrefab;

    public Dropdown Techniques;
    public Button Craft;

    [Space(10)]
    [Header("Pagination")]
    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private List<JsonObjects.CraftingComponent> _craftingItems;

    private Guid _craftId;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Job.Craft, (message) =>
        {
            if (message.Success)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    GetComponent<Page>().CloseAction();
                });
            }
        });

        InputController.This.AddCallback("Craft", (direction) =>
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
    void Start()
    {
        AddItemButton.onClick.AddListener(() =>
        {
            BagItemSelection.Initialize(AddItem, new List<JsonObjects.Items.ItemType> { JsonObjects.Items.ItemType.CraftingMaterial });
        });

        Techniques.AddOptions(Enum.GetNames(typeof(JsonObjects.JobTechnique)).ToList());

        Craft.onClick.AddListener(() =>
        {
            if (Techniques.value > 0 && !Guid.Empty.Equals(_craftId) && _craftingItems.Count > 0)
            {
                var technique = (JsonObjects.JobTechnique)Enum.Parse(typeof(JsonObjects.JobTechnique), Techniques.options[Techniques.value].text);

                UIManager.This.ShowLoading();
                NetworkManager.This.CraftItem(_craftId, technique, _craftingItems);

                Techniques.value = 0;
                _craftId = Guid.Empty;
                _craftingItems = new List<JsonObjects.CraftingComponent>();
                SetItems();
            }
        });

        SetDatas();
    }

    public void ShowCraft(JsonObjects.Recipe recipe, bool showDetails)
    {
        _craftId = recipe.Id;

        CraftItem.text = string.Format(": {0}", recipe.Name);

        ShowRecipe.onClick.RemoveAllListeners();
        if (showDetails)
        {
            ShowRecipe.onClick.AddListener(() =>
            {
                RecipeDetails.SetDatas(recipe);
            });
        }

        gameObject.SetActive(true);
        //transform.SetAsLastSibling();
        UIManager.This.HideLoading();
    }

    public void SetDatas()
    {
        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(5, 0, SetItems);

        _craftingItems = new List<JsonObjects.CraftingComponent>();
    }

    private void AddItem(JsonObjects.Items.Item item, int quantity)
    {
        var inventorySlot = BagItemSelection.Inventory.FirstOrDefault(i => i.ItemId.Equals(item.Id));
        var exists = _craftingItems.FirstOrDefault(b => b.ItemId.Equals(item.Id));
        if (exists == null)
        {
            exists = new JsonObjects.CraftingComponent
            {
                ItemId = item.Id,
                InventoryId = inventorySlot.Id,
                Quantity = 0,
            };
        }
        else
            _craftingItems.Remove(exists);

        exists.Quantity += quantity;

        BagItemSelection.TakeItem(item, quantity);

        _craftingItems.Add(exists);

        SetItems();
    }

    private void SetItems()
    {
        foreach (Transform child in Items.transform)
        {
            Destroy(child.gameObject);
        }

        var craftList = _craftingItems.ToList();
        var craftIds = craftList.Select(s => s.ItemId).ToList();
        var items = DatasManager.Instance.Items.Where(a => craftIds.Contains(a.Id)).ToList();

        items = items.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var item in items)
        {
            var slot = craftList.FirstOrDefault(m => m.ItemId.Equals(item.Id));
            var itemObj = Instantiate(ItemListPrefab, Items.transform);
            var itemScript = itemObj.GetComponent<CraftListItem>();
            itemScript.Initialize(item, slot);

            //itemScript.Action.onClick.AddListener(() =>
            //{
            //    ItemDetails.ShowDetails(item);
            //});

            itemScript.Remove.onClick.AddListener(() =>
            {
                BagItemSelection.PutBack(slot.InventoryId, slot.Quantity);
                _craftingItems.Remove(slot);
                Destroy(itemObj);
            });

        }

        _pagination.SetIndicator((_craftingItems.Count / ItemPerPage) + (_craftingItems.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
