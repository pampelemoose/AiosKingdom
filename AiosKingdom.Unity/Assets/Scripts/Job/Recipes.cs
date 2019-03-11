using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Recipes : MonoBehaviour
{
    public GameObject Content;
    public GameObject RecipeListItem;

    [Header("Pagination")]
    public GameObject PaginationPrefab;
    public GameObject PaginationBox;
    public int ItemPerPage = 5;

    private Pagination _pagination;

    private List<JsonObjects.Recipe> _recipes;

    public void LoadRecipes(JsonObjects.Job job)
    {
        _recipes = DatasManager.Instance.Recipes.Where(r => r.JobType == job.Type).ToList();

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _recipes.Count, SetRecipes);

        SetRecipes();
    }

    private void SetRecipes()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var items = _recipes.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var recipe in items)
        {
            var advObj = Instantiate(RecipeListItem, Content.transform);

            var script = advObj.GetComponent<RecipeListItem>();
            script.SetDatas(recipe);
            //script.Action.onClick.AddListener(() =>
            //{
            //    BagSetup.SetDatas(adventure);
            //});
        }

        _pagination.SetIndicator((_recipes.Count / ItemPerPage) + (_recipes.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
