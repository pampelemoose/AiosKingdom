using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button Commands;
    public GameObject MenuBox;

    private void Awake()
    {
        Commands.onClick.RemoveAllListeners();
        Commands.onClick.AddListener(() =>
        {
            MenuBox.SetActive(!MenuBox.activeSelf);
        });
    }

    public void Hide()
    {
        MenuBox.SetActive(false);
    }
}
