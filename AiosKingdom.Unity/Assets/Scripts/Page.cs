using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Page : MonoBehaviour
{
    public Button Close;

    void Start()
    {
        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
