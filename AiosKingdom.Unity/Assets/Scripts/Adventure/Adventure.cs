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
        Exit.onClick.RemoveAllListeners();
        Exit.onClick.AddListener(() =>
        {
            NetworkManager.This.ExitDungeon();
            gameObject.SetActive(false);
        });
    }

    public void Initialize()
    {
        NetworkManager.This.UpdateDungeonRoom();
    }

    public void UpdateCurrentState()
    {
        Name.text = DatasManager.Instance.Adventure.Name;
        Room.text = string.Format("[{0} / {1}]", DatasManager.Instance.Adventure.CurrentRoom, DatasManager.Instance.Adventure.TotalRoomCount);
    }
}
