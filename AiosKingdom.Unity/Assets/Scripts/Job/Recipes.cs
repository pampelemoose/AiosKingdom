using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Recipes : MonoBehaviour, ICallbackHooker
{
    public Button Craft;
    public CraftPanel CraftPanel;

    public Text JobRank;
    public Text JobPoints;

    public GameObject Content;
    public GameObject RecipeListItem;
    public RecipeDetails RecipeDetails;

    [Header("Pagination")]
    public GameObject PaginationPrefab;
    public GameObject PaginationBox;
    public int ItemPerPage = 5;

    private Pagination _pagination;

    private List<JsonObjects.Recipe> _recipes;
    private List<JsonObjects.RecipeUnlocked> _unlocked;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Job.Get, (message) =>
        {
            if (message.Success)
            {
                var job = JsonConvert.DeserializeObject<JsonObjects.Job>(message.Json);

                if (job != null)
                {
                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        LoadRecipes(job);
                    });
                }
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Job.Craft, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                var item = JsonConvert.DeserializeObject<JsonObjects.Items.Item>(message.Json);

                NetworkManager.This.GetJob();
                NetworkManager.This.AskInventory();

                if (item != null)
                {
                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        UIManager.This.ShowAlert(string.Format("You successfully crafted < {0} >", item.Name), "Crafting Successful");
                    });
                }
            }
            else
            {
                Debug.Log("Job Craft error : " + message.Json);
            }
        });

        InputController.This.AddCallback("Recipes", (direction) =>
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

    }

    public void LoadRecipes(JsonObjects.Job job)
    {
        JobRank.text = string.Format(": {0}", job.Rank);
        JobPoints.text = string.Format(": {0}", job.Points);

        _recipes = DatasManager.Instance.Recipes.Where(r => r.JobType == job.Type).ToList();
        _unlocked = job.Recipes;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _recipes.Count, _setRecipes);

        _setRecipes();
    }

    private void _setRecipes()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var items = _recipes.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var recipe in items)
        {
            var advObj = Instantiate(RecipeListItem, Content.transform);
            var unlocked = _unlocked.FirstOrDefault(u => u.RecipeId.Equals(recipe.Id));

            var script = advObj.GetComponent<RecipeListItem>();
            script.SetDatas(recipe, unlocked);
            script.Action.onClick.AddListener(() =>
            {
                if (unlocked != null)
                {
                    RecipeDetails.SetDatas(recipe);
                    InputController.This.SetId("RecipeDetails");
                }
            });
            script.Craft.onClick.AddListener(() =>
            {
                InputController.This.SetId("Craft");
                UIManager.This.ShowLoading();
                CraftPanel.ShowCraft(recipe, unlocked != null);
            });
        }

        _pagination.SetIndicator((_recipes.Count / ItemPerPage) + (_recipes.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
