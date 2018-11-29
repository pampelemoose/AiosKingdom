using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Adventure : MonoBehaviour
{
    public Text Name;
    public Text Room;

    public Button Exit;

    void Awake()
    {
        Exit.onClick.AddListener(() =>
        {
            NetworkManager.This.ExitDungeon();
        });
    }

    public void UpdateCurrentState()
    {
        Name.text = DatasManager.Instance.Adventure.
    }
}
