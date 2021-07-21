using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeListItem : MonoBehaviour
{
    public Image BorderImage;
    public Text NameText;
    public Text QualityText;
    public Text TalentPointsText;
    public Button ShowDetailsButton;

    public void SetDatas(JsonObjects.Skills.Book book, int talentPoints)
    {
        NameText.text = book.Name;
        BorderImage.color = UIManager.BookQualityColor[book.Quality];
        QualityText.color = UIManager.BookQualityColor[book.Quality];
        QualityText.text = book.Quality.ToString();
        TalentPointsText.text = talentPoints.ToString();
    }
}
