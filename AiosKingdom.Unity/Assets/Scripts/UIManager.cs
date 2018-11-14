using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour, IEventSystemHandler
{
    public static UIManager This { get; set; }

    public enum Views
    {
        None,
        Account,
        ServerList,
        SoulList,
        ContentLoadingScreen,
        Home,
        Market
    }

    public Views StartingView = Views.None;
    private Views _currentView = Views.None;
    private GameObject _currentObject;

    public GameObject LoadingScreen;
    private GameObject _loadingScreen;

    public GameObject AccountForm;
    public GameObject ServerList;
    public GameObject SoulList;
    public GameObject ContentLoadingScreen;
    public GameObject Home;
    public GameObject Market;

    void Awake()
    {
        This = this;

        if (StartingView != Views.None)
        {
            ChangeView(StartingView);
        }
    }

    public void ShowAccountForm()
    {
        ChangeView(Views.Account);

        HideLoading();
    }

    public void ShowLogin(Guid safeKey)
    {
        _currentObject.GetComponent<AccountForm>().AccountCreated(safeKey);
    }

    public void ShowServerList(List<JsonObjects.GameServerInfos> servers)
    {
        ChangeView(Views.ServerList);

        var script = _currentObject.GetComponent<ServerList>();
        script.SetServers(servers);

        HideLoading();
    }

    public void ShowSoulList(List<JsonObjects.SoulInfos> souls)
    {
        ChangeView(Views.SoulList);

        var script = _currentObject.GetComponent<SoulList>();
        script.SetSouls(souls);

        HideLoading();
    }

    public void ShowContentLoadingScreen()
    {
        ChangeView(Views.ContentLoadingScreen);

        ShowLoading();
    }

    public void ShowHome()
    {
        ChangeView(Views.Home);

        HideLoading();
    }

    public void ShowMarket()
    {
        ChangeView(Views.Market);

        HideLoading();
    }

    private void ChangeView(Views viewType)
    {
        if (_currentView != viewType)
        {
            Destroy(_currentObject);

            switch (viewType)
            {
                case Views.Account:
                    _currentObject = Instantiate(AccountForm, transform);
                    break;
                case Views.ServerList:
                    _currentObject = Instantiate(ServerList, transform);
                    break;
                case Views.SoulList:
                    _currentObject = Instantiate(SoulList, transform);
                    break;
                case Views.ContentLoadingScreen:
                    _currentObject = Instantiate(ContentLoadingScreen, transform);
                    break;
                case Views.Home:
                    _currentObject = Instantiate(Home, transform);
                    break;
                case Views.Market:
                    _currentObject = Instantiate(Market, transform);
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
        {
            SceneManager.LoadScene(1);
        }
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

    public void UpdateMarket()
    {
        if (_currentView == Views.Market)
        {
            var script = _currentObject.GetComponent<Market>();
            script.UpdateItems();
        }
    }

    #endregion

    #region Loading Screen

    public void ShowLoading()
    {
        if (_loadingScreen == null)
        {
            _loadingScreen = Instantiate(LoadingScreen, transform);
        }

        _loadingScreen.transform.SetAsLastSibling();
        _loadingScreen.SetActive(true);
    }

    public void HideLoading()
    {
        if (_loadingScreen != null)
        {
            _loadingScreen.SetActive(false);
        }
    }

    #endregion
}
