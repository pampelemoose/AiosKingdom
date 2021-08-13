using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookstoreActionPopup : MonoBehaviour
{
    public Text BookstoreNameText;
    public Button BuyButton;
    public Button TalentButton;
    public Button CloseButton;

    public GameObject BookstoreListPopup;

    public void Open(JsonObjects.Adventures.Bookstore bookstore)
    {
        var bookIds = bookstore.Books.Select(o => o.BookId).ToList();
        var books = DatasManager.Instance.Books.Where(b => bookIds.Contains(b.Id)).ToList();

        BookstoreNameText.text = bookstore.Name;

        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(() =>
        {
            var bookListScript = BookstoreListPopup.GetComponent<BookstoreListPopup>();
            bookListScript.Open(books);
        });

        TalentButton.onClick.RemoveAllListeners();
        TalentButton.onClick.AddListener(() =>
        {
            
        });

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(() =>
        {
            WorldManager.This.SetCanMove(true);
            gameObject.SetActive(false);
        });

        WorldManager.This.SetCanMove(false);
        gameObject.SetActive(true);
    }
}
