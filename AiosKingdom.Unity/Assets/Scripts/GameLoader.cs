using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    void Awake()
    {
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
        else
        {
            UIManager.This.ShowAccountForm();
        }

        Destroy(gameObject);
    }
}
