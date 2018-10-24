using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AiosKingdom : MonoBehaviour, IEventSystemHandler
{
    private enum Views
    {
        Account,
        ServerList,
        SoulList,
    }

    private static bool _created = false;

    private NetworkManager _network;

    public Canvas Ui;
    private Views _currentView = Views.Account;
    private GameObject _currentObject;

    public GameObject AccountForm;
    public GameObject ServerList;
    public GameObject SoulList;

    void Awake()
    {
        if (!_created)
        {
            DontDestroyOnLoad(this.gameObject);
            _created = true;
            Debug.Log("Awake: " + this.gameObject);
        }
    }

    void Start()
    {
        _network = this.GetComponent<NetworkManager>();

        if (PlayerPrefs.HasKey("AiosKingdom_IdentifyingKey"))
        {
            LoadingScreen.Loading.Show();
            _network.AskAuthentication(PlayerPrefs.GetString("AiosKingdom_IdentifyingKey"));
        }
        else
        {
            ShowAccountForm();
        }
    }

    public void ShowAccountForm()
    {
        ChangeView(Views.Account);

        _currentObject.GetComponent<AccountForm>().Network = _network;
    }

    public void ShowLogin(Guid safeKey)
    {
        _currentObject.GetComponent<AccountForm>().AccountCreated(safeKey);
    }

    public void GetServerList()
    {
        LoadingScreen.Loading.Show();
        _network.AskServerInfos();
    }

    public void ShowServerList(List<JsonObjects.GameServerInfos> servers)
    {
        ChangeView(Views.ServerList);

        var script = _currentObject.GetComponent<ServerList>();
        script.Network = _network;
        script.SetServers(servers);

        LoadingScreen.Loading.Hide();
    }

    public void ShowSoulList(List<JsonObjects.SoulInfos> souls)
    {
        ChangeView(Views.SoulList);

        var script = _currentObject.GetComponent<SoulList>();
        script.Network = _network;
        script.SetSouls(souls);

        LoadingScreen.Loading.Hide();
    }

    private void ChangeView(Views viewType)
    {
        if (_currentView != viewType)
        {
            Destroy(_currentObject);

            switch (viewType)
            {
                case Views.Account:
                    _currentObject = Instantiate(AccountForm, Ui.transform);
                    break;
                case Views.ServerList:
                    _currentObject = Instantiate(ServerList, Ui.transform);
                    break;
                case Views.SoulList:
                    _currentObject = Instantiate(SoulList, Ui.transform);
                    break;
            }
            
            _currentView = viewType;
        }
    }
}
