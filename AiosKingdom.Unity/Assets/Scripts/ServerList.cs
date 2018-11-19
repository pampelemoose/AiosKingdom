using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerList : MonoBehaviour
{
    public GameObject List;
    public GameObject PaginationBox;
    public int ItemPerPage = 5;

    public GameObject ServerListItemPrefab;
    public GameObject PaginationPrefab;

    private Pagination _pagination;
    private int _currentPage = 1;

    private List<JsonObjects.GameServerInfos> _servers;

    public void SetServers(List<JsonObjects.GameServerInfos> servers)
    {
        _servers = servers;

        SetupPagination();

        SetServerList();
    }

    private void SetServerList()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var paginatedServers = _servers.Skip((_currentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var server in paginatedServers)
        {
            var serverItem = Instantiate(ServerListItemPrefab, List.transform);

            var script = serverItem.GetComponent<ServerListItem>();
            script.SetDatas(server);
        }

        _pagination.SetIndicator(_currentPage, (_servers.Count / ItemPerPage) + (_servers.Count % ItemPerPage > 0 ? 1 : 0));
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

                SetServerList();
            });
            
            _pagination.Next.onClick.AddListener(() =>
            {
                if ((_servers.Count - ((_currentPage + 1) * ItemPerPage)) <= 0)
                {
                    _pagination.Next.gameObject.SetActive(false);
                }

                _pagination.Prev.gameObject.SetActive(true);
                ++_currentPage;

                SetServerList();
            });
        }

        _currentPage = 1;
        _pagination.Prev.gameObject.SetActive(false);
        _pagination.Next.gameObject.SetActive(_servers.Count > ItemPerPage);
    }
}
