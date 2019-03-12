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
    }

    private Views _currentView = Views.None;
    private GameObject _currentPage;

    public GameObject LoadingScreen;
    private GameObject _loadingScreen;

    [Header("Content Objects")]
    public GameObject Menu;

    public GameObject Settings;

    public MonoBehaviour[] CallbackHookers;

    void Awake()
    {
        This = this;

        foreach (ICallbackHooker hooker in CallbackHookers)
        {
            hooker.HookCallbacks();
        }
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
