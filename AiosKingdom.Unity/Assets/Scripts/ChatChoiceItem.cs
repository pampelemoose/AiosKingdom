using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatChoiceItem : MonoBehaviour
{
    public Text Content;
    public Action Callback;

    private void Start()
    {
        if (Callback != null)
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                Callback();
            });
        }
    }
}
