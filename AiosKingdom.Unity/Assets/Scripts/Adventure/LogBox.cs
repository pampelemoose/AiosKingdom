using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LogBox : MonoBehaviour
{
    public GameObject List;
    public GameObject LogListItem;

    [Header("Pagination")]
    public GameObject PaginationPrefab;
    public GameObject PaginationBox;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private Stack<JsonObjects.AdventureState.ActionResult> _logs = new Stack<JsonObjects.AdventureState.ActionResult>();

    public void ClearLogs()
    {
        _logs.Clear();
    }

    public void ShowLogs()
    {
        gameObject.SetActive(true);

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }

        _pagination.Setup(ItemPerPage, _logs.Count, SetLogs);

        SetLogs();
    }

    public void AddLogs(JsonObjects.AdventureState.ActionResult result)
    {
        _logs.Push(result);
    }

    private void SetLogs()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var logs = _logs.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var log in logs)
        {
            var newObj = Instantiate(LogListItem, List.transform);
            var script = newObj.GetComponent<LogListItem>();
            script.SetDatas(log);
        }

        _pagination.SetIndicator((_logs.Count / ItemPerPage) + (_logs.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
