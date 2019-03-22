using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Page : MonoBehaviour
{
    public Button Close;
    public string InputId;

    void Start()
    {
        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            CloseAction();
        });
    }

    public void CloseAction()
    {
        InputController.This.SetId(InputId);
        gameObject.SetActive(false);
    }
}
