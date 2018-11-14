using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountForm : MonoBehaviour
{
    public GameObject Token;
    public InputField TokenInput;
    public Button RetrieveAccountButton;
    public Button CreateAccountButton;

    public GameObject Safekey;
    public InputField SafekeyInput;
    public Button LoginButton;

    void Start()
    {
        TokenInput.onValueChanged.AddListener((value) =>
        {
            RetrieveAccountButton.interactable = false;

            if (value.Length == 36)
                RetrieveAccountButton.interactable = true;
        });

        RetrieveAccountButton.onClick.AddListener(() =>
        {
            NetworkManager.This.AskNewAccount();
        });

        CreateAccountButton.onClick.AddListener(() =>
        {
            NetworkManager.This.AskNewAccount();
        });

        LoginButton.onClick.AddListener(() =>
        {
            NetworkManager.This.AskAuthentication(PlayerPrefs.GetString("AiosKingdom_IdentifyingKey"));
        });
    }

    public void AccountCreated(Guid safeKey)
    {
        Token.SetActive(false);
        CreateAccountButton.gameObject.SetActive(false);

        Safekey.SetActive(true);
        SafekeyInput.text = safeKey.ToString();
        LoginButton.gameObject.SetActive(true);
    }
}
