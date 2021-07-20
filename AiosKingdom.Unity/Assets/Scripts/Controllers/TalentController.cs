using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TalentController : MonoBehaviour
{
    public GameObject TalentTreePrefab;
    public GameObject TalentLeafPrefab;

    public TalentChoices TalentChoices;

    public GameObject PageOne;
    public GameObject PageTwo;
    public Button NextPageButton;
    public Button PreviousPageButton;

    public Button CloseButton;

    void Awake()
    {
        PreviousPageButton.onClick.RemoveAllListeners();
        PreviousPageButton.onClick.AddListener(() =>
        {
            PageOne.SetActive(true);
            PageTwo.SetActive(false);

            NextPageButton.gameObject.SetActive(true);
            PreviousPageButton.gameObject.SetActive(false);
        });

        PreviousPageButton.gameObject.SetActive(false);

        NextPageButton.onClick.RemoveAllListeners();
        NextPageButton.onClick.AddListener(() =>
        {
            PageOne.SetActive(false);
            PageTwo.SetActive(true);

            NextPageButton.gameObject.SetActive(false);
            PreviousPageButton.gameObject.SetActive(true);
        });

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void ShowTalents()
    {
        UIManager.This.HideLoading();
        gameObject.SetActive(true);
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

        foreach (Transform child in PageOne.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in PageTwo.transform)
        {
            Destroy(child.gameObject);
        }

        for (int leaf = 0; leaf < 12; ++leaf)
        {
            var treeOneObject = Instantiate(TalentTreePrefab, PageOne.transform);
            var treeTwoObject = Instantiate(TalentTreePrefab, PageTwo.transform);

            for (int tier = 0; tier < 5; ++tier)
            {
                var treeObject = tier < 3 ? treeOneObject : treeTwoObject;

                for (int tree = 0; tree < 6; ++tree)
                {
                    var leafObject = Instantiate(TalentLeafPrefab, treeObject.transform);
                    var talentButton = leafObject.GetComponentInChildren<Button>();

                    talentButton.interactable = false;
                    leafObject.GetComponent<Image>().color = disabledColor;
                    talentButton.GetComponentInChildren<Text>().color = disabledColor;
                    talentButton.onClick.RemoveAllListeners();

                    var leafTalents = talents.Where(t => t.Branch == leaf && t.Leaf == (tier * 6) + tree).ToList();
                    var leafUnlocked = talentUnlocked.FirstOrDefault(u => leafTalents.Select(t => t.Id).Contains(u.TalentId));

                    if (leafUnlocked != null)
                    {
                        talentButton.interactable = true;
                        leafObject.GetComponent<Image>().color = learnedColor;
                        talentButton.GetComponentInChildren<Text>().color = learnedColor;
                        SetTalentBox(leafObject,
                                leaf, (tier * 6) + tree,
                                leafUnlocked, leafTalents);

                        talentButton.onClick.AddListener(() =>
                        {
                            TalentChoices.SetDatas(leafObject.GetComponent<TalentBox>());
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

                        var unlockNext = talentUnlocked.FirstOrDefault(u => talents.Where(l => l.Branch == leaf && l.Leaf == ((tier * 6) + tree) - 1 && l.Unlocks.Any(ul => ul == JsonObjects.Skills.TalentUnlock.Next)).Select(t => t.Id).Contains(u.TalentId));
                        var unlockLeft = talentUnlocked.FirstOrDefault(u => talents.Where(l => l.Branch == leaf + 1 && l.Leaf == ((tier * 6) + tree) - 1 && l.Unlocks.Any(ul => ul == JsonObjects.Skills.TalentUnlock.Left)).Select(t => t.Id).Contains(u.TalentId));
                        var unlockRight = talentUnlocked.FirstOrDefault(u => talents.Where(l => l.Branch == leaf - 1 && l.Leaf == ((tier * 6) + tree) - 1 && l.Unlocks.Any(ul => ul == JsonObjects.Skills.TalentUnlock.Right)).Select(t => t.Id).Contains(u.TalentId));

                        if ((tier * 6) + tree == 0 || unlockNext != null || unlockLeft != null || unlockRight != null)
                        {
                            leafObject.GetComponent<Image>().color = canLearnColor;
                            talentButton.GetComponentInChildren<Text>().color = canLearnColor;

                            SetTalentBox(leafObject,
                                leaf, (tier * 6) + tree,
                                null, leafTalents);

                            talentButton.onClick.AddListener(() =>
                            {
                                TalentChoices.SetDatas(leafObject.GetComponent<TalentBox>());
                            });
                        }
                        else
                        {
                            leafObject.GetComponent<Image>().color = availableColor;
                            talentButton.GetComponentInChildren<Text>().color = availableColor;
                        }
                    }
                    else if (leafTalents.Count > 0)
                    {
                        leafObject.GetComponent<Image>().color = availableColor;
                        talentButton.GetComponentInChildren<Text>().color = availableColor;
                    }
                }
            }
        }
    }

    private void SetTalentBox(GameObject leafObject,
        int branch, int leaf,
        JsonObjects.TalentUnlocked unlocked, List<JsonObjects.Skills.Talent> talents)
    {
        var talentBox = leafObject.GetComponent<TalentBox>();
        talentBox.Tree = branch;
        talentBox.Leaf = leaf;
        talentBox.Unlocked = unlocked;
        talentBox.Talents = talents;
    }
}
