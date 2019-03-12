using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    void Awake()
    {
        Screen.SetResolution(480, 800, false);

        StartCoroutine(Auth());
    }

    private IEnumerator Auth()
    {
        yield return new WaitForSeconds(1);

        if (PlayerPrefs.HasKey("AiosKingdom_IdentifyingKey"))
        {
            UIManager.This.ShowLoading();
            NetworkManager.This.AskAuthentication(PlayerPrefs.GetString("AiosKingdom_IdentifyingKey"));
        }

        Destroy(gameObject);
    }
}
