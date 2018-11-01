using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookListItem : MonoBehaviour
{
    public Text Name;
    public Text Quality;
    public Text Rank;
    public GameObject Types;
    public GameObject TypePrefab;
    public GameObject Stats;
    public GameObject StatPrefab;

    public Button ShowDetailsButton;

    public void SetDatas(JsonObjects.Skills.Book book)
    {
        Name.text = book.Name;
        Quality.text = book.Quality.ToString();
        Rank.text = book.Pages.Select(p => p.Rank).Max().ToString();

        var inscriptions = book.Pages.SelectMany(p => p.Inscriptions).OrderByDescending(i => i.Ratio);
        var types = inscriptions.Select(i => i.Type).Distinct().Take(4).ToList();
        var stats = inscriptions.Select(i => i.StatType).Distinct().Take(4).ToList();

        foreach (var type in types)
        {
            var txtObj = Instantiate(TypePrefab, Types.transform);
            var script = txtObj.GetComponentInChildren<Text>();
            script.text = type.ToString();
        }

        foreach (var stat in stats)
        {
            var txtObj = Instantiate(TypePrefab, Stats.transform);
            var script = txtObj.GetComponentInChildren<Text>();
            script.text = stat.ToString();
        }
    }
}
