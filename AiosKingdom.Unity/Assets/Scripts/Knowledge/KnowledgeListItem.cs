using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeListItem : MonoBehaviour
{
    public Text Name;
    public Text Quality;
    public Text Rank;
    public Button ShowDetailsButton;

    public void SetDatas(JsonObjects.Skills.Book book, int rank)
    {
        Name.text = book.Name;
        Quality.text = book.Quality.ToString();
        Rank.text = rank.ToString();
    }
}
