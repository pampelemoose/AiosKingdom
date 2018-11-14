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

    public Button ShowDetailsButton;

    public void SetDatas(JsonObjects.Skills.Book book)
    {
        Name.text = book.Name;
        Quality.text = book.Quality.ToString();
        Rank.text = book.Pages.Select(p => p.Rank).Max().ToString();
    }
}
