using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TalentChoices : PaginationBox
{
    [Header("Talent Choices")]
    public Button Close;

    private JsonObjects.TalentUnlocked _unlocked;
    private List<JsonObjects.Skills.Talent> _talents;

    private void Awake()
    {
        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void SetDatas(TalentBox talentBox)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        _unlocked = talentBox.Unlocked;
        _talents = talentBox.Talents;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }

        _pagination.Setup(ItemPerPage, _talents.Count, SetTalents);

        SetTalents();
    }

    private void SetTalents()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var talents = _talents.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var talent in talents)
        {
            var book = DatasManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(talent.BookId));
            var talentListObj = Instantiate(ListItemPrefab, List.transform);
            var script = talentListObj.GetComponent<TalentChoiceListItem>();

            script.SetDatas(talent, book.Name);
            script.Action.onClick.AddListener(() =>
            {
                UIManager.This.ShowLoading();
                NetworkManager.This.LearnTalent(talent.Id);
                gameObject.SetActive(false);
            });

            if (_unlocked != null && _unlocked.TalentId.Equals(talent.Id))
            {
                script.Select();
            }
        }

        _pagination.SetIndicator((_talents.Count / ItemPerPage) + (_talents.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
