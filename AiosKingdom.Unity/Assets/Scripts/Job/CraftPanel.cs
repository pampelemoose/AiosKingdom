using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftPanel : MonoBehaviour
{
    public Button AddItemButton;
    public ItemSelection BagItemSelection;

    public GameObject Items;
    public GameObject ItemListPrefab;

    public Dropdown Techniques;
    public Text CraftResult;
    public Button Craft;

    [Space(10)]
    [Header("Pagination")]
    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private List<JsonObjects.CraftingComponent> _craftingItems;

    void Start()
    {
        Techniques.AddOptions(Enum.GetNames(typeof(JsonObjects.JobTechnique)).ToList());
        Techniques.onValueChanged.AddListener((value) =>
        {
            SetCraftingResult();
        });

        AddItemButton.onClick.RemoveAllListeners();
        AddItemButton.onClick.AddListener(() =>
        {
            BagItemSelection.Initialize(AddItem, new List<JsonObjects.Items.ItemType> { JsonObjects.Items.ItemType.CraftingMaterial });
        });

        Craft.onClick.RemoveAllListeners();
        Craft.onClick.AddListener(() =>
        {
            if (Techniques.value > 0)
            {
                var technique = (JsonObjects.JobTechnique)Enum.Parse(typeof(JsonObjects.JobTechnique), Techniques.options[Techniques.value].text);

                NetworkManager.This.CraftItem(technique, _craftingItems);

                Techniques.value = 0;
                _craftingItems = new List<JsonObjects.CraftingComponent>();
                SetItems();
                SetCraftingResult();
            }
        });

        SetDatas();
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
        SetCraftingResult();
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
                SetCraftingResult();
                Destroy(itemObj);
            });

        }

        _pagination.SetIndicator((_craftingItems.Count / ItemPerPage) + (_craftingItems.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void SetCraftingResult()
    {
        var resultString = "Unknown";

        if (Techniques.value > 0)
        {
            var technique = (JsonObjects.JobTechnique)Enum.Parse(typeof(JsonObjects.JobTechnique), Techniques.options[Techniques.value].text);
            var knownRecipesIds = DatasManager.Instance.Job.Value.Recipes.Select(r => r.RecipeId).ToList();
            var availableRecipes = DatasManager.Instance.Recipes.Where(r => r.Technique == technique && knownRecipesIds.Contains(r.Id)).ToList();

            if (availableRecipes.Count > 0)
            {
                foreach (var recipe in availableRecipes)
                {
                    var isFulfilled = true;
                    foreach (var crafting in _craftingItems)
                    {
                        var item = DatasManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(crafting.ItemId));
                        var combinaison = recipe.Combinaisons.FirstOrDefault(c => c.ItemId(item.Quality).Equals(item.Id));
                        if (combinaison == null
                            || (combinaison != null && combinaison.MinQuantity > crafting.Quantity && combinaison.MaxQuantity < crafting.Quantity))
                        {
                            isFulfilled = false;
                            break;
                        }
                    }

                    if (isFulfilled)
                    {
                        resultString = recipe.Name;
                        break;
                    }
                }
            }
        }

        CraftResult.text = string.Format("<{0}>", resultString);
    }
}
