using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAction : MonoBehaviour
{
    public Action Callback;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (Callback != null)
            {
                Callback();
            }
        });
    }

    public void SetText(string content)
    {
        var text = GetComponentInChildren<Text>();

        if (text != null)
        {
            text.text = $"<{content}>";
        }
    }
}
