using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdventureSelection : MonoBehaviour
{
    public GameObject Content;
    public GameObject AdventureListItem;

    public BagSetup BagSetup;

    [Space(10)]
    [Header("Pagination")]
    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private List<JsonObjects.Adventures.Adventure> _adventures;

    void Awake()
    {
        _adventures = DatasManager.Instance.Dungeons;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _adventures.Count, SetAdventures);

        SetAdventures();
    }

    private void SetAdventures()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var items = _adventures.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var adventure in items)
        {
            var advObj = Instantiate(AdventureListItem, Content.transform);

            var script = advObj.GetComponent<AdventureListItem>();
            script.SetDatas(adventure);
            script.Action.onClick.AddListener(() =>
            {
                BagSetup.SetDatas(adventure);
            });
        }

        _pagination.SetIndicator((_adventures.Count / ItemPerPage) + (_adventures.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
