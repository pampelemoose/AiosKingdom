using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountForm : MonoBehaviour
{
    public NetworkManager Network;

    public GameObject Token;
    public InputField TokenInput;
    public Button RetrieveAccountButton;
    public Button CreateAccountButton;

    public GameObject Safekey;
    public InputField SafekeyInput;
    public Button LoginButton;

    // Use this for initialization
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
            Network.AskNewAccount();
        });

        CreateAccountButton.onClick.AddListener(() =>
        {
            Network.AskNewAccount();
        });

        LoginButton.onClick.AddListener(() =>
        {
            Network.AskAuthentication(PlayerPrefs.GetString("AiosKingdom_IdentifyingKey"));
        });
    }

    // Update is called once per frame
    void Update()
    {

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
