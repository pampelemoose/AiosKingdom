using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MenuBox;

public class UIHandler : MonoBehaviour
{
    [Serializable]
    public struct UIEntry
    {
        public UIEntity Entity;
        public GameObject Prefab;
    }

    public static UIHandler This;
    private static bool _created = false;

    public enum UIEntity
    {
        PlayerCurrentMap,
        Character,
        Adventure,
        Combat,

        MenuLeft,
        MenuRight,
        Chatbox
    }

    public GameObject World;
    public UIEntry[] UIPrefabs;

    private Dictionary<UIEntity, GameObject> _instances;
    private bool _canMove = true;

    void Awake()
    {
        if (!_created)
        {
            _instances = new Dictionary<UIEntity, GameObject>();

            This = this;

            World.SetActive(true);
        }
    }

    #region Player Current Map
    public void SetPlayerCurrentMap(string map)
    {
        var currentMap = _getInstance(UIEntity.PlayerCurrentMap);

        if (currentMap != null)
        {
            var currentMapText = currentMap.GetComponent<Text>();
            if (currentMapText != null)
            {
                currentMapText.text = map;
            }
        }
    }
    #endregion

    #region Character
    public void UpdateCharacterStats(int Stamina)
    {
        var character = _getInstance(UIEntity.Character);

        if (character != null)
        {
            var characterScript = character.GetComponent<CharacterUI>();
            characterScript.SetStamina(Stamina);
        }
    }
    #endregion

    #region Adventure
    public void UpdateAdventureDay(int day)
    {
        var adventure = _getInstance(UIEntity.Adventure);

        if (adventure != null)
        {
            var adventureScript = adventure.GetComponent<AdventureUI>();
            adventureScript.SetDay(day);
        }
    }
    #endregion

    #region Combat
    public void StartCombat(int maxHealth, int health, int maxMana, int mana, List<Guid> enemies)
    {
        var currentMap = _getInstance(UIEntity.PlayerCurrentMap);
        var character = _getInstance(UIEntity.Character);
        var combat = _getInstance(UIEntity.Combat);

        if (combat != null && character != null)
        {
            World.SetActive(false);
            currentMap.SetActive(false);
            character.SetActive(false);

            combat.SetActive(true);

            var combatScript = combat.GetComponent<CombatUI>();
            combatScript.SetPlayerData(maxHealth, health, maxMana, mana);
            combatScript.SetEnnemyData("SomeMonster");
        }
    }

    public void EndCombat()
    {
        var currentMap = _getInstance(UIEntity.PlayerCurrentMap);
        var character = _getInstance(UIEntity.Character);
        var combat = _getInstance(UIEntity.Combat);

        if (combat != null && character != null && currentMap != null)
        {
            World.SetActive(true);
            currentMap.SetActive(true);
            character.SetActive(true);

            combat.SetActive(false);

            //var combatScript = combat.GetComponent<AdventureUI>();
            //combatScript.SetDay(day);
        }
    }
    #endregion

    #region Menu
    public void ShowMenuAction(ActionType action, Action callback)
    {
        var isRight = _isMenuRight();
        var menu = _getInstance(isRight ? UIEntity.MenuRight : UIEntity.MenuLeft);

        if (menu != null)
        {
            var menuScript = menu.GetComponent<MenuBox>();
            menuScript.AddAction(action, callback);
        }
    }

    public void RemoveMenuAction(ActionType action)
    {
        var isRight = _isMenuRight();
        var menu = _getInstance(isRight ? UIEntity.MenuRight : UIEntity.MenuLeft);

        if (menu != null)
        {
            var menuScript = menu.GetComponent<MenuBox>();
            menuScript.RemoveAction(action);
        }
    }

    private bool _isMenuRight()
    {
        if (!PlayerPrefs.HasKey("AiosKingdom_MenuSide"))
        {
            PlayerPrefs.SetString("AiosKingdom_MenuSide", "left");
            PlayerPrefs.Save();
        }

        var side = PlayerPrefs.GetString("AiosKingdom_MenuSide");

        return side == "right";
    }
    #endregion

    #region Chatbox
    public void ShowChat(string chat)
    {
        _canMove = false;

        var chatbox = _getInstance(UIEntity.Chatbox);

        if (chatbox != null)
        {
            var chatboxScript = chatbox.GetComponent<Chatbox>();
            chatboxScript.SetChat(chat);
        }
    }

    public void CloseChat()
    {
        var chatbox = _getInstance(UIEntity.Chatbox);

        if (chatbox != null)
        {
            var chatboxScript = chatbox.GetComponent<Chatbox>();
            chatboxScript.CloseChat();
            _canMove = true;
        }
    }

    public void ChatboxAddChoice(string text, Action callback)
    {
        var chatbox = _getInstance(UIEntity.Chatbox);

        if (chatbox != null)
        {
            var chatboxScript = chatbox.GetComponent<Chatbox>();
            chatboxScript.AddChoice(text, callback);
        }
    }

    public void ChatboxClearChoices()
    {
        var chatbox = _getInstance(UIEntity.Chatbox);

        if (chatbox != null)
        {
            var chatboxScript = chatbox.GetComponent<Chatbox>();
            chatboxScript.ClearChoices();
        }
    }
    #endregion

    public bool CanMove => _canMove;

    private GameObject _getInstance(UIEntity entity)
    {
        if (!_instances.ContainsKey(entity))
        {
            var found = false;
            foreach (var prefab in UIPrefabs)
            {
                if (prefab.Entity == entity)
                {
                    found = true;
                    _instances.Add(entity, Instantiate(prefab.Prefab, gameObject.transform));
                    break;
                }
            }

            if (!found)
            {
                Debug.LogError($"Could not find ui instance for {UIEntity.Chatbox}");
                return null;
            }
        }

        return _instances[entity];
    }
}
