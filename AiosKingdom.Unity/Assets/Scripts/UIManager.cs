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
        Unavailable,
        Login,
        ServerSelection,
        SoulSelection,
        CreateSoul,

        Main,
        StatSelection,
        Equipment,
        Inventory,

        Settings,
        Adventure
    }

    private Views _currentView = Views.None;
    private GameObject _currentPage;

    public GameObject LoadingScreenPrefab;
    private GameObject _loadingScreen;

    [Header("Content Objects")]
    public GameObject LandingScreen;
    public GameObject Unavailable;
    public GameObject Login;
    public GameObject ServerSelection;
    public GameObject SoulSelection;
    public GameObject CreateSoul;

    public GameObject Main;
    public GameObject StatSelection;
    public GameObject Equipment;
    public GameObject Inventory;

    public GameObject Menu;

    public GameObject Settings;
    public GameObject Adventure;

    public static Dictionary<JsonObjects.Items.ItemQuality, Color> QualityColor = new Dictionary<JsonObjects.Items.ItemQuality, Color>
    {
        { JsonObjects.Items.ItemQuality.Common, new Color(1, 0, 0) },
        { JsonObjects.Items.ItemQuality.Uncommon, new Color(0.9f, 1, 0) },
        { JsonObjects.Items.ItemQuality.Rare, new Color(1, 0, 0.8f) },
        { JsonObjects.Items.ItemQuality.Epic, new Color(0, 1, 0.3f) },
        { JsonObjects.Items.ItemQuality.Legendary, new Color(0, 0.5f, 1) },
    };

    public static Dictionary<JsonObjects.Stats, Color> StatColors = new Dictionary<JsonObjects.Stats, Color>
    {
        { JsonObjects.Stats.Stamina, new Color(1, 0, 0) },
        { JsonObjects.Stats.Energy, new Color(0.9f, 1, 0) },
        { JsonObjects.Stats.Strength, new Color(1, 0, 0.8f) },
        { JsonObjects.Stats.Agility, new Color(0, 1, 0.3f) },
        { JsonObjects.Stats.Intelligence, new Color(0, 0.5f, 1) },
        { JsonObjects.Stats.Wisdom, new Color(0, 1, 1) }
    };

    void Awake()
    {
        This = this;

        _currentPage = LandingScreen;
    }

    public void ShowUnavailable()
    {
        ChangeView(Views.Unavailable);

        HideLoading();
    }

    public void ShowLogin()
    {
        ChangeView(Views.Login);

        HideLoading();
    }   
    
    public void AccountCreated(Guid safeKey)
    {
        Login.GetComponent<LoginController>().AccountCreated(safeKey);
    }

    public void ShowServerList(List<JsonObjects.GameServerInfos> servers)
    {
        ChangeView(Views.ServerSelection);

        var script = ServerSelection.GetComponent<ServerSelectionController>();
        script.SetServers(servers);

        HideLoading();
    }

    public void ShowSoulList(List<JsonObjects.SoulInfos> souls)
    {
        ChangeView(Views.SoulSelection);

        var script = SoulSelection.GetComponent<SoulSelectionController>();
        script.SetSouls(souls);

        HideLoading();
    }

    public void ShowCreateSoul()
    {
        ChangeView(Views.CreateSoul);
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

    public void ShowMain()
    {
        ChangeView(Views.Main);

        //Menu.SetActive(true);
        //Menu.transform.SetAsLastSibling();

        HideLoading();
    }

    public void ShowStatSelection()
    {
        ChangeView(Views.StatSelection);
    }

    public void ShowEquipment()
    {
        ChangeView(Views.Equipment);
    }

    public void ShowInventory()
    {
        ChangeView(Views.Inventory);
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
                case Views.None:
                    newPage = LandingScreen;
                    break;
                case Views.Unavailable:
                    newPage = Unavailable;
                    break;
                case Views.Login:
                    newPage = Login;
                    break;
                case Views.ServerSelection:
                    newPage = ServerSelection;
                    break;
                case Views.SoulSelection:
                    newPage = SoulSelection;
                    break;
                case Views.CreateSoul:
                    newPage = CreateSoul;
                    break;

                case Views.Main:
                    newPage = Main;
                    break;
                case Views.StatSelection:
                    newPage = StatSelection;
                    break;
                case Views.Equipment:
                    newPage = Equipment;
                    break;
                case Views.Inventory:
                    newPage = Inventory;
                    break;

                case Views.Settings:
                    newPage = Settings;
                    break;
                
                case Views.Adventure:
                    newPage = Adventure;
                    break;    
            }

            if (newPage != null)
            {
                newPage.SetActive(true);
                //newPage.transform.SetAsLastSibling();

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
            _loadingScreen = Instantiate(LoadingScreenPrefab, transform);
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
