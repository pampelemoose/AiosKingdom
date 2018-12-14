using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionListItem : MonoBehaviour
{
    public Text Name;
    public Text ManaCost;
    public Button Action;

    public void SetDatas(JsonObjects.Knowledge knowledge)
    {
        var book = DatasManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(knowledge.BookId));

        Name.text = string.Format("{0} (Rank {1})", book.Name, knowledge.Rank);
        ManaCost.text = string.Format("[{0}]", book.Pages.FirstOrDefault(p => p.Rank == knowledge.Rank).ManaCost);
    }
}
