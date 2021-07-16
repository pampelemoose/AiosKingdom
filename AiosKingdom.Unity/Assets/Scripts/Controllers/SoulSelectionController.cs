using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoulSelectionController : PaginationBox
{
    [Header("Soul Selection")]
    public Button CreateSoul;

    private List<JsonObjects.SoulInfos> _souls;

    private void Start()
    {
        CreateSoul.onClick.RemoveAllListeners();
        CreateSoul.onClick.AddListener(() =>
        {
            UIManager.This.ShowCreateSoul();
        });
    }

    public void SetSouls(List<JsonObjects.SoulInfos> souls)
    {
        _souls = souls;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _souls.Count, SetSoulList);

        SetSoulList();
    }

    private void SetSoulList()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var paginatedSouls = _souls.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var soul in paginatedSouls)
        {
            var soulItem = Instantiate(ListItemPrefab, List.transform);

            var script = soulItem.GetComponent<SoulListItem>();
            script.SetDatas(soul);
        }

        _pagination.SetIndicator((_souls.Count / ItemPerPage) + (_souls.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
