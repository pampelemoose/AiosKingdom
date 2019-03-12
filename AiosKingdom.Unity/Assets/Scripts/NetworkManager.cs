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

    // CALLBACKS
    public event Action<string> GlobalMessageCallback;
    public class NetworkCallback
    {
        public event Action<JsonObjects.Message> Callback;

        public void Call(JsonObjects.Message message)
        {
            if (Callback != null)
            {
                Callback.Invoke(message);
            }
        }
    }
    private Dictionary<int, NetworkCallback> _callbacks = new Dictionary<int, NetworkCallback>();

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

    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (!hasFocus)
    //    {
    //        _network.DisconnectGame();
    //        _network.Disconnect();
    //    }
    //}

    public void AddCallback(int id, Action<JsonObjects.Message> callback)
    {
        if (_callbacks.ContainsKey(id))
        {
            _callbacks[id].Callback += callback;
        }
        else
        {
            var networkCallback = new NetworkCallback();
            networkCallback.Callback += callback;
            _callbacks.Add(id, networkCallback);
        }
    }

    private void _invokeCallback(int id, JsonObjects.Message message)
    {
        if (_callbacks.ContainsKey(id))
        {
            _callbacks[id].Call(message);
        }
    }

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

                    if (GlobalMessageCallback != null) GlobalMessageCallback.Invoke(message.Json);
                }
                break;

            case JsonObjects.CommandCodes.Client_CreateAccount:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Client_CreateAccount, message);
                }
                break;
            case JsonObjects.CommandCodes.Client_Authenticate:
                {
                    if (message.Success)
                    {
                        var authToken = JsonConvert.DeserializeObject<Guid>(message.Json);
                        _dispatchAuthToken = authToken;
                        AskServerList();
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Client_Authenticate, message);
                }
                break;
            case JsonObjects.CommandCodes.Client_ServerList:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Client_ServerList, message);
                }
                break;
            case JsonObjects.CommandCodes.Client_AnnounceGameConnection:
                {
                    if (message.Success)
                    {
                        var connection = JsonConvert.DeserializeObject<JsonObjects.GameServerConnection>(message.Json);
                        ConnectToGameServer(connection);
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Client_AnnounceGameConnection, message);
                }
                break;
            case JsonObjects.CommandCodes.Client_RetrieveAccount:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Client_RetrieveAccount, message);
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

    public void AskServerList()
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
                Code = JsonObjects.CommandCodes.Game_Authenticate,
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

                    if (GlobalMessageCallback != null) GlobalMessageCallback.Invoke(message.Json);
                }
                break;

            // SERVER
            case JsonObjects.CommandCodes.Game_Authenticate:
                {
                    if (message.Success)
                    {
                        var authToken = JsonConvert.DeserializeObject<Guid>(message.Json);

                        _gameAuthToken = authToken;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Game_Authenticate, message);
                }
                break;
            case JsonObjects.CommandCodes.Server.CreateSoul:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Server.CreateSoul, message);
                }
                break;
            case JsonObjects.CommandCodes.Server.SoulList:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Server.SoulList, message);
                }
                break;
            case JsonObjects.CommandCodes.Server.ConnectSoul:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Server.ConnectSoul, message);
                }
                break;

            // PLAYER
            case JsonObjects.CommandCodes.Player.CurrentSoulDatas:
                {
                    if (message.Success)
                    {
                        var soulDatas = JsonConvert.DeserializeObject<JsonObjects.SoulDatas>(message.Json);
                        DatasManager.Instance.Datas = soulDatas;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Player.CurrentSoulDatas, message);
                }
                break;
            case JsonObjects.CommandCodes.Player.Market_PlaceOrder:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Player.Market_PlaceOrder, message);
                }
                break;
            case JsonObjects.CommandCodes.Player.Market_OrderProcessed:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Player.Market_OrderProcessed, message);
                }
                break;
            case JsonObjects.CommandCodes.Player.EquipItem:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Player.EquipItem, message);
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
                    _invokeCallback(JsonObjects.CommandCodes.Player.UseSpiritPills, message);
                }
                break;
            case JsonObjects.CommandCodes.Player.LearnSkill:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Player.LearnSkill, message);
                }
                break;
            case JsonObjects.CommandCodes.Player.LearnTalent:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Player.LearnTalent, message);
                }
                break;
            case JsonObjects.CommandCodes.Player.Currencies:
                {
                    if (message.Success)
                    {
                        var currencies = JsonConvert.DeserializeObject<JsonObjects.Currencies>(message.Json);
                        DatasManager.Instance.Currencies = currencies;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Player.Currencies, message);
                }
                break;
            case JsonObjects.CommandCodes.Player.Inventory:
                {
                    if (message.Success)
                    {
                        var inventory = JsonConvert.DeserializeObject<List<JsonObjects.InventorySlot>>(message.Json);
                        DatasManager.Instance.Inventory = inventory;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Player.Inventory, message);
                }
                break;
            case JsonObjects.CommandCodes.Player.Knowledges:
                {
                    if (message.Success)
                    {
                        var knowledges = JsonConvert.DeserializeObject<List<JsonObjects.Knowledge>>(message.Json);
                        DatasManager.Instance.Knowledges = knowledges;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Player.Knowledges, message);
                }
                break;
            case JsonObjects.CommandCodes.Player.Equipment:
                {
                    if (message.Success)
                    {
                        var equipment = JsonConvert.DeserializeObject<JsonObjects.Equipment>(message.Json);
                        DatasManager.Instance.Equipment = equipment;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Player.Equipment, message);
                }
                break;

            // LISTING
            case JsonObjects.CommandCodes.Listing.Item:
                {
                    if (message.Success)
                    {
                        var items = JsonConvert.DeserializeObject<List<JsonObjects.Items.Item>>(message.Json);
                        DatasManager.Instance.Items = items;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Listing.Item, message);
                }
                break;
            case JsonObjects.CommandCodes.Listing.Book:
                {
                    if (message.Success)
                    {
                        var books = JsonConvert.DeserializeObject<List<JsonObjects.Skills.Book>>(message.Json);
                        DatasManager.Instance.Books = books;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Listing.Book, message);
                }
                break;
            case JsonObjects.CommandCodes.Listing.Monster:
                {
                    if (message.Success)
                    {
                        var monsters = JsonConvert.DeserializeObject<List<JsonObjects.Monsters.Monster>>(message.Json);
                        DatasManager.Instance.Monsters = monsters;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Listing.Monster, message);
                }
                break;
            case JsonObjects.CommandCodes.Listing.Dungeon:
                {
                    if (message.Success)
                    {
                        var dungeons = JsonConvert.DeserializeObject<List<JsonObjects.Adventures.Adventure>>(message.Json);
                        DatasManager.Instance.Dungeons = dungeons;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Listing.Dungeon, message);
                }
                break;
            case JsonObjects.CommandCodes.Listing.Recipes:
                {
                    if (message.Success)
                    {
                        var recipes = JsonConvert.DeserializeObject<List<JsonObjects.Recipe>>(message.Json);
                        DatasManager.Instance.Recipes = recipes;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Listing.Recipes, message);
                }
                break;
            case JsonObjects.CommandCodes.Listing.Market:
                {
                    if (message.Success)
                    {
                        //, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                        var items = JsonConvert.DeserializeObject<List<JsonObjects.MarketSlot>>(message.Json);
                        DatasManager.Instance.MarketItems = items;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Listing.Market, message);
                }
                break;
            case JsonObjects.CommandCodes.Listing.SpecialsMarket:
                {
                    if (message.Success)
                    {
                        //, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                        var items = JsonConvert.DeserializeObject<List<JsonObjects.MarketSlot>>(message.Json);
                        DatasManager.Instance.SpecialMarketItems = items;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Listing.SpecialsMarket, message);
                }
                break;

            // DUNGEON
            case JsonObjects.CommandCodes.Dungeon.Enter:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.Enter, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.EnterRoom:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.EnterRoom, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.Exit:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.Exit, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.UpdateRoom:
                {
                    if (message.Success)
                    {
                        var adventure = JsonConvert.DeserializeObject<JsonObjects.AdventureState>(message.Json);
                        DatasManager.Instance.Adventure = adventure;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.UpdateRoom, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.UseSkill:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.UseSkill, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.UseConsumable:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.UseConsumable, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.EnemyTurn:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.EnemyTurn, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.DoNothingTurn:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.DoNothingTurn, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.GetLoots:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.GetLoots, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.LootItem:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.LootItem, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.LeaveFinishedRoom:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.LeaveFinishedRoom, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.BuyShopItem:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.BuyShopItem, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.PlayerDied:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.PlayerDied, message);
                }
                break;
            case JsonObjects.CommandCodes.Dungeon.PlayerRest:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Dungeon.PlayerDied, message);
                }
                break;

            // JOB
            case JsonObjects.CommandCodes.Job.Get:
                {
                    if (message.Success)
                    {
                        var job = JsonConvert.DeserializeObject<JsonObjects.Job>(message.Json);
                        DatasManager.Instance.Job = job;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Job.Get, message);
                }
                break;
            case JsonObjects.CommandCodes.Job.Learn:
                {
                    if (message.Success)
                    {
                        var job = JsonConvert.DeserializeObject<JsonObjects.Job>(message.Json);
                        DatasManager.Instance.Job = job;
                    }

                    _invokeCallback(JsonObjects.CommandCodes.Job.Learn, message);
                }
                break;
            case JsonObjects.CommandCodes.Job.Craft:
                {
                    _invokeCallback(JsonObjects.CommandCodes.Job.Craft, message);
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

    public void AskRecipeList()
    {
        SendRequest(JsonObjects.CommandCodes.Listing.Recipes);
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

    #region Job Commands

    public void GetJob()
    {
        SendRequest(JsonObjects.CommandCodes.Job.Get);
    }

    public void LearnJob(JsonObjects.JobType type)
    {
        SendRequest(JsonObjects.CommandCodes.Job.Learn, new string[1] { type.ToString() });
    }

    public void CraftItem(JsonObjects.JobTechnique technique, List<JsonObjects.CraftingComponent> components)
    {
        SendRequest(JsonObjects.CommandCodes.Job.Craft, new string[2] { technique.ToString(), JsonConvert.SerializeObject(components) });
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
