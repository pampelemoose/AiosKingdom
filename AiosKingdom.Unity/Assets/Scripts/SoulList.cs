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
    private int _currentPage = 1;

    private List<JsonObjects.SoulInfos> _souls;

    public void SetSouls(List<JsonObjects.SoulInfos> souls)
    {
        _souls = souls;

        SetupPagination();

        SetSoulList();
    }

    private void SetSoulList()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var paginatedSouls = _souls.Skip((_currentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var soul in paginatedSouls)
        {
            var soulItem = Instantiate(SoulListItemPrefab, List.transform);

            var script = soulItem.GetComponent<SoulListItem>();
            script.SetDatas(soul);
        }

        var createItem = Instantiate(SoulListCreateItemPrefab, List.transform);
        var createScript = createItem.GetComponent<SoulListCreateItem>();

        _pagination.SetIndicator(_currentPage, (_souls.Count / ItemPerPage) + (_souls.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void SetupPagination()
    {
        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();

            _pagination.Prev.onClick.AddListener(() =>
            {
                if (_currentPage - 1 == 1)
                {
                    _pagination.Prev.gameObject.SetActive(false);
                }

                _pagination.Next.gameObject.SetActive(true);
                --_currentPage;

                SetSoulList();
            });

            _pagination.Next.onClick.AddListener(() =>
            {
                if ((_souls.Count - ((_currentPage + 1) * ItemPerPage)) <= 0)
                {
                    _pagination.Next.gameObject.SetActive(false);
                }

                _pagination.Prev.gameObject.SetActive(true);
                ++_currentPage;

                SetSoulList();
            });
        }

        _currentPage = 1;
        _pagination.Prev.gameObject.SetActive(false);
        _pagination.Next.gameObject.SetActive(_souls.Count > ItemPerPage);
    }
}
