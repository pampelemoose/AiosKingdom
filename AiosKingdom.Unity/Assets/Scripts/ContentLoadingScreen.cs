using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentLoadingScreen : MonoBehaviour
{
    public GameObject Items;
    public GameObject Books;
    public GameObject Monsters;
    public GameObject Adventures;

    private bool _itemsLoaded = false;
    private bool _booksLoaded = false;
    private bool _monstersLoaded = false;
    private bool _adventuresLoaded = false;

    public void IsLoaded(string name)
    {
        Color color = new Color(152, 4, 184, 255);

        switch (name)
        {
            case "items":
                Items.SetActive(true);
                _itemsLoaded = true;
                break;
            case "books":
                Books.SetActive(true);
                _booksLoaded = true;
                break;
            case "monsters":
                Monsters.SetActive(true);
                _monstersLoaded = true;
                break;
            case "adventures":
                Adventures.SetActive(true);
                _adventuresLoaded = true;
                break;
        }
    }

    public bool IsFinishedLoading
    {
        get
        {
            return _itemsLoaded && _booksLoaded && _monstersLoaded && _adventuresLoaded;
        }
    }
}
