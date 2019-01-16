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
        Talents,
        TalentChoices,
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
    public GameObject Menu;

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
    public GameObject Talents;
    public GameObject TalentChoices;
    public GameObject Bookstore;

    public GameObject Adventure;

    void Awake()
    {
        This = this;
    }

    public void HideMenu()
    {
        Menu.GetComponent<Menu>().Hide();
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

        Menu.SetActive(true);
        Menu.transform.SetAsLastSibling();

        HideLoading();
    }

    public void StartAdventure()
    {
        ChangeView(Views.Adventure, true);

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
