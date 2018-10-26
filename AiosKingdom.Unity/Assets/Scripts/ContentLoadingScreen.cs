using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentLoadingScreen : MonoBehaviour
{
    public Image Armors;
    public Image Consumables;
    public Image Bags;
    public Image Weapons;
    public Image Books;
    public Image Monsters;
    public Image Adventures;

    private bool _armorsLoaded = false;
    private bool _consumablesLoaded = false;
    private bool _bagsLoaded = false;
    private bool _weaponsLoaded = false;
    private bool _booksLoaded = false;
    private bool _monstersLoaded = false;
    private bool _adventuresLoaded = false;

    public void IsLoaded(string name)
    {
        Color color = new Color(152, 4, 184, 255);

        switch (name)
        {
            case "armors":
                Armors.color = color;
                _armorsLoaded = true;
                break;
            case "consumables":
                Consumables.color = color;
                _consumablesLoaded = true;
                break;
            case "bags":
                Bags.color = color;
                _bagsLoaded = true;
                break;
            case "weapons":
                Weapons.color = color;
                _weaponsLoaded = true;
                break;
            case "books":
                Books.color = color;
                _booksLoaded = true;
                break;
            case "monsters":
                Monsters.color = color;
                _monstersLoaded = true;
                break;
            case "adventures":
                Adventures.color = color;
                _adventuresLoaded = true;
                break;
        }
    }

    public bool IsFinishedLoading
    {
        get
        {
            return _armorsLoaded && _consumablesLoaded && _bagsLoaded && _weaponsLoaded
                && _booksLoaded && _monstersLoaded && _adventuresLoaded;
        }
    }
}
