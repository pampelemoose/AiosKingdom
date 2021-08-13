using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static MenuBox;

public class AdventureUIManager : MonoBehaviour
{
    [Header("Content Object")]
    public GameObject PlayerCurrentMap;
    public GameObject Character;
    public GameObject TavernActionPopup;
    public GameObject BookstoreActionPopup;

    public GameObject QuestObjectivePopup;
    public GameObject QuestFinishedPopup;
    public GameObject AdventureFinishedPopup;

    public static AdventureUIManager This;
    private static bool _created = false;

    [Header("World")]
    public GameObject WorldCamera;
    public GameObject World;

    private bool _canMove = true;

    void Awake()
    {
        if (!_created)
        {
            This = this;

            gameObject.SetActive(false);
        }
    }

    public void StartAdventure(Guid adventureId)
    {
        gameObject.SetActive(true);

        World.SetActive(true);
        WorldCamera.SetActive(true);

        var adventure = DatasManager.Instance.Adventures.FirstOrDefault(a => a.Id == adventureId);

        if (adventure != null)
        {
            WorldManager.This.LoadMap(adventure);
            WorldManager.This.LoadCharacter();
        }

        PlayerCurrentMap.SetActive(true);
        Character.SetActive(true);
    }

    public void FinishAdventure()
    {
        World.SetActive(false);
        WorldCamera.SetActive(false);

        PlayerCurrentMap.SetActive(false);
        Character.SetActive(false);
        TavernActionPopup.SetActive(false);
        BookstoreActionPopup.SetActive(false);
        QuestObjectivePopup.SetActive(false);
        QuestFinishedPopup.SetActive(false);
        AdventureFinishedPopup.SetActive(false);
    }

    #region Player Current Map
    public void SetPlayerCurrentMap(string map)
    {
        var currentMapText = PlayerCurrentMap.GetComponent<Text>();
        if (currentMapText != null)
        {
            currentMapText.text = map;
        }
    }
    #endregion

    #region Character
    public void UpdateCharacterStats(int Stamina)
    {
        var characterScript = Character.GetComponent<CharacterUI>();
        characterScript.SetStamina(Stamina);
    }
    #endregion

    #region Adventure
    //public void UpdateAdventureDay(int day)
    //{
    //    var adventure = _getInstance(UIEntity.Adventure);

    //    if (adventure != null)
    //    {
    //        var adventureScript = adventure.GetComponent<AdventureUI>();
    //        adventureScript.SetDay(day);
    //    }
    //}

    public void EnterTavern(JsonObjects.Adventures.Tavern tavern)
    {
        var tavernScript = TavernActionPopup.GetComponent<TavernActionPopup>();
        tavernScript.Open(tavern);
    }

    public void EnterBookstore(JsonObjects.Adventures.Bookstore bookstore)
    {
        var bookstoreScript = BookstoreActionPopup.GetComponent<BookstoreActionPopup>();
        bookstoreScript.Open(bookstore);
    }

    public void UpdateAdventure(JsonObjects.AdventureState adventureState)
    {
        UpdateSkills(adventureState);
        UpdateQuests(adventureState);
    }

    public void QuestFinished(Guid questId)
    {
        var currentAdventure = DatasManager.Instance.Adventure;
        var adventure = DatasManager.Instance.Adventures.FirstOrDefault(a => a.Id == currentAdventure.AdventureId);
        var quest = adventure.Quests.FirstOrDefault(q => q.Id == questId);

        var questFinishedScript = QuestFinishedPopup.GetComponent<QuestFinishedPopup>();
        questFinishedScript.Open(quest);
    }

    #endregion

    #region Combat
    //public void StartCombat(int maxHealth, int health, int maxMana, int mana, List<Guid> enemies)
    //{
    //    var currentMap = _getInstance(UIEntity.PlayerCurrentMap);
    //    var character = _getInstance(UIEntity.Character);
    //    var combat = _getInstance(UIEntity.Combat);

    //    if (combat != null && character != null)
    //    {
    //        World.SetActive(false);
    //        currentMap.SetActive(false);
    //        character.SetActive(false);

    //        combat.SetActive(true);

    //        var combatScript = combat.GetComponent<CombatUI>();
    //        combatScript.SetPlayerData(maxHealth, health, maxMana, mana);
    //        combatScript.SetEnnemyData("SomeMonster");
    //    }
    //}

    //public void EndCombat()
    //{
    //    var currentMap = _getInstance(UIEntity.PlayerCurrentMap);
    //    var character = _getInstance(UIEntity.Character);
    //    var combat = _getInstance(UIEntity.Combat);

    //    if (combat != null && character != null && currentMap != null)
    //    {
    //        World.SetActive(true);
    //        currentMap.SetActive(true);
    //        character.SetActive(true);

    //        combat.SetActive(false);

    //        //var combatScript = combat.GetComponent<AdventureUI>();
    //        //combatScript.SetDay(day);
    //    }
    //}
    #endregion

    #region Menu
    //public void ShowMenuAction(ActionType action, Action callback)
    //{
    //    var isRight = _isMenuRight();
    //    var menu = _getInstance(isRight ? UIEntity.MenuRight : UIEntity.MenuLeft);

    //    if (menu != null)
    //    {
    //        var menuScript = menu.GetComponent<MenuBox>();
    //        menuScript.AddAction(action, callback);
    //    }
    //}

