using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateSoulController : MonoBehaviour
{
    public InputField NewSoulName;
    public Button CreateButton;

    void Start()
    {
        NewSoulName.onValueChanged.AddListener((name) =>
        {
            CreateButton.interactable = false;

            if (name.Length > 4)
                CreateButton.interactable = true;
        });

        CreateButton.onClick.RemoveAllListeners();
        CreateButton.onClick.AddListener(() =>
        {
            NetworkManager.This.CreateSoul(NewSoulName.text);
            CreateButton.interactable = false;
        });
    }
}
