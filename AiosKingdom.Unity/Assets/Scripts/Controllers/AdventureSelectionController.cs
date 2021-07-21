using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdventureSelectionController : PaginationBox
{
    [Header("Adventure Selection")]
    public AdventureBagSetup BagSetup;
    public Button CloseButton;

    private List<JsonObjects.Adventures.Adventure> _adventures;

    void Awake()
    {
        _adventures = DatasManager.Instance.Dungeons;

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(() =>
        {
            UIManager.This.ShowMain();
        });

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _adventures.Count, SetAdventures);

        SetAdventures();
    }

    private void SetAdventures()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var items = _adventures.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var adventure in items)
        {
            var advObj = Instantiate(ListItemPrefab, List.transform);

            var script = advObj.GetComponent<AdventureSelectionListItem>();
            script.SetDatas(adventure);
            script.Action.onClick.AddListener(() =>
            {
                BagSetup.SetDatas(adventure);
            });
        }

        _pagination.SetIndicator((_adventures.Count / ItemPerPage) + (_adventures.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
