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
        Pills,
        Market,
        Equipment,
        Knowledges,
        Bookstore,
        Inventory,
        Adventures
    }

    private Views _currentView = Views.None;
    private GameObject _currentPage;

    public GameObject LoadingScreen;
    private GameObject _loadingScreen;

    [Header("Content Objects")]
    public GameObject AccountForm;
    public GameObject ServerList;
    public GameObject SoulList;
    public GameObject ContentLoadingScreen;
    public GameObject Home;
    public GameObject Market;
    public GameObject Pills;
    public GameObject Equipment;
    public GameObject Adventures;
    public GameObject Inventory;
    public GameObject Knowledges;
    public GameObject Bookstore;


    void Awake()
    {
        This = this;
    }

    public void ShowAccountForm()
    {
        ChangeView(Views.Account);

        HideLoading();
    }

    public void ShowLogin(Guid safeKey)
    {
        AccountForm.GetComponent<AccountForm>().AccountCreated(safeKey);
    }

    public void ShowServerList(List<JsonObjects.GameServerInfos> servers)
    {
        ChangeView(Views.ServerList);

        var script = _currentPage.GetComponent<ServerList>();
        script.SetServers(servers);

        HideLoading();
    }

    public void ShowSoulList(List<JsonObjects.SoulInfos> souls)
    {
        ChangeView(Views.SoulList);

        var script = _currentPage.GetComponent<SoulList>();
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
        ChangeView(Views.Market, true);

        HideLoading();
    }

    public void ShowPills()
    {
        ChangeView(Views.Pills, true);

        var script = _currentPage.GetComponent<Pills>();
        script.UpdateCurrencies();

        HideLoading();
    }

    public void ShowEquipment()
    {
        ChangeView(Views.Equipment, true);
        
        var script = _currentPage.GetComponent<Equipment>();
        script.UpdateEquipment();

        HideLoading();
    }

    public void ShowKnowledges()
    {
        ChangeView(Views.Knowledges, true);

        var script = _currentPage.GetComponent<Knowledges>();

        HideLoading();
    }

    public void ShowBookstore()
    {
        ChangeView(Views.Bookstore, true);

        var script = _currentPage.GetComponent<Bookstore>();

        HideLoading();
    }

    public void ShowInventory()
    {
        ChangeView(Views.Inventory, true);

        var script = _currentPage.GetComponent<Inventory>();

        HideLoading();
    }

    public void ShowAdventures()
    {
        ChangeView(Views.Adventures, true);
        
        var script = _currentPage.GetComponent<Adventures>();

        HideLoading();
    }

    private void ChangeView(Views viewType, bool push = false)
    {
        if (_currentView != viewType)
        {
            if (_currentPage != null && !push) _currentPage.SetActive(false);

            switch (viewType)
            {
                case Views.Account:
                    _currentPage = AccountForm;
                    break;
                case Views.ServerList:
                    _currentPage = ServerList;
                    break;
                case Views.SoulList:
                    _currentPage = SoulList;
                    break;
                case Views.ContentLoadingScreen:
                    _currentPage = ContentLoadingScreen;
                    break;
                case Views.Home:
                    _currentPage = Home;
                    break;
                case Views.Pills:
                    _currentPage = Pills;
                    break;
                case Views.Equipment:
                    _currentPage = Equipment;
                    break;
                case Views.Knowledges:
                    _currentPage = Knowledges;
                    break;
                case Views.Bookstore:
                    _currentPage = Bookstore;
                    break;
                case Views.Inventory:
                    _currentPage = Inventory;
                    break;
                case Views.Adventures:
                    _currentPage = Adventures;
                    break;
                case Views.Market:
                    _currentPage = Market;
                    break;
            }

            _currentPage.SetActive(true);
            _currentPage.transform.SetAsLastSibling();

            if (!push) _currentView = viewType;
        }
    }

    #region Network Callbacks

    public void ContentLoaded(string name)
    {
        var script = ContentLoadingScreen.GetComponent<ContentLoadingScreen>();
        script.IsLoaded(name);

        if (script.IsFinishedLoading)
        {
            ShowHome();
        }
    }

    public void UpdatePlayerDatas()
    {
        var script = Home.GetComponent<Home>();
        script.UpdatePlayerDatas();
    }

    public void UpdateCurrencies()
    {
        var script = Home.GetComponent<Home>();
        script.UpdateCurrencies();
    }

    public void UpdateEquipment()
    {
        var script = Equipment.GetComponent<Equipment>();
        script.UpdateEquipment();
    }

    public void UpdateInventory()
    {
        var script = Inventory.GetComponent<Inventory>();
        script.UpdateItems();
    }

    public void UpdateKnowledges()
    {
        var script = Knowledges.GetComponent<Knowledges>();
        script.LoadKnowledges();
    }

    public void UpdateMarket()
    {
        var script = Market.GetComponent<Market>();
        script.UpdateItems();
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
