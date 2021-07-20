using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookstoreListItem : MonoBehaviour
{
    public Text Name;
    public Text Quality;

    public Button ShowDetailsButton;

    public void SetDatas(JsonObjects.Skills.Book book)
    {
        Name.text = book.Name;
        Quality.text = book.Quality.ToString();
    }
}
