using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoulList : MonoBehaviour
{
    public GameObject List;
    public GameObject PaginationBox;
    public int ItemPerPage = 5;

    public GameObject SoulListItemPrefab;
    public GameObject SoulListCreateItemPrefab;
    public GameObject PaginationPrefab;

    private Pagination _pagination;
    private List<JsonObjects.SoulInfos> _souls;

    public void SetSouls(List<JsonObjects.SoulInfos> souls)
    {
        _souls = souls;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
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
            var soulItem = Instantiate(SoulListItemPrefab, List.transform);

            var script = soulItem.GetComponent<SoulListItem>();
            script.SetDatas(soul);
        }

        var createItem = Instantiate(SoulListCreateItemPrefab, List.transform);
        //var createScript = createItem.GetComponent<SoulListCreateItem>();

        _pagination.SetIndicator((_souls.Count / ItemPerPage) + (_souls.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
