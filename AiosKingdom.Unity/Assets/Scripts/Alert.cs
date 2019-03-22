using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    public Text Title;
    public Text Content;
    public Button Close;

    void Start()
    {
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Show(string content, string title = null)
    {
        Title.text = "";
        if (title != null)
        {
            Title.text = string.Format("[{0}]", title);
        }

        Content.text = content;
    }
}
