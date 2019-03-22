using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageDoor : MonoBehaviour
{
    public GameObject Page;
    public string InputId;

    void Awake()
    {
        var button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            InputController.This.SetId(InputId);
            Page.SetActive(true);
            //Page.transform.SetAsLastSibling();

            UIManager.This.HideMenu();
        });
    }
}
