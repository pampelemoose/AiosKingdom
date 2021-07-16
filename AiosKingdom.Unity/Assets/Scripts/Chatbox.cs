using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chatbox : MonoBehaviour
{
    public GameObject ChatChoice;
    public GameObject ChatChoiceItemPrefab;

    public Text Content;

    private string _textToShow;

    private List<GameObject> _choices;

    public void SetChat(string content)
    {
        _textToShow = content;

        Content.text = _textToShow;

        gameObject.SetActive(true);
    }

    public void AddChoice(string text, Action callback)
    {
        if (_choices == null)
        {
            _choices = new List<GameObject>();
        }

        var choiceObject = Instantiate(ChatChoiceItemPrefab, ChatChoice.transform);
        var choiceScript = choiceObject.GetComponent<ChatChoiceItem>();
        choiceScript.Content.text = text;
        choiceScript.Callback = callback;

        _choices.Add(choiceObject);
    }

    public void ClearChoices()
    {
        foreach (var choice in _choices)
        {
            Destroy(choice);
        }

        _choices = null;
    }

    public void CloseChat()
    {
        gameObject.SetActive(false);
        Content.text = "";
    }

    //void Update()
    //{
    //    if (Application.platform != RuntimePlatform.Android)
    //    {
    //        _handleKeyboard();
    //    }
    //    else
    //    {
    //        _handleTouch();
    //    }
    //}

    //private void _handleKeyboard()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        if (_currentIndex + 1 == _textToShow.Length)
    //        {
    //            if (_callback != null)
    //            {
    //                _callback();
    //            }

    //            return;
    //        }

    //        ++_currentIndex;
    //        Content.text = _textToShow[_currentIndex];
    //    }
    //}

    //private void _handleTouch()
    //{
    //    if (Input.touchCount == 0)
    //    {
    //        _canNext = true;
    //    }
    //    else if (Input.touchCount == 1 && _canNext)
    //    {
    //        if (_currentIndex + 1 == _textToShow.Length)
    //        {
    //            if (_callback != null)
    //            {
    //                _callback();
    //            }

    //            CloseChat();
    //            return;
    //        }

    //        ++_currentIndex;
    //        Content.text = _textToShow[_currentIndex];

    //        _canNext = false;
    //    }
    //}
}
