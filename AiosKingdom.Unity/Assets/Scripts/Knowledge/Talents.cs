using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Talents : MonoBehaviour
{
    [Serializable]
    public class Branch
    {
        public Button[] Talents;
    }

    public TalentChoices TalentChoices;

    public GameObject First;
    public GameObject Second;
    public Button Previous;
    public Button Next;

    [Header("Tree")]
    public Branch[] Branches;

    void Awake()
    {
        Previous.onClick.RemoveAllListeners();
        Previous.onClick.AddListener(() =>
        {
            First.SetActive(true);
            Second.SetActive(false);
        });

        Previous.gameObject.SetActive(false);

        Next.onClick.RemoveAllListeners();
        Next.onClick.AddListener(() =>
        {
            First.SetActive(false);
            Second.SetActive(true);
        });

        Next.gameObject.SetActive(false);
    }

    public void ShowTalents()
    {
        gameObject.SetActive(true);
        //transform.SetAsLastSibling();
        UIManager.This.HideLoading();
    }

    public void LoadTalents()
    {
        var talentUnlocked = DatasManager.Instance.Knowledges.SelectMany(k => k.Talents).ToList();
        var knowledgesBookIds = DatasManager.Instance.Knowledges.Select(t => t.BookId).ToList();
        var talents = DatasManager.Instance.Books.Where(b => knowledgesBookIds.Contains(b.Id)).SelectMany(t => t.Talents).ToList();

        var disabledColor = new Color(1, 1, 1, 0.1f);
        var learnedColor = new Color(1, 1, 1, 1);
        var canLearnColor = new Color(0, 0.5f, 0.5f, 1);
        var availableColor = new Color(0.5f, 0.5f, 0, 0.7f);

        for (int i = 0; i < 12; ++i)
        {
            for (int j = 0; j < 30; ++j)
            {
                var talentButton = Branches[i].Talents[j];

                talentButton.interactable = false;
                talentButton.GetComponentInChildren<Text>().color = disabledColor;
                talentButton.onClick.RemoveAllListeners();

                var leafTalents = talents.Where(t => t.Branch == i && t.Leaf == j).ToList();
                var leafUnlocked = talentUnlocked.FirstOrDefault(u => leafTalents.Select(t => t.Id).Contains(u.TalentId));

                if (leafUnlocked != null)
                {
                    talentButton.interactable = true;
                    talentButton.GetComponentInChildren<Text>().color = learnedColor;
                    SetTalentBox(talentButton,
                            i, j,
                            leafUnlocked, leafTalents);

                    talentButton.onClick.AddListener(() =>
                    {
                        TalentChoices.SetDatas(talentButton.GetComponent<TalentBox>());
                    });
                    continue;
                }

                var leafAvailableTalents = new List<JsonObjects.Skills.Talent>();
                foreach (var leafTal in leafTalents)
                {
                    var knowledge = DatasManager.Instance.Knowledges.FirstOrDefault(k => k.BookId.Equals(leafTal.BookId));
                    if (leafTal.TalentPointsRequired <= knowledge.TalentPoints)
                    {
                        leafAvailableTalents.Add(leafTal);
                    }
                }

                if (leafAvailableTalents.Count > 0)
                {
                    talentButton.interactable = true;

                    var unlockNext = talentUnlocked.FirstOrDefault(u => talents.Where(l => l.Branch == i && l.Leaf == j - 1 && l.Unlocks.Any(ul => ul == JsonObjects.Skills.TalentUnlock.Next)).Select(t => t.Id).Contains(u.TalentId));
                    var unlockLeft = talentUnlocked.FirstOrDefault(u => talents.Where(l => l.Branch == i + 1 && l.Leaf == j - 1 && l.Unlocks.Any(ul => ul == JsonObjects.Skills.TalentUnlock.Left)).Select(t => t.Id).Contains(u.TalentId));
                    var unlockRight = talentUnlocked.FirstOrDefault(u => talents.Where(l => l.Branch == i - 1 && l.Leaf == j - 1 && l.Unlocks.Any(ul => ul == JsonObjects.Skills.TalentUnlock.Right)).Select(t => t.Id).Contains(u.TalentId));

                    if (j == 0 || unlockNext != null || unlockLeft != null || unlockRight != null)
                    {
                        talentButton.GetComponentInChildren<Text>().color = canLearnColor;

                        SetTalentBox(talentButton,
                            i, j,
                            null, leafTalents);

                        talentButton.onClick.AddListener(() =>
                        {
                            TalentChoices.SetDatas(talentButton.GetComponent<TalentBox>());
                        });
                    }
                    else
                    {
                        talentButton.GetComponentInChildren<Text>().color = availableColor;
                    }
                }
                else if (leafTalents.Count > 0)
                {
                    talentButton.GetComponentInChildren<Text>().color = availableColor;
                }
            }
        }
    }

    private void SetTalentBox(Button talentButton,
        int branch, int leaf,
        JsonObjects.TalentUnlocked unlocked, List<JsonObjects.Skills.Talent> talents)
    {
        var talentBox = talentButton.GetComponent<TalentBox>();
        talentBox.Tree = branch;
        talentBox.Leaf = leaf;
        talentBox.Unlocked = unlocked;
        talentBox.Talents = talents;
    }
}
