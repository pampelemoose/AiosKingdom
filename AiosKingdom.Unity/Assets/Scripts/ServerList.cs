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
    private List<JsonObjects.GameServerInfos> _servers;

    public void SetServers(List<JsonObjects.GameServerInfos> servers)
    {
        _servers = servers;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _servers.Count, SetServerList);

        SetServerList();
    }

    private void SetServerList()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var paginatedServers = _servers.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var server in paginatedServers)
        {
            var serverItem = Instantiate(ServerListItemPrefab, List.transform);

            var script = serverItem.GetComponent<ServerListItem>();
            script.SetDatas(server);
        }

        _pagination.SetIndicator((_servers.Count / ItemPerPage) + (_servers.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
