using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum NpcType
    {
        Talk,
        Sell,
        Buy,
        Fight
    }

    public enum NpcActionType
    {
        Next,
        Close
    }

    [Serializable]
    public struct NpcActionChoice
    {
        public int Order;
        public NpcActionType Type;
        public string ActionText;
        public int GoToIndex;
    }

    [Serializable]
    public struct NpcAction
    {
        public int Index;
        public string ChatText;
        public NpcActionChoice[] Choices;
    }

    public string Id;
    public NpcType[] Types;

    [Header("Talk")]
    public NpcAction[] Actions;

    [Header("Sell")]
    public string[] SellingItemIds;

    private int _currentAction = 0;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "MyPlayer")
        {
            if (Types.Contains(NpcType.Talk))
            {
                UIHandler.This.ShowMenuAction(MenuBox.ActionType.Talk, _setAction);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "MyPlayer")
        {
            UIHandler.This.RemoveMenuAction(MenuBox.ActionType.Talk);
        }
    }

    private void _setAction()
    {
        var action = Actions.FirstOrDefault(a => a.Index == _currentAction);
        action.Choices = action.Choices.OrderBy(c => c.Order).ToArray();

        UIHandler.This.ShowChat(Actions[_currentAction].ChatText);

        foreach (var choice in action.Choices)
        {
            switch (choice.Type)
            {
                case NpcActionType.Next:
                    {
                        UIHandler.This.ChatboxAddChoice(choice.ActionText != "" ? choice.ActionText : "Next", () =>
                        {
                            _nextAction(choice.GoToIndex);
                        });
                    }
                    break;
                case NpcActionType.Close:
                    {
                        UIHandler.This.ChatboxAddChoice(choice.ActionText != "" ? choice.ActionText : "Close", _closeAction);
                    }
                    break;
            }
        }
    }

    private void _nextAction(int index)
    {
        UIHandler.This.ChatboxClearChoices();

        _currentAction = index;

        _setAction();
    }

    private void _closeAction()
    {
        _currentAction = 0;
        UIHandler.This.CloseChat();
        UIHandler.This.ChatboxClearChoices();
    }
}
