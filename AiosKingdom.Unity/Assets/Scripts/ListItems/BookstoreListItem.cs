using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookstoreListItem : MonoBehaviour
{
    public Image Border;
    public Text Name;
    public Text Quality;

    public Button ShowDetailsButton;

    public void SetDatas(JsonObjects.Skills.Book book)
    {
        Name.text = book.Name;
        Border.color = UIManager.BookQualityColor[book.Quality];
        Quality.color = UIManager.BookQualityColor[book.Quality];
        Quality.text = book.Quality.ToString();
    }
}
