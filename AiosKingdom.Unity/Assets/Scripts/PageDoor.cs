using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageDoor : MonoBehaviour
{
    public GameObject Page;

    void Awake()
    {
        var button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Page.SetActive(true);
            //Page.transform.SetAsLastSibling();

            UIManager.This.HideMenu();
        });
    }
}
