using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentLoadingScreen : MonoBehaviour
{
    public Text Items;
    public Text Books;
    public Text Monsters;
    public Text Adventures;
    public Text Recipes;

    private bool _itemsLoaded = false;
    private bool _booksLoaded = false;
    private bool _monstersLoaded = false;
    private bool _adventuresLoaded = false;
    private bool _recipesLoaded = false;

    public void IsLoaded(string name)
    {
        switch (name)
        {
            case "items":
                Items.text = "[x]";
                _itemsLoaded = true;
                break;
            case "books":
                Books.text = "[x]";
                _booksLoaded = true;
                break;
            case "monsters":
                Monsters.text = "[x]";
                _monstersLoaded = true;
                break;
            case "adventures":
                Adventures.text = "[x]";
                _adventuresLoaded = true;
                break;
            case "recipes":
                Recipes.text = "[x]";
                _recipesLoaded = true;
                break;
        }
    }

    public bool IsFinishedLoading
    {
        get
        {
            return _itemsLoaded && _booksLoaded && _monstersLoaded && _adventuresLoaded && _recipesLoaded;
        }
    }
}
