using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public GameObject RetrieveTokenBox;
    public InputField TokenInput;
    public Button RetrieveAccountButton;
    public Button CreateAccountButton;

    public GameObject SafekeyBox;
    public Text SafekeyText;
    public Button LoginButton;

    void Start()
    {
        TokenInput.onValueChanged.AddListener((value) =>
        {
            RetrieveAccountButton.interactable = false;

            if (value.Length == 36)
            {
                RetrieveAccountButton.interactable = true;
            }
        });

        RetrieveAccountButton.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();

            NetworkManager.This.AskOldAccount(TokenInput.text);
        });

        CreateAccountButton.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();

            NetworkManager.This.AskNewAccount();
        });

        LoginButton.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();

            NetworkManager.This.AskAuthentication(PlayerPrefs.GetString("AiosKingdom_IdentifyingKey"));
        });
    }

    public void AccountCreated(Guid safeKey)
    {
        UIManager.This.HideLoading();

        RetrieveTokenBox.SetActive(false);
        CreateAccountButton.gameObject.SetActive(false);

        SafekeyBox.SetActive(true);
        SafekeyText.text = safeKey.ToString();
        LoginButton.gameObject.SetActive(true);
    }
}
