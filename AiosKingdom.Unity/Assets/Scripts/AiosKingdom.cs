using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AiosKingdom : MonoBehaviour, IEventSystemHandler
{
    private enum Views
    {
        None,
        Account,
        ServerList,
        SoulList,
        ContentLoadingScreen,
        Home,
    }

    private static bool _created = false;

    private NetworkManager _network;

    public Canvas Ui;
    private Views _currentView = Views.None;
    private GameObject _currentObject;

    public GameObject AccountForm;
    public GameObject ServerList;
    public GameObject SoulList;
    public GameObject ContentLoadingScreen;
    public GameObject Home;

    void Awake()
    {
        if (!_created)
        {
            DontDestroyOnLoad(this.gameObject);
            _created = true;
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

    void OnApplicationQuit()
    {
        _network.DisconnectGame();
        _network.Disconnect();
    }

    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (!hasFocus)
    //    {
    //        _network.DisconnectGame();
    //        _network.Disconnect();
    //    }
    //}

    public void ShowAccountForm()
    {
        ChangeView(Views.Account);

        _currentObject.GetComponent<AccountForm>().Network = _network;

        LoadingScreen.Loading.Hide();
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

    public void ShowContentLoadingScreen()
    {
        ChangeView(Views.ContentLoadingScreen);

        LoadingScreen.Loading.Show();
    }

    public void ShowHome()
    {
        ChangeView(Views.Home);

        var script = _currentObject.GetComponent<Home>();
        script.Main = this;
        script.Network = _network;

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
                case Views.ContentLoadingScreen:
                    _currentObject = Instantiate(ContentLoadingScreen, Ui.transform);
                    break;
                case Views.Home:
                    _currentObject = Instantiate(Home, Ui.transform);
                    break;
            }

            _currentView = viewType;
        }
    }

    #region Network Callbacks

    public void ContentLoaded(string name)
    {
        var script = _currentObject.GetComponent<ContentLoadingScreen>();
        script.IsLoaded(name);

        if (script.IsFinishedLoading)
            ShowHome();
    }

    public void UpdatePlayerDatas()
    {
        if (_currentView == Views.Home)
        {
            var script = _currentObject.GetComponent<Home>();
            script.UpdatePlayerDatas();
        }
    }

    public void UpdateCurrencies()
    {
        if (_currentView == Views.Home)
        {
            var script = _currentObject.GetComponent<Home>();
            script.UpdateCurrencies();
        }
    }

    public void UpdateEquipment()
    {
        if (_currentView == Views.Home)
        {
            var script = _currentObject.GetComponent<Home>();
            script.UpdateEquipment();
        }
    }

    public void UpdateInventory()
    {
        if (_currentView == Views.Home)
        {
            var script = _currentObject.GetComponent<Home>();
            script.UpdateInventory();
        }
    }

    public void UpdateKnowledges()
    {
        if (_currentView == Views.Home)
        {
            var script = _currentObject.GetComponent<Home>();
            script.UpdateKnowledges();
        }
    }

    #endregion
}