    //public void RemoveMenuAction(ActionType action)
    //{
    //    var isRight = _isMenuRight();
    //    var menu = _getInstance(isRight ? UIEntity.MenuRight : UIEntity.MenuLeft);

    //    if (menu != null)
    //    {
    //        var menuScript = menu.GetComponent<MenuBox>();
    //        menuScript.RemoveAction(action);
    //    }
    //}

    //private bool _isMenuRight()
    //{
    //    if (!PlayerPrefs.HasKey("AiosKingdom_MenuSide"))
    //    {
    //        PlayerPrefs.SetString("AiosKingdom_MenuSide", "left");
    //        PlayerPrefs.Save();
    //    }

    //    var side = PlayerPrefs.GetString("AiosKingdom_MenuSide");

    //    return side == "right";
    //}
    #endregion

    #region Chatbox
    //public void ShowChat(string chat)
    //{
    //    _canMove = false;

    //    var chatbox = _getInstance(UIEntity.Chatbox);

    //    if (chatbox != null)
    //    {
    //        var chatboxScript = chatbox.GetComponent<Chatbox>();
    //        chatboxScript.SetChat(chat);
    //    }
    //}

    //public void CloseChat()
    //{
    //    var chatbox = _getInstance(UIEntity.Chatbox);

    //    if (chatbox != null)
    //    {
    //        var chatboxScript = chatbox.GetComponent<Chatbox>();
    //        chatboxScript.CloseChat();
    //        _canMove = true;
    //    }
    //}

    //public void ChatboxAddChoice(string text, Action callback)
    //{
    //    var chatbox = _getInstance(UIEntity.Chatbox);

    //    if (chatbox != null)
    //    {
    //        var chatboxScript = chatbox.GetComponent<Chatbox>();
    //        chatboxScript.AddChoice(text, callback);
    //    }
    //}

    //public void ChatboxClearChoices()
    //{
    //    var chatbox = _getInstance(UIEntity.Chatbox);

    //    if (chatbox != null)
    //    {
    //        var chatboxScript = chatbox.GetComponent<Chatbox>();
    //        chatboxScript.ClearChoices();
    //    }
    //}
    #endregion

    public bool CanMove => _canMove;

    //private GameObject _getInstance(UIEntity entity)
    //{
    //    if (!_instances.ContainsKey(entity))
    //    {
    //        var found = false;
    //        foreach (var prefab in UIPrefabs)
    //        {
    //            if (prefab.Entity == entity)
    //            {
    //                found = true;
    //                _instances.Add(entity, Instantiate(prefab.Prefab, gameObject.transform));
    //                break;
    //            }
    //        }

    //        if (!found)
    //        {
    //            Debug.LogError($"Could not find ui instance for {UIEntity.Chatbox}");
    //            return null;
    //        }
    //    }

    //    return _instances[entity];
    //}

    private void UpdateSkills(JsonObjects.AdventureState adventureState)
    {
        DatasManager.Instance.Adventure.State.Skills = adventureState.State.Skills;
        DatasManager.Instance.Adventure.State.Experience = adventureState.State.Experience;
    }

    private void UpdateQuests(JsonObjects.AdventureState adventureState)
    {
        var quests = adventureState.Quests;
        var liveQuests = DatasManager.Instance.Adventure.Quests;
        var newQuests = new List<JsonObjects.AdventureState.QuestState>();

        foreach (var quest in quests)
        {
            if (quest.Finished)
            {
                newQuests.Add(quest);
                continue;
            }

            var liveQuest = liveQuests.FirstOrDefault(q => q.QuestId == quest.QuestId);

            bool allFinished = true;
            foreach (var objective in quest.Objectives)
            {
                if (!objective.Finished)
                {
                    allFinished = false;
                    continue;
                }

                var liveObjective = liveQuest.Objectives.FirstOrDefault(o => o.ObjectiveId == objective.ObjectiveId);

                if (!liveObjective.Finished)
                {
                    QuestObjectiveFinished(quest.QuestId, objective.ObjectiveId);
                }
            }

            if (allFinished)
            {
                var que = quests.FirstOrDefault(q => q.QuestId == quest.QuestId);
                que.Finished = true;

                newQuests.Add(que);

                NetworkManager.This.FinishQuest(quest.QuestId);
            }
            else
            {
                newQuests.Add(quest);
            }
        }

        DatasManager.Instance.Adventure.Quests = newQuests;

        {
            bool allFinished = true;
            foreach (var quest in newQuests)
            {
                if (!quest.Finished)
                {
                    allFinished = false;
                }
            }

            if (allFinished)
            {
                QuestObjectiveFinished();
            }
        }
    }

    private void QuestObjectiveFinished(Guid questId, Guid objectiveId)
    {
        var adventureState = DatasManager.Instance.Adventure;
        var adventure = DatasManager.Instance.Adventures.FirstOrDefault(a => a.Id == adventureState.AdventureId);
        var quest = adventure.Quests.FirstOrDefault(q => q.Id == questId);

        var questObjectiveScript = QuestObjectivePopup.GetComponent<QuestObjectivePopup>();
        questObjectiveScript.Open(quest, objectiveId);
    }    
    
    private void QuestObjectiveFinished()
    {
        var adventureState = DatasManager.Instance.Adventure;
        var adventure = DatasManager.Instance.Adventures.FirstOrDefault(a => a.Id == adventureState.AdventureId);

        var questObjectiveScript = AdventureFinishedPopup.GetComponent<AdventureFinishedPopup>();
        questObjectiveScript.Open(adventure);
    }
}
