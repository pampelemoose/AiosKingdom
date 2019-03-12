using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoulList : MonoBehaviour, ICallbackHooker
{
    public GameObject List;
    public GameObject PaginationBox;
    public int ItemPerPage = 5;

    public GameObject SoulListItemPrefab;
    public GameObject SoulListCreateItemPrefab;
    public GameObject PaginationPrefab;

    private Pagination _pagination;
    private List<JsonObjects.SoulInfos> _souls;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Server.SoulList, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();

            });
            if (message.Success)
            {
                var souls = JsonConvert.DeserializeObject<List<JsonObjects.SoulInfos>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    gameObject.SetActive(true);
                    _setSouls(souls);
                });
            }
            else
            {
                Debug.Log("SoulList error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Server.CreateSoul, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.AskSoulList();
            }
            else
            {
                Debug.Log("New Soul error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Server.ConnectSoul, (message) =>
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

    private void _setSouls(List<JsonObjects.SoulInfos> souls)
    {
        _souls = souls;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _souls.Count, _setSoulList);

        _setSoulList();
    }

    private void _setSoulList()
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

        _pagination.SetIndicator((_souls.Count / ItemPerPage) + (_souls.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
