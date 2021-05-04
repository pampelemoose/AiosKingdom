using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager This { get; set; }

    private static bool _created = false;

    [Header("Callbacks")]
    public ContentLoadingScreen ContentLoadingScreen;
    public Home Home;
    public Pills Pills;
    public Inventory Inventory;
    public Knowledges Knowledges;
    public Equipment Equipment;
    public Market Market;
    public Adventure Adventure;
    public Talents Talents;

    private bool _isDispatchRunning;
    private TcpClient _dispatch;
    private Thread _dispatchThread;
    private string _dispatchBuffer;
    private Guid _dispatchAuthToken;

    private JsonObjects.GameServerConnection _gameConnection;
    private bool _isGameRunning;
    private TcpClient _game;
    private Thread _gameThread;
    private string _gameBuffer;
    private Guid _gameAuthToken;

    void Start()
    {
        ConnectToDispatchServer();
    }

    void Awake()
    {
        if (!_created)
        {
            DontDestroyOnLoad(this.gameObject);
            _created = true;
        }

        This = this;
    }

    void OnApplicationQuit()
    {
        DisconnectSoul();
        AnnounceDisconnection();
        DisconnectGame();
        Disconnect();
    }

    public void Reconnect()
    {
        SceneLoom.Loom.QueueOnMainThread(() =>
        {
            UIManager.This.ShowAccountForm();
        });

        ConnectToDispatchServer();
    }

    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (!hasFocus)
    //    {
    //        _network.DisconnectGame();
    //        _network.Disconnect();
    //    }
    //}

    #region Dispatch Functions

    private void ConnectToDispatchServer()
    {
        try
        {
            _dispatchThread = new Thread(new ThreadStart(ListenForData));
            _dispatchThread.IsBackground = true;
            _dispatchThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception : " + e);
        }
    }

    private void ListenForData()
    {
        try
        {
            _dispatch = new TcpClient("127.0.0.1", 1337);
            _isDispatchRunning = true;

            while (_isDispatchRunning)
            {
                var bufferSize = _dispatch.Available;

                if (bufferSize > 0)
                {
                    var client = _dispatch.Client;
                    string bufferStr = _dispatchBuffer;
                    _dispatchBuffer = string.Empty;
                    int received = 0;

                    do
                    {
                        byte[] buffer = new byte[bufferSize > 256 ? 256 : bufferSize];
                        received = client.Receive(buffer);
                        if (received > 0)
                            bufferStr += Encoding.UTF8.GetString(buffer);
                        bufferSize -= received;
                    } while (received == 256);

                    var messages = bufferStr.Trim().Split('|');
                    foreach (var message in messages)
                    {
                        if (string.IsNullOrEmpty(message) || message.All(char.IsWhiteSpace))
                            continue;

                        try
                        {
                            var fromJson = JsonConvert.DeserializeObject<JsonObjects.Message>(message);

                            if (!ProcessDispatchMessage(fromJson))
                            {
                                /*_message = buffer;
                                NotifyPropertyChanged(nameof(Message));*/
                            }
                        }
                        catch
                        {
                            _dispatchBuffer += message;
                        }
                    }
                }
            }

            _dispatch.GetStream().Flush();
            _dispatch.Client.Close();
            _dispatch.Close();
        }
        catch (SocketException sockE)
        {
            Debug.Log("Socket exception : " + sockE);

            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowUnavailable();
            });
        }
    }

    private bool ProcessDispatchMessage(JsonObjects.Message message)
    {
        switch (message.Code)
        {
            //case Network.CommandCodes.Ping:
            //    {
            //        var args = new string[0];
            //        var retMess = new Network.Message
            //        {
            //            Code = Network.CommandCodes.Ping,
            //            Json = JsonConvert.SerializeObject(args)
            //        };
            //        SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
            //    }
            //    break;
            case JsonObjects.CommandCodes.GlobalMessage:
                {
                    //ScreenManager.Instance.AlertScreen("Server Message", message.Json);
                    Debug.Log("Server Message : " + message.Json);
                }
                break;

            case JsonObjects.CommandCodes.Client_CreateAccount:
                {
                    if (message.Success)
                    {
                        var appUser = JsonConvert.DeserializeObject<JsonObjects.NewAccount>(message.Json);

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            PlayerPrefs.SetString("AiosKingdom_IdentifyingKey", appUser.Identifier.ToString());
                            PlayerPrefs.Save();
                            UIManager.This.ShowLogin(appUser.SafeKey);
                        });

                        //MessagingCenter.Send(this, MessengerCodes.CreateNewAccount, appUser.SafeKey);
                    }
                    else
                    {
                        //MessagingCenter.Send(this, MessengerCodes.CreateNewAccountFailed, message.Json);
                        Debug.Log("CreateAccount error : " + message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Client_Authenticate:
                {
                    if (message.Success)
                    {
                        var authToken = JsonConvert.DeserializeObject<Guid>(message.Json);

                        _dispatchAuthToken = authToken;
                        //MessagingCenter.Send(this, MessengerCodes.LoginSuccessful);
                        AskServerInfos();
                    }
                    else
                    {
                        //MessagingCenter.Send(this, MessengerCodes.LoginFailed, message.Json);
                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            PlayerPrefs.DeleteKey("AiosKingdom_IdentifyingKey");
                            PlayerPrefs.Save();
                            UIManager.This.ShowAccountForm();
                        });

                        Debug.Log("Authenticate error : " + message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Client_ServerList:
                {
                    var servers = JsonConvert.DeserializeObject<List<JsonObjects.GameServerInfos>>(message.Json);
                    //MessagingCenter.Send(this, MessengerCodes.ServerListReceived, servers);
                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        UIManager.This.ShowServerList(servers);
                    });
                }
                break;
            case JsonObjects.CommandCodes.Client_AnnounceGameConnection:
                {
                    if (message.Success)
                    {
                        var connection = JsonConvert.DeserializeObject<JsonObjects.GameServerConnection>(message.Json);
                        ConnectToGameServer(connection);
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Server Connection", message.Json);
                        Debug.Log("Server Connection : " + message.Json);
                        AskServerInfos();
                    }
                }
                break;
            case JsonObjects.CommandCodes.Client_RetrieveAccount:
                {
                    if (message.Success)
                    {
                        var appUser = JsonConvert.DeserializeObject<JsonObjects.NewAccount>(message.Json);

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            PlayerPrefs.SetString("AiosKingdom_IdentifyingKey", appUser.Identifier.ToString());
                            PlayerPrefs.Save();
                            AskAuthentication(appUser.Identifier.ToString());
                        });
                        //MessagingCenter.Send(this, MessengerCodes.RetrievedAccount);
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Failed", message.Json);
                    }
                }
                break;
            default:
                return false;
        }

        //_dispatchTimedOut = DateTime.Now;

        return true;
    }

    public void Disconnect()
    {
        if (_isDispatchRunning && _dispatch.Connected)
        {
            _isDispatchRunning = false;
        }
    }

    public void AskNewAccount()
    {
        var args = new string[0];
        var retMess = new JsonObjects.Message
        {
            Code = JsonObjects.CommandCodes.Client_CreateAccount,
            Json = JsonUtility.ToJson(args)
        };
        SendJsonToDispatch(JsonUtility.ToJson(retMess));
    }

    public void AskOldAccount(string publicKey)
    {
        var args = new string[1] { publicKey };
        var retMess = new JsonObjects.Message
        {
            Code = JsonObjects.CommandCodes.Client_RetrieveAccount,
            Json = JsonConvert.SerializeObject(args)
        };
        SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
    }

    public void AskAuthentication(string identifier)
    {
        var args = new string[1] { identifier };
        var retMess = new JsonObjects.Message
        {
            Code = JsonObjects.CommandCodes.Client_Authenticate,
            Json = JsonConvert.SerializeObject(args)
        };
        SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
    }

    public void AskServerInfos()
    {
        var args = new string[0];
        var retMess = new JsonObjects.Message
        {
            Code = JsonObjects.CommandCodes.Client_ServerList,
            Json = JsonConvert.SerializeObject(args),
            Token = _dispatchAuthToken
        };
        SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
    }

    public void AnnounceGameServerConnection(Guid serverId)
    {
        var args = new string[1];
        args[0] = serverId.ToString();
        var retMess = new JsonObjects.Message
        {
            Code = JsonObjects.CommandCodes.Client_AnnounceGameConnection,
            Json = JsonConvert.SerializeObject(args),
            Token = _dispatchAuthToken
        };
        SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
    }

    public void AnnounceDisconnection()
    {
        var retMess = new JsonObjects.Message
        {
            Code = JsonObjects.CommandCodes.Client_AnnounceDisconnection,
            Json = JsonConvert.SerializeObject(new string[0]),
            Token = _dispatchAuthToken
        };
        SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
    }

    private void SendJsonToDispatch(string json)
    {
        if (!_isDispatchRunning) return;

        var encoder = new ASCIIEncoding();
        var bytes = encoder.GetBytes(json + '|');

        try
        {
            var result = _dispatch.Client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, (asyncResult) =>
            {
                try
                {
                    _dispatch.Client.EndSend(asyncResult);
                }
                catch (SocketException sockE)
                {
                    //MessagingCenter.Send(this, MessengerCodes.Disconnected, sockE.Message);
                    Debug.Log("Socket exception : " + sockE);
                }
            }, null);

            if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
            {
                //MessagingCenter.Send(this, MessengerCodes.Disconnected, "Connection lost, please reconnect..");
                Debug.Log("Connection lost, please reconnect..");
            }
        }
        catch (SocketException sockE)
        {
            Debug.Log("Socket exception : " + sockE);
        }
        catch (ObjectDisposedException disposedE)
        {
            Debug.Log("ObjectDisposed exception : " + disposedE);
        }
    }

    #endregion

    #region Game Functions

    public void ConnectToGameServer(JsonObjects.GameServerConnection connection)
    {
        try
        {
            _gameConnection = connection;
            _gameThread = new Thread(new ThreadStart(ListenForGameData));
            _gameThread.IsBackground = true;
            _gameThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception : " + e);
        }
    }

    private void ListenForGameData()
    {
        try
        {
            _game = new TcpClient(_gameConnection.Host, _gameConnection.Port);
            _isGameRunning = true;

            var args = new string[1];
            args[0] = _gameConnection.Token.ToString();
            var retMess = new JsonObjects.Message
            {
                Code = JsonObjects.CommandCodes.Client_Authenticate,
                Json = JsonConvert.SerializeObject(args)
            };
            SendJsonToGame(JsonConvert.SerializeObject(retMess));

            while (_isGameRunning)
            {
                var bufferSize = _game.Available;

                if (bufferSize > 0)
                {
                    var client = _game.Client;
                    string bufferStr = _gameBuffer;
                    _gameBuffer = string.Empty;
                    int received = 0;

                    do
                    {
                        byte[] buffer = new byte[bufferSize > 256 ? 256 : bufferSize];
                        received = client.Receive(buffer);
                        if (received > 0)
                            bufferStr += Encoding.UTF8.GetString(buffer);
                        bufferSize -= received;
                    } while (received == 256);

                    var messages = bufferStr.Trim().Split('|');
                    foreach (var message in messages)
                    {
                        if (string.IsNullOrEmpty(message) || message.All(char.IsWhiteSpace))
                            continue;

                        try
                        {
                            var fromJson = JsonConvert.DeserializeObject<JsonObjects.Message>(message);

                            if (!ProcessGameMessage(fromJson))
                            {
                                /*_message = buffer;
                                NotifyPropertyChanged(nameof(Message));*/
                            }
                        }
                        catch
                        {
                            _gameBuffer += message;
                        }
                    }
                }
            }

            if (_game.Connected)
                _game.GetStream().Flush();

            _game.Client.Close();
            _game.Close();
        }
        catch (SocketException sockE)
        {
            Debug.Log("Socket exception : " + sockE);
        }
    }

    public void DisconnectGame()
    {
        if (_game != null && _game.Connected)
        {
            _isGameRunning = false;
        }
    }

    private bool ProcessGameMessage(JsonObjects.Message message)
    {
        switch (message.Code)
        {
            //case Network.CommandCodes.Ping:
            //    {
            //        SendRequest(Network.CommandCodes.Ping);
            //    }
            //    break;
            case JsonObjects.CommandCodes.GlobalMessage:
                {
                    //ScreenManager.Instance.AlertScreen("Kingdom Message", message.Json);
                    Debug.Log("Server Message : " + message.Json);
                }
                break;

            // SERVER
            case JsonObjects.CommandCodes.Client_Authenticate:
                {
                    if (message.Success)
                    {
                        var authToken = JsonConvert.DeserializeObject<Guid>(message.Json);

                        if (!Guid.Empty.Equals(authToken))
                        {
                            _gameAuthToken = authToken;
                            AskSoulList();
                        }
                    }
                    else
                    {
                        //MessagingCenter.Send(this, MessengerCodes.ConnectedToServerFailed, message.Json);
                        Debug.Log("Authenticate error : " + message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Server.CreateSoul:
                {
                    if (message.Success)
                    {
                        AskSoulList();
                        //MessagingCenter.Send(this, MessengerCodes.CreateSoulSuccess);
                    }
                    else
                    {
                        //MessagingCenter.Send(this, MessengerCodes.CreateSoulFailed);
                        Debug.Log("New Soul error : " + message.Json);
                    }
                    //ScreenManager.Instance.AlertScreen("Soul Creation", message.Json);
                }
                break;
            case JsonObjects.CommandCodes.Server.SoulList:
                {
                    var souls = JsonConvert.DeserializeObject<List<JsonObjects.SoulInfos>>(message.Json);

                    //MessagingCenter.Send(this, MessengerCodes.SoulListReceived, souls);
                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        UIManager.This.ShowSoulList(souls);
                    });
                }
                break;
            case JsonObjects.CommandCodes.Server.ConnectSoul:
                {
                    if (message.Success)
                    {
                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            UIManager.This.ShowContentLoadingScreen();
                        });

                        //MessagingCenter.Send(this, MessengerCodes.SoulConnected);
                        AskItemList();
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Soul Connection", message.Json);
                        Debug.Log("Connect Soul error : " + message.Json);
                    }
                }
                break;

            // PLAYER
            case JsonObjects.CommandCodes.Player.CurrentSoulDatas:
                {
                    if (message.Success)
                    {
                        var soulDatas = JsonConvert.DeserializeObject<JsonObjects.SoulDatas>(message.Json);
                        DatasManager.Instance.Datas = soulDatas;

                        if (soulDatas.TotalStamina == 0 && soulDatas.TotalEnergy == 0
                            && soulDatas.TotalStrength == 0 && soulDatas.TotalAgility == 0
                            && soulDatas.TotalIntelligence == 0 && soulDatas.TotalWisdom == 0)
                        {
                            //Application.Current.Properties["AiosKingdom_TutorialStep"] = 3;
                            //Application.Current.SavePropertiesAsync();
                            //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
                        }

                        //MessagingCenter.Send(this, MessengerCodes.SoulDatasUpdated);
                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Home.UpdatePlayerDatas();
                        });
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Current Soul Datas", message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Player.Market_PlaceOrder:
                {
                    if (message.Success)
                    {
                        AskCurrencies();
                        AskMarketItems();
                    }
                    //MessagingCenter.Send(this, MessengerCodes.BuyMarketItem, message.Json);
                }
                break;
            case JsonObjects.CommandCodes.Player.Market_OrderProcessed:
                {
                    if (message.Success)
                    {
                        Debug.Log(JsonConvert.DeserializeObject<JsonObjects.MarketOrderProcessed>(message.Json));

                        AskInventory();
                    }
                    //MessagingCenter.Send(this, MessengerCodes.BuyMarketItem, message.Json);
                }
                break;
            case JsonObjects.CommandCodes.Player.EquipItem:
                {
                    if (message.Success)
                    {
                        AskInventory();
                        AskEquipment();
                        AskSoulCurrentDatas();
                    }
                    //MessagingCenter.Send(this, MessengerCodes.ItemEquiped, message.Json);
                }
                break;
            //case Network.CommandCodes.Player.SellItem:
            //    {
            //        if (message.Success)
            //        {
            //            AskInventory();
            //            AskCurrencies();
            //        }
            //        MessagingCenter.Send(this, MessengerCodes.ItemSold, message.Json);
            //    }
            //    break;
            case JsonObjects.CommandCodes.Player.UseSpiritPills:
                {
                    if (message.Success)
                    {
                        AskCurrencies();
                        AskSoulCurrentDatas();

                        //Application.Current.Properties["AiosKingdom_TutorialStep"] = 4;
                        //Application.Current.SavePropertiesAsync();
                        //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
                    }

                    //MessagingCenter.Send(this, MessengerCodes.LearnSpiritPills, message.Json);
                }
                break;
            case JsonObjects.CommandCodes.Player.LearnSkill:
                {
                    if (message.Success)
                    {
                        AskCurrencies();
                        AskKnowledges();

                        //Application.Current.Properties["AiosKingdom_TutorialStep"] = 3;
                        //Application.Current.SavePropertiesAsync();
                        //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
                    }
                    //MessagingCenter.Send(this, MessengerCodes.SkillLearned, message.Json);
                }
                break;
            case JsonObjects.CommandCodes.Player.LearnTalent:
                {
                    if (message.Success)
                    {
                        AskKnowledges();

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            UIManager.This.HideLoading();
                        });

                        //Application.Current.Properties["AiosKingdom_TutorialStep"] = 3;
                        //Application.Current.SavePropertiesAsync();
                        //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
                    }
                    //MessagingCenter.Send(this, MessengerCodes.SkillLearned, message.Json);
                }
                break;
            case JsonObjects.CommandCodes.Player.Currencies:
                {
                    if (message.Success)
                    {
                        var currencies = JsonConvert.DeserializeObject<JsonObjects.Currencies>(message.Json);
                        DatasManager.Instance.Currencies = currencies;

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Home.UpdateCurrencies();
                            Pills.UpdateCurrencies();
                        });
                    }
                    //MessagingCenter.Send(this, MessengerCodes.CurrenciesUpdated);
                }
                break;
            case JsonObjects.CommandCodes.Player.Inventory:
                {
                    if (message.Success)
                    {
                        var inventory = JsonConvert.DeserializeObject<List<JsonObjects.InventorySlot>>(message.Json);
                        DatasManager.Instance.Inventory = inventory;

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Inventory.UpdateItems();
                        });
                    }
                    //MessagingCenter.Send(this, MessengerCodes.InventoryUpdated);
                }
                break;
            case JsonObjects.CommandCodes.Player.Knowledges:
                {
                    if (message.Success)
                    {
                        var knowledges = JsonConvert.DeserializeObject<List<JsonObjects.Knowledge>>(message.Json);

                        if (knowledges.Count == 0)
                        {
                            //Application.Current.Properties["AiosKingdom_IsNewCharacter"] = true;
                            //Application.Current.Properties["AiosKingdom_TutorialStep"] = 1;
                            //Application.Current.SavePropertiesAsync();
                            //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
                        }

                        DatasManager.Instance.Knowledges = knowledges;

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Knowledges.LoadKnowledges();
                            Talents.LoadTalents();
                        });
                    }
                    //MessagingCenter.Send(this, MessengerCodes.KnowledgeUpdated);
                }
                break;
            case JsonObjects.CommandCodes.Player.Equipment:
                {
                    if (message.Success)
                    {
                        var equipment = JsonConvert.DeserializeObject<JsonObjects.Equipment>(message.Json);
                        DatasManager.Instance.Equipment = equipment;

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Equipment.UpdateEquipment();
                        });
                    }
                    //MessagingCenter.Send(this, MessengerCodes.EquipmentUpdated);
                }
                break;

            // LISTING
            case JsonObjects.CommandCodes.Listing.Item:
                {
                    var items = JsonConvert.DeserializeObject<List<JsonObjects.Items.Item>>(message.Json);
                    DatasManager.Instance.Items = items;

                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        ContentLoadingScreen.IsLoaded("items");
                        AskBookList();
                    });
                }
                break;
            case JsonObjects.CommandCodes.Listing.Book:
                {
                    var books = JsonConvert.DeserializeObject<List<JsonObjects.Skills.Book>>(message.Json);
                    DatasManager.Instance.Books = books;

                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        ContentLoadingScreen.IsLoaded("books");
                        AskMonsterList();
                    });
                }
                break;
            case JsonObjects.CommandCodes.Listing.Monster:
                {
                    var monsters = JsonConvert.DeserializeObject<List<JsonObjects.Monsters.Monster>>(message.Json);
                    DatasManager.Instance.Monsters = monsters;

                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        ContentLoadingScreen.IsLoaded("monsters");
                        AskAdventureList();
                    });
                }
                break;
            case JsonObjects.CommandCodes.Listing.Dungeon:
                {
                    var dungeons = JsonConvert.DeserializeObject<List<JsonObjects.Adventures.Adventure>>(message.Json);
                    DatasManager.Instance.Dungeons = dungeons;

                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        ContentLoadingScreen.IsLoaded("adventures");
                        UIManager.This.ShowLoading();
                        UIManager.This.ShowHome();
                    });
                    //MessagingCenter.Send(this, MessengerCodes.DungeonListUpdated);
                }
                break;
            case JsonObjects.CommandCodes.Listing.Market:
                {
                    //, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                    var items = JsonConvert.DeserializeObject<List<JsonObjects.MarketSlot>>(message.Json);
                    DatasManager.Instance.MarketItems = items;

                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        Market.UpdateItems();
                    });
                    //MessagingCenter.Send(this, MessengerCodes.MarketUpdated);
                }
                break;
            case JsonObjects.CommandCodes.Listing.SpecialsMarket:
                {
                    //, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                    var items = JsonConvert.DeserializeObject<List<JsonObjects.MarketSlot>>(message.Json);
                    DatasManager.Instance.SpecialMarketItems = items;

                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        Market.UpdateSpecialItems();
                    });
                    //MessagingCenter.Send(this, MessengerCodes.MarketUpdated);
                }
                break;

            // DUNGEON
            case JsonObjects.CommandCodes.Dungeon.Enter:
                {
                    if (message.Success)
                    {
                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            UIManager.This.StartAdventure();
                            Adventure.StartDungeon();
                        });
                        //MessagingCenter.Send(this, MessengerCodes.EnterDungeon);
                    }
                    else
                    {
                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            UIManager.This.HideLoading();
                        });
                        //ScreenManager.Instance.AlertScreen("Enter Dungeon", message.Json);
                    }
                    Debug.Log(message.Json);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.EnterRoom:
                {
                    if (message.Success)
                    {
                        UpdateDungeonRoom();
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Enter Room", message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.Exit:
                {
                    if (message.Success)
                    {
                        AskCurrencies();
                        AskInventory();
                        AskSoulCurrentDatas();

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            UIManager.This.ShowHome();
                        });
                        //MessagingCenter.Send(this, MessengerCodes.ExitedDungeon);
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Exit Room", message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.UpdateRoom:
                {
                    if (message.Success)
                    {
                        var adventure = JsonConvert.DeserializeObject<JsonObjects.AdventureState>(message.Json);
                        DatasManager.Instance.Adventure = adventure;

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Adventure.UpdateCurrentState();
                        });
                        //MessagingCenter.Send(this, MessengerCodes.DungeonUpdated);
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Update Room", message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.UseSkill:
                {
                    if (message.Success)
                    {
                        UpdateDungeonRoom();

                        var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);
                        //MessagingCenter.Send(this, MessengerCodes.RoundResults, arList);

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Adventure.LogResults(arList);
                            Adventure.TriggerEnemyTurn();
                        });
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Skill", message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.UseConsumable:
                {
                    if (message.Success)
                    {
                        AskInventory();
                        UpdateDungeonRoom();

                        var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);
                        //MessagingCenter.Send(this, MessengerCodes.RoundResults, arList);

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Adventure.LogResults(arList);
                            Adventure.TriggerEnemyTurn();
                        });
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Consumable", message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.EnemyTurn:
                {
                    if (message.Success)
                    {
                        UpdateDungeonRoom();

                        var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);
                        //MessagingCenter.Send(this, MessengerCodes.RoundResults, arList);

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Adventure.LogResults(arList);
                        });
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Turn", message.Json);
                    }
                    //MessagingCenter.Send(this, MessengerCodes.EnemyTurnEnded);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.DoNothingTurn:
                {
                    if (message.Success)
                    {
                        UpdateDungeonRoom();

                        var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);
                        //MessagingCenter.Send(this, MessengerCodes.RoundResults, arList);

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Adventure.LogResults(arList);
                            Adventure.TriggerEnemyTurn();
                        });
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Turn", message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.GetLoots:
                {
                    if (message.Success)
                    {
                        var loots = JsonConvert.DeserializeObject<List<JsonObjects.LootItem>>(message.Json);
                        //MessagingCenter.Send(this, MessengerCodes.DungeonLootsReceived, loots);

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Adventure.ShowLoots(loots);
                        });
                    }
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.LootItem:
                {
                    if (message.Success)
                    {
                        GetDungeonRoomLoots();
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Looting", message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.LeaveFinishedRoom:
                {
                    if (message.Success)
                    {
                        var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);
                        //MessagingCenter.Send(this, MessengerCodes.RoundResults, arList);

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Adventure.ShowEndResults(arList);
                        });
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Dungeon", message.Json);
                    }
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.BuyShopItem:
                {
                    if (message.Success)
                    {
                        AskCurrencies();
                        AskInventory();
                        UpdateDungeonRoom();
                    }
                    //MessagingCenter.Send(this, MessengerCodes.BuyMarketItem, message.Json);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.PlayerDied:
                {
                    var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);
                    //MessagingCenter.Send(this, MessengerCodes.PlayerDied, arList);

                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        Adventure.ShowEndResults(arList);
                    });
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.PlayerRest:
                {
                    if (message.Success)
                    {
                        OpenDungeonRoom();

                        var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);
                        //MessagingCenter.Send(this, MessengerCodes.RoundResults, arList);

                        SceneLoom.Loom.QueueOnMainThread(() =>
                        {
                            Adventure.LogResults(arList);
                        });
                    }
                    else
                    {
                        //ScreenManager.Instance.AlertScreen("Resting", message.Json);
                    }
                }
                break;

            default:
                return false;
        }

        //_gameTimedOut = DateTime.Now;

        return true;
    }

    #region Server Commands

    public void AskSoulList()
    {
        SendRequest(JsonObjects.CommandCodes.Server.SoulList);
    }

    public void CreateSoul(string soulname)
    {
        SendRequest(JsonObjects.CommandCodes.Server.CreateSoul, new string[1] { soulname });
    }

    public void ConnectSoul(Guid id)
    {
        SendRequest(JsonObjects.CommandCodes.Server.ConnectSoul, new string[1] { id.ToString() });
    }

    public void DisconnectSoul()
    {
        SendRequest(JsonObjects.CommandCodes.Server.DisconnectSoul);
    }

    #endregion

    #region Listing Commands

    public void AskItemList()
    {
        SendRequest(JsonObjects.CommandCodes.Listing.Item);
    }

    public void AskBookList()
    {
        SendRequest(JsonObjects.CommandCodes.Listing.Book);
    }

    public void AskMonsterList()
    {
        SendRequest(JsonObjects.CommandCodes.Listing.Monster);
    }

    public void AskAdventureList()
    {
        SendRequest(JsonObjects.CommandCodes.Listing.Dungeon);
    }

    public void AskMarketItems()
    {
        SendRequest(JsonObjects.CommandCodes.Listing.Market);
    }

    public void AskSpecialMarketItems()
    {
        SendRequest(JsonObjects.CommandCodes.Listing.SpecialsMarket);
    }

    #endregion

    #region Player Commands 

    public void AskSoulCurrentDatas()
    {
        SendRequest(JsonObjects.CommandCodes.Player.CurrentSoulDatas);
    }

    public void OrderMarketItem(Guid slotId)
    {
        SendRequest(JsonObjects.CommandCodes.Player.Market_PlaceOrder, new string[1] { slotId.ToString() });
    }

    public void EquipItem(Guid slotId)
    {
        SendRequest(JsonObjects.CommandCodes.Player.EquipItem, new string[1] { slotId.ToString() });
    }

    //public void SellItem(Guid slotId)
    //{
    //    SendRequest(Network.CommandCodes.Player.SellItem, new string[1] { slotId.ToString() });
    //}

    public void UseSpiritPills(JsonObjects.Stats statType, int quantity)
    {
        SendRequest(JsonObjects.CommandCodes.Player.UseSpiritPills, new string[2] { statType.ToString(), quantity.ToString() });
    }

    public void LearnSkill(Guid bookId)
    {
        SendRequest(JsonObjects.CommandCodes.Player.LearnSkill, new string[1] { bookId.ToString() });
    }

    public void LearnTalent(Guid talentId)
    {
        SendRequest(JsonObjects.CommandCodes.Player.LearnTalent, new string[1] { talentId.ToString() });
    }

    public void AskCurrencies()
    {
        SendRequest(JsonObjects.CommandCodes.Player.Currencies);
    }

    public void AskInventory()
    {
        SendRequest(JsonObjects.CommandCodes.Player.Inventory);
    }

    public void AskKnowledges()
    {
        SendRequest(JsonObjects.CommandCodes.Player.Knowledges);
    }

    public void AskEquipment()
    {
        SendRequest(JsonObjects.CommandCodes.Player.Equipment);
    }

    #endregion

    #region Dungeon Commands

    public void EnterDungeon(Guid dungeonId, List<JsonObjects.AdventureState.BagItem> items)
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.Enter, new string[2] { dungeonId.ToString(), JsonConvert.SerializeObject(items) });
    }

    public void OpenDungeonRoom()
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.EnterRoom);
    }

    public void UpdateDungeonRoom()
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.UpdateRoom);
    }

    public void EnemyTurn()
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.EnemyTurn);
    }

    public void DoNothingTurn()
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.DoNothingTurn);
    }

    public void BuyShopItem(Guid tempId, int quantity)
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.BuyShopItem, new string[2] { tempId.ToString(), quantity.ToString() });
    }

    public void AdventureUseSkill(Guid knowledgeId, Guid enemyId)
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.UseSkill, new string[2] { knowledgeId.ToString(), enemyId.ToString() });
    }

    public void AdventureUseConsumable(Guid slotId, Guid enemyId)
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.UseConsumable, new string[2] { slotId.ToString(), enemyId.ToString() });
    }

    public void GetDungeonRoomLoots()
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.GetLoots);
    }

    public void LootDungeonItem(Guid lootId)
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.LootItem, new string[1] { lootId.ToString() });
    }

    public void ExitDungeon()
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.Exit);
    }

    public void LeaveFinishedRoom()
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.LeaveFinishedRoom);
    }

    public void PlayerRest()
    {
        SendRequest(JsonObjects.CommandCodes.Dungeon.PlayerRest);
    }

    public void DungeonLeft()
    {
        //MessagingCenter.Send(this, MessengerCodes.SoulConnected);
        AskCurrencies();
        AskInventory();
        AskSoulCurrentDatas();
    }

    #endregion

    private void SendRequest(int code, string[] args = null)
    {
        if (!_isGameRunning) return;

        var retMess = new JsonObjects.Message
        {
            Code = code,
            Json = JsonConvert.SerializeObject(args != null ? args : new string[0]),
            Token = _gameAuthToken
        };
        SendJsonToGame(JsonConvert.SerializeObject(retMess));
    }

    private void SendJsonToGame(string json)
    {
        var encoder = new ASCIIEncoding();
        var bytes = encoder.GetBytes(json + '|');

        try
        {
            var result = _game.Client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, (asyncResult) =>
            {
                try
                {
                    _game.Client.EndSend(asyncResult);
                }
                catch (SocketException sockE)
                {
                    //MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, sockE.Message);
                    Debug.Log("Socket Exception : " + sockE.Message);
                }
            }, null);

            if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
            {
                //MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, "Connection lost, please reconnect..");
                Debug.Log("Connection lost, please reconnect..");
            }
        }
        catch (SocketException sockE)
        {
            //MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, $"{sockE.StackTrace} : {sockE.Message}");
            Debug.Log("Socket Exception : " + sockE.Message);
        }
        catch (NullReferenceException nullE)
        {
            //MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, $"{nullE.StackTrace} : {nullE.Message}");
            Debug.Log("NullRef Exception : " + nullE.Message);
        }
        catch (ObjectDisposedException disposedE)
        {
            //MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, $"{disposedE.StackTrace} : {disposedE.Message}");
            Debug.Log("ObjDisposed Exception : " + disposedE.Message);
        }
    }

    #endregion
}
