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

        Token.SetActive(false);
        CreateAccountButton.gameObject.SetActive(false);

        Safekey.SetActive(true);
        SafekeyInput.text = safeKey.ToString();
        LoginButton.gameObject.SetActive(true);
    }
}
