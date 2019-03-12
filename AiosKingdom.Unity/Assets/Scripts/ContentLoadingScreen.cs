using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentLoadingScreen : MonoBehaviour, ICallbackHooker
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

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Server.ConnectSoul, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                NetworkManager.This.AskItemList();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    gameObject.SetActive(true);
                });
            }
            else
            {
                Debug.Log("Connect Soul error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Listing.Item, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.AskBookList();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    Items.text = "[x]";
                    _itemsLoaded = true;
                });
            }
            else
            {
                Debug.Log("Item error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Listing.Book, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.AskMonsterList();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    Books.text = "[x]";
                    _booksLoaded = true;
                });
            }
            else
            {
                Debug.Log("Book error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Listing.Monster, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.AskAdventureList();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    Monsters.text = "[x]";
                    _monstersLoaded = true;
                });
            }
            else
            {
                Debug.Log("Monster error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Listing.Dungeon, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.AskRecipeList();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    Adventures.text = "[x]";
                    _adventuresLoaded = true;
                });
            }
            else
            {
                Debug.Log("Dungeon error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Listing.Recipes, (message) =>
        {
            if (message.Success)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    Recipes.text = "[x]";
                    _recipesLoaded = true;

                    gameObject.SetActive(false);
                });
            }
            else
            {
                Debug.Log("Recipes error : " + message.Json);
            }
        });
    }

    public bool IsFinishedLoading
    {
        get
        {
            return _itemsLoaded && _booksLoaded && _monstersLoaded && _adventuresLoaded && _recipesLoaded;
        }
    }
}
