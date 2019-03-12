using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountForm : MonoBehaviour, ICallbackHooker
{
    public GameObject Token;
    public InputField TokenInput;
    public Button RetrieveAccountButton;
    public Button CreateAccountButton;

    public GameObject Safekey;
    public InputField SafekeyInput;
    public Button LoginButton;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Client_CreateAccount, (message) =>
        {
            if (message.Success)
            {
                var appUser = JsonConvert.DeserializeObject<JsonObjects.NewAccount>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    PlayerPrefs.SetString("AiosKingdom_IdentifyingKey", appUser.Identifier.ToString());
                    PlayerPrefs.Save();
                    _accountCreated(appUser.SafeKey);
                });
            }
            else
            {
                Debug.Log("CreateAccount error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Client_RetrieveAccount, (message) =>
        {
            if (message.Success)
            {
                var appUser = JsonConvert.DeserializeObject<JsonObjects.NewAccount>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    PlayerPrefs.SetString("AiosKingdom_IdentifyingKey", appUser.Identifier.ToString());
                    PlayerPrefs.Save();
                    NetworkManager.This.AskAuthentication(appUser.Identifier.ToString());
                });
                //MessagingCenter.Send(this, MessengerCodes.RetrievedAccount);
            }
            else
            {
                Debug.Log("CreateAccount error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Client_Authenticate, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.AskServerList();
            }
            else
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    PlayerPrefs.DeleteKey("AiosKingdom_IdentifyingKey");
                    PlayerPrefs.Save();
                });

                Debug.Log("Authenticate error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Client_ServerList, (message) =>
        {
            if (message.Success)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    gameObject.SetActive(false);
                });
            }
        });
    }

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

        if (!PlayerPrefs.HasKey("AiosKingdom_IdentifyingKey"))
        {
            gameObject.SetActive(true);
        }
    }

    private void _accountCreated(Guid safeKey)
    {
        UIManager.This.HideLoading();

        Token.SetActive(false);
        CreateAccountButton.gameObject.SetActive(false);

        Safekey.SetActive(true);
        SafekeyInput.text = safeKey.ToString();
        LoginButton.gameObject.SetActive(true);
    }
}
