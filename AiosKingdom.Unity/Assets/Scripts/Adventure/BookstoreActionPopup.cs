using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookstoreActionPopup : MonoBehaviour
{
    public Text BookstoreNameText;
    public Button BuyButton;
    public Button TalentButton;
    public Button CloseButton;

    public void Open(JsonObjects.Adventures.Bookstore bookstore)
    {
        BookstoreNameText.text = bookstore.Name;

        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(() =>
        {
            //NetworkManager.This.RestInTavern(tavern.Id);
            //gameObject.SetActive(false);
        });

        TalentButton.onClick.RemoveAllListeners();
        TalentButton.onClick.AddListener(() =>
        {
            
        });

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        gameObject.SetActive(true);
    }
}
