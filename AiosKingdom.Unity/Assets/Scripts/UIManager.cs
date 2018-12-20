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
        Settings,
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
        AdventureSelection,
        Adventure
    }

    private Views _currentView = Views.None;
    private GameObject _currentPage;

    public GameObject LoadingScreen;
    private GameObject _loadingScreen;

    [Header("Content Objects")]
    public GameObject Settings;
    public GameObject AccountForm;
    public GameObject ServerList;
    public GameObject SoulList;
    public GameObject ContentLoadingScreen;
    public GameObject Home;
    public GameObject Market;
    public GameObject Pills;
    public GameObject Equipment;
    public GameObject AdventureSelection;
    public GameObject Inventory;
    public GameObject Knowledges;
    public GameObject Bookstore;

    public GameObject Adventure;

    void Awake()
    {
        This = this;
    }

    public void ShowSettings()
    {
        ChangeView(Views.Settings);

        HideLoading();
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

        var script = ServerList.GetComponent<ServerList>();
        script.SetServers(servers);

        HideLoading();
    }

    public void ShowSoulList(List<JsonObjects.SoulInfos> souls)
    {
        ChangeView(Views.SoulList);

        var script = SoulList.GetComponent<SoulList>();
        script.SetSouls(souls);

        HideLoading();
    }

    public void ShowContentLoadingScreen()
    {
        ChangeView(Views.ContentLoadingScreen);
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

        var script = Pills.GetComponent<Pills>();
        script.UpdateCurrencies();

        HideLoading();
    }

    public void ShowEquipment()
    {
        ChangeView(Views.Equipment, true);
        
        var script = Equipment.GetComponent<Equipment>();
        script.UpdateEquipment();

        HideLoading();
    }

    public void ShowKnowledges()
    {
        ChangeView(Views.Knowledges, true);

        //var script = _currentPage.GetComponent<Knowledges>();

        HideLoading();
    }

    public void ShowBookstore()
    {
        ChangeView(Views.Bookstore, true);

        //var script = _currentPage.GetComponent<Bookstore>();

        HideLoading();
    }

    public void ShowInventory()
    {
        ChangeView(Views.Inventory, true);

        //var script = _currentPage.GetComponent<Inventory>();

        HideLoading();
    }

    public void ShowAdventures()
    {
        ChangeView(Views.AdventureSelection, true);
        
        //var script = _currentPage.GetComponent<AdventureSelection>();

        HideLoading();
    }

    public void StartAdventure()
    {
        ChangeView(Views.Adventure, true);

        var script = Adventure.GetComponent<Adventure>();
        script.Initialize();

        HideLoading();
    }

    private void ChangeView(Views viewType, bool push = false)
    {
        if (_currentView != viewType)
        {
            if (_currentPage != null && !push) _currentPage.SetActive(false);

            GameObject newPage = null;

            switch (viewType)
            {
                case Views.Settings:
                    newPage = Settings;
                    break;
                case Views.Account:
                    newPage = AccountForm;
                    break;
                case Views.ServerList:
                    newPage = ServerList;
                    break;
                case Views.SoulList:
                    newPage = SoulList;
                    break;
                case Views.ContentLoadingScreen:
                    newPage = ContentLoadingScreen;
                    break;
                case Views.Home:
                    newPage = Home;
                    break;
                case Views.Pills:
                    newPage = Pills;
                    break;
                case Views.Equipment:
                    newPage = Equipment;
                    break;
                case Views.Knowledges:
                    newPage = Knowledges;
                    break;
                case Views.Bookstore:
                    newPage = Bookstore;
                    break;
                case Views.Inventory:
                    newPage = Inventory;
                    break;
                case Views.Market:
                    newPage = Market;
                    break;
                case Views.AdventureSelection:
                    newPage = AdventureSelection;
                    break;
                case Views.Adventure:
                    newPage = Adventure;
                    break;
            }

            if (newPage != null)
            {
                newPage.SetActive(true);
                newPage.transform.SetAsLastSibling();

                if (!push)
                {
                    _currentView = viewType;
                    _currentPage = newPage;
                }
            }
        }
    }

    #region Network Callbacks

    public void ContentLoaded(string name)
    {
        var script = ContentLoadingScreen.GetComponent<ContentLoadingScreen>();
        script.IsLoaded(name);

        if (script.IsFinishedLoading)
        {
            ShowLoading();
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
        var homeScript = Home.GetComponent<Home>();
        homeScript.UpdateCurrencies();

        var pillsScript = Pills.GetComponent<Pills>();
        pillsScript.UpdateCurrencies();
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

    public void UpdateSpecialMarket()
    {
        var script = Market.GetComponent<Market>();
        script.UpdateSpecialItems();
    }

    public void UpdateAdventure()
    {
        var script = Adventure.GetComponent<Adventure>();
        script.UpdateCurrentState();
    }

    public void AdventureLogResults(List<JsonObjects.AdventureState.ActionResult> actionResults)
    {
        var script = Adventure.GetComponent<Adventure>();
        script.LogResults(actionResults);
    }

    public void AdventureTriggerEnemyTurn()
    {
        var script = Adventure.GetComponent<Adventure>();
        script.TriggerEnemyTurn();
    }

    public void AdventureShowLoots(List<JsonObjects.LootItem> loots)
    {
        var script = Adventure.GetComponent<Adventure>();
        script.ShowLoots(loots);
    }

    public void AdventureShowEndResults(List<JsonObjects.AdventureState.ActionResult> actionResults)
    {
        var script = Adventure.GetComponent<Adventure>();
        script.ShowEndResults(actionResults);
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
