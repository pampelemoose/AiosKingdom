using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private static LoadingScreen _instance;
    public static LoadingScreen Loading
    {
        get
        {
            if (_instance != null)
                return _instance;
            return null;
        }
    }

    void Start()
    {
        _instance = this;
        Hide();
    }

    void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public void Show()
    {
        _instance.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _instance.gameObject.SetActive(false);
    }
}
