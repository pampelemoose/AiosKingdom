using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerList : MonoBehaviour, ICallbackHooker
{
    public GameObject List;
    public GameObject PaginationBox;
    public int ItemPerPage = 5;

    public GameObject ServerListItemPrefab;
    public GameObject PaginationPrefab;

    private Pagination _pagination;
    private List<JsonObjects.GameServerInfos> _servers;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Client_ServerList, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                var servers = JsonConvert.DeserializeObject<List<JsonObjects.GameServerInfos>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    gameObject.SetActive(true);
                    _setServers(servers);
                });
            }
            else
            {
                Debug.Log("ServerList error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Client_AnnounceGameConnection, (message) =>
        {
            if (!message.Success)
            {
                Debug.Log("AnnounceGameConnection error : " + message.Json);
                NetworkManager.This.AskServerList();
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Game_Authenticate, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.AskSoulList();
            }
            else
            {
                Debug.Log("Authenticate error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Server.SoulList, (message) =>
        {
            if (message.Success)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    gameObject.SetActive(false);
                });
            }
        });
    }

    private void _setServers(List<JsonObjects.GameServerInfos> servers)
    {
        _servers = servers;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _servers.Count, _setServerList);

        _setServerList();
    }

    private void _setServerList()
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
