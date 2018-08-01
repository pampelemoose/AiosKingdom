﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AiosKingdom
{
    public class NetworkManager
    {
        private static NetworkManager _instance;
        public static NetworkManager Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new NetworkManager();
                }

                return _instance;
            }
        }

        private TcpClient _dispatch;
        private bool _isDispatchRunning;
        private Guid _dispatchAuthToken;
        private string _dispatchBuffer;

        private TcpClient _game;
        private bool _isGameRunning;
        private Guid _gameAuthToken;
        private string _gameBuffer;

        private NetworkManager()
        {
        }

        #region Dispatch Functions

        private DateTime _dispatchTimedOut;

        private void RunDispatch()
        {
            _isDispatchRunning = true;
            _dispatchTimedOut = DateTime.Now;

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
                        if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message))
                            continue;

                        try
                        {
                            var fromJson = JsonConvert.DeserializeObject<Network.Message>(message);

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

                var diff = DateTime.Now - _dispatchTimedOut;
                if (diff.TotalSeconds > 10)
                {
                    MessagingCenter.Send(this, MessengerCodes.Disconnected, "Timed out.");
                    _isDispatchRunning = false;
                }
            }

            _dispatch.GetStream().Flush();
            _dispatch.Client.Close();
            _dispatch.Close();
        }

        private bool ProcessDispatchMessage(Network.Message message)
        {
            switch (message.Code)
            {
                case Network.CommandCodes.Ping:
                    {
                        var args = new string[0];
                        var retMess = new Network.Message
                        {
                            Code = Network.CommandCodes.Ping,
                            Json = JsonConvert.SerializeObject(args)
                        };
                        SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
                    }
                    break;
                case Network.CommandCodes.Client_CreateAccount:
                    {
                        if (message.Success)
                        {
                            var appUser = JsonConvert.DeserializeObject<DataModels.AppUser>(message.Json);

                            Application.Current.Properties["AiosKingdom_IdentifyingKey"] = appUser.Identifier.ToString();
                            Application.Current.SavePropertiesAsync();
                            MessagingCenter.Send(this, MessengerCodes.CreateNewAccount, appUser.SafeKey);
                        }
                        else
                        {
                            MessagingCenter.Send(this, MessengerCodes.CreateNewAccountFailed, message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Client_Authenticate:
                    {
                        if (message.Success)
                        {
                            var authToken = JsonConvert.DeserializeObject<Guid>(message.Json);

                            _dispatchAuthToken = authToken;
                            MessagingCenter.Send(this, MessengerCodes.LoginSuccessful);
                        }
                        else
                        {
                            MessagingCenter.Send(this, MessengerCodes.LoginFailed, message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Client_ServerList:
                    {
                        var servers = JsonConvert.DeserializeObject<List<Network.GameServerInfos>>(message.Json);
                        MessagingCenter.Send(this, MessengerCodes.ServerListReceived, servers);
                    }
                    break;
                case Network.CommandCodes.Client_AnnounceGameConnection:
                    {
                        if (message.Success)
                        {
                            var connection = JsonConvert.DeserializeObject<Network.GameServerConnection>(message.Json);
                            ConnectToGameServer(connection);
                        }
                        else
                        {
                            ScreenManager.Instance.AlertScreen("Server Connection", message.Json);
                            AskServerInfos();
                        }
                    }
                    break;
                case Network.CommandCodes.Client_AnnounceDisconnection:
                    {
                        MessagingCenter.Send(this, MessengerCodes.Disconnected, "You disconnected !");
                    }
                    break;
                case Network.CommandCodes.Client_RetrieveAccount:
                    {
                        if (message.Success)
                        {
                            var appUser = JsonConvert.DeserializeObject<DataModels.AppUser>(message.Json);

                            Application.Current.Properties["AiosKingdom_IdentifyingKey"] = appUser.Identifier.ToString();
                            Application.Current.SavePropertiesAsync();
                            MessagingCenter.Send(this, MessengerCodes.RetrievedAccount);
                        }
                        else
                        {
                            ScreenManager.Instance.AlertScreen("Failed", message.Json);
                        }
                    }
                    break;
                default:
                    return false;
            }

            _dispatchTimedOut = DateTime.Now;

            return true;
        }

        public void ConnectToServer()
        {
            if (_isDispatchRunning) return;

            try
            {
                _dispatch = new TcpClient();
                var result = _dispatch.BeginConnect("127.0.0.1", 1337, (asyncResult) =>
                {
                    try
                    {
                        Task.Factory.StartNew(RunDispatch, TaskCreationOptions.LongRunning);
                        _dispatch.EndConnect(asyncResult);
                    }
                    catch (SocketException sockE)
                    {
                        MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, sockE.Message);
                    }
                }, null);

                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, "Server unreachable.. Please try again later..");
                }
                return;
            }
            catch (SocketException sockE)
            {
                MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, sockE.Message);
                return;
            }
        }

        public void Disconnect()
        {
            if (_dispatch.Connected)
            {
                _isDispatchRunning = false;
            }
        }

        public void AskNewAccount()
        {
            var args = new string[0];
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_CreateAccount,
                Json = JsonConvert.SerializeObject(args)
            };
            SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
        }

        public void AskOldAccount(string publicKey)
        {
            var args = new string[1] { publicKey };
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_RetrieveAccount,
                Json = JsonConvert.SerializeObject(args)
            };
            SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
        }

        public void AskAuthentication(string identifier)
        {
            var args = new string[1] { identifier };
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_Authenticate,
                Json = JsonConvert.SerializeObject(args)
            };
            SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
        }

        public void AskServerInfos()
        {
            var args = new string[0];
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_ServerList,
                Json = JsonConvert.SerializeObject(args),
                Token = _dispatchAuthToken
            };
            SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
        }

        public void AnnounceGameServerConnection(Guid serverId)
        {
            var args = new string[1];
            args[0] = serverId.ToString();
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_AnnounceGameConnection,
                Json = JsonConvert.SerializeObject(args),
                Token = _dispatchAuthToken
            };
            SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
        }

        public void AnnounceDisconnection()
        {
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_AnnounceDisconnection,
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
                        MessagingCenter.Send(this, MessengerCodes.Disconnected, sockE.Message);
                    }
                }, null);

                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    MessagingCenter.Send(this, MessengerCodes.Disconnected, "Connection lost, please reconnect..");
                }
            }
            catch (SocketException sockE)
            {
                MessagingCenter.Send(this, MessengerCodes.Disconnected, sockE.Message);
            }
            catch (ObjectDisposedException disposedE)
            {
                MessagingCenter.Send(this, MessengerCodes.Disconnected, disposedE.Message);
            }
        }

        #endregion

        #region Game Function

        private DateTime _gameTimedOut;

        public void ConnectToGameServer(Network.GameServerConnection connection)
        {
            try
            {
                _game = new TcpClient();
                var result = _game.BeginConnect(connection.Host/*"10.0.2.2"*/, connection.Port, (asyncResult) =>
                {
                    try
                    {
                        Task.Factory.StartNew(RunGame, TaskCreationOptions.LongRunning);
                        _game.EndConnect(asyncResult);
                    }
                    catch (SocketException sockE)
                    {
                        //MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, sockE.Message);
                    }
                }, null);

                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    //MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, "Server unreachable.. Please try again later..");
                }

                var args = new string[1];
                args[0] = connection.Token.ToString();
                var retMess = new Network.Message
                {
                    Code = Network.CommandCodes.Client_Authenticate,
                    Json = JsonConvert.SerializeObject(args)
                };
                SendJsonToGame(JsonConvert.SerializeObject(retMess));
                return;
            }
            catch (SocketException sockE)
            {
                //MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, sockE.Message);
                return;
            }
        }

        public void DisconnectGame()
        {
            if (_game != null && _game.Connected)
            {
                _isGameRunning = false;
            }
        }

        private void RunGame()
        {
            _isGameRunning = true;
            _gameTimedOut = DateTime.Now;

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
                        if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message))
                            continue;

                        try
                        {
                            var fromJson = JsonConvert.DeserializeObject<Network.Message>(message);

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

                var diff = DateTime.Now - _gameTimedOut;
                if (diff.TotalSeconds > 10)
                {
                    MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, "Timed out.");
                    _isGameRunning = false;
                }
            }

            _game.GetStream().Flush();
            _game.Client.Close();
            _game.Close();
        }

        private bool ProcessGameMessage(Network.Message message)
        {
            switch (message.Code)
            {
                case Network.CommandCodes.Ping:
                    {
                        SendRequest(Network.CommandCodes.Ping);
                    }
                    break;

                // SERVER
                case Network.CommandCodes.Client_Authenticate:
                    {
                        if (message.Success)
                        {
                            var authToken = JsonConvert.DeserializeObject<Guid>(message.Json);

                            if (!Guid.Empty.Equals(authToken))
                            {
                                _gameAuthToken = authToken;
                                MessagingCenter.Send(this, MessengerCodes.ConnectedToServer);
                            }
                        }
                        else
                        {
                            MessagingCenter.Send(this, MessengerCodes.ConnectedToServerFailed, message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Server.CreateSoul:
                    {
                        if (message.Success)
                        {
                            AskSoulList();
                        }
                        ScreenManager.Instance.AlertScreen("Soul Creation", message.Json);
                    }
                    break;
                case Network.CommandCodes.Server.SoulList:
                    {
                        var souls = JsonConvert.DeserializeObject<List<DataModels.Soul>>(message.Json);

                        // TEMP BECAUSE OF UWP NAVBAR BUG
                        if (souls.Count == 0)
                            CreateSoul("test");

                        MessagingCenter.Send(this, MessengerCodes.SoulListReceived, souls);
                    }
                    break;
                case Network.CommandCodes.Server.ConnectSoul:
                    {
                        if (message.Success)
                        {
                            MessagingCenter.Send(this, MessengerCodes.SoulConnected);
                            AskArmorList();
                            AskConsumableList();
                            AskBagList();
                            AskWeaponList();
                            AskBookList();
                            AskMonsterList();
                            AskDungeonList();
                        }
                        else
                        {
                            ScreenManager.Instance.AlertScreen("Soul Connection", message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Server.DisconnectSoul:
                    {
                        // TODO : What if not success ? Is there a case ?
                        if (message.Success)
                        {
                            if (_isDispatchRunning)
                            {
                                AskServerInfos();

                                MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, "Disconnected.");
                            }
                        }
                    }
                    break;

                // PLAYER
                case Network.CommandCodes.Player.CurrentSoulDatas:
                    {
                        if (message.Success)
                        {
                            var soulDatas = JsonConvert.DeserializeObject<Network.SoulDatas>(message.Json);
                            DatasManager.Instance.Datas = soulDatas;
                            MessagingCenter.Send(this, MessengerCodes.SoulDatasUpdated);
                        }
                        else
                        {
                            ScreenManager.Instance.AlertScreen("Current Soul Datas", message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Player.BuyMarketItem:
                    {
                        if (message.Success)
                        {
                            AskCurrencies();
                            AskInventory();
                            AskMarketItems();
                        }
                        ScreenManager.Instance.AlertScreen("Market Item", message.Json);
                    }
                    break;
                case Network.CommandCodes.Player.EquipItem:
                    {
                        if (message.Success)
                        {
                            AskInventory();
                            AskEquipment();
                            AskSoulCurrentDatas();
                        }
                        ScreenManager.Instance.AlertScreen("Equip Item", message.Json);
                    }
                    break;
                case Network.CommandCodes.Player.UseSpiritPills:
                    {
                        if (message.Success)
                        {
                            AskCurrencies();
                            AskSoulCurrentDatas();
                        }
                        ScreenManager.Instance.AlertScreen("Spirit Pills", message.Json);
                    }
                    break;
                case Network.CommandCodes.Player.LearnSkill:
                    {
                        if (message.Success)
                        {
                            AskCurrencies();
                            AskKnowledges();
                        }
                        MessagingCenter.Send(this, MessengerCodes.SkillLearned, message.Json);
                    }
                    break;
                case Network.CommandCodes.Player.Currencies:
                    {
                        if (message.Success)
                        {
                            var currencies = JsonConvert.DeserializeObject<Network.Currencies>(message.Json);
                            DatasManager.Instance.Currencies = currencies;
                        }
                        MessagingCenter.Send(this, MessengerCodes.CurrenciesUpdated);
                    }
                    break;
                case Network.CommandCodes.Player.Inventory:
                    {
                        if (message.Success)
                        {
                            var inventory = JsonConvert.DeserializeObject<List<DataModels.InventorySlot>>(message.Json);
                            DatasManager.Instance.Inventory = inventory;
                        }
                        MessagingCenter.Send(this, MessengerCodes.InventoryUpdated);
                    }
                    break;
                case Network.CommandCodes.Player.Knowledges:
                    {
                        if (message.Success)
                        {
                            var knowledges = JsonConvert.DeserializeObject<List<DataModels.Knowledge>>(message.Json);
                            DatasManager.Instance.Knowledges = knowledges;
                        }
                        MessagingCenter.Send(this, MessengerCodes.KnowledgeUpdated);
                    }
                    break;
                case Network.CommandCodes.Player.Equipment:
                    {
                        if (message.Success)
                        {
                            var equipment = JsonConvert.DeserializeObject<DataModels.Equipment>(message.Json);
                            DatasManager.Instance.Equipment = equipment;
                        }
                        MessagingCenter.Send(this, MessengerCodes.EquipmentUpdated);
                    }
                    break;

                // LISTING
                case Network.CommandCodes.Listing.Armor:
                    {
                        var armors = JsonConvert.DeserializeObject<List<DataModels.Items.Armor>>(message.Json);
                        DatasManager.Instance.Armors = armors;
                    }
                    break;
                case Network.CommandCodes.Listing.Consumable:
                    {
                        var consumable = JsonConvert.DeserializeObject<List<DataModels.Items.Consumable>>(message.Json);
                        DatasManager.Instance.Consumables = consumable;
                    }
                    break;
                case Network.CommandCodes.Listing.Bag:
                    {
                        var bags = JsonConvert.DeserializeObject<List<DataModels.Items.Bag>>(message.Json);
                        DatasManager.Instance.Bags = bags;
                    }
                    break;
                case Network.CommandCodes.Listing.Weapon:
                    {
                        var weapons = JsonConvert.DeserializeObject<List<DataModels.Items.Weapon>>(message.Json);
                        DatasManager.Instance.Weapons = weapons;
                    }
                    break;
                case Network.CommandCodes.Listing.Book:
                    {
                        var books = JsonConvert.DeserializeObject<List<DataModels.Skills.Book>>(message.Json);
                        DatasManager.Instance.Books = books;
                    }
                    break;
                case Network.CommandCodes.Listing.Monster:
                    {
                        var monsters = JsonConvert.DeserializeObject<List<DataModels.Monsters.Monster>>(message.Json);
                        DatasManager.Instance.Monsters = monsters;
                    }
                    break;
                case Network.CommandCodes.Listing.Dungeon:
                    {
                        var dungeons = JsonConvert.DeserializeObject<List<DataModels.Dungeons.Dungeon>>(message.Json);
                        DatasManager.Instance.Dungeons = dungeons;
                    }
                    break;
                case Network.CommandCodes.Listing.Market:
                    {
                        var items = JsonConvert.DeserializeObject<List<DataModels.MarketSlot>>(message.Json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                        DatasManager.Instance.MarketItems = items;
                        MessagingCenter.Send(this, MessengerCodes.MarketUpdated);
                    }
                    break;

                // DUNGEON
                case Network.CommandCodes.Dungeon.Enter:
                    {
                        if (message.Success)
                        {
                            MessagingCenter.Send(this, MessengerCodes.EnterDungeon);
                        }
                        else
                        {
                            ScreenManager.Instance.AlertScreen("Enter Dungeon", message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Dungeon.EnterRoom:
                    {
                        if (message.Success)
                        {
                            UpdateDungeonRoom();
                        }
                        else
                        {
                            ScreenManager.Instance.AlertScreen("Enter Room", message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Dungeon.Exit:
                    {
                        if (message.Success)
                        {
                            AskCurrencies();
                            AskInventory();
                            AskSoulCurrentDatas();
                            MessagingCenter.Send(this, MessengerCodes.SoulConnected);
                        }
                        else
                        {
                            ScreenManager.Instance.AlertScreen("Exit Room", message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Dungeon.UpdateRoom:
                    {
                        if (message.Success)
                        {
                            var adventure = JsonConvert.DeserializeObject<Network.AdventureState>(message.Json);
                            DatasManager.Instance.Adventure = adventure;
                            MessagingCenter.Send(this, MessengerCodes.DungeonUpdated);
                        }
                        else
                        {
                            ScreenManager.Instance.AlertScreen("Update Room", message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Dungeon.UseSkill:
                    {
                        if (message.Success)
                        {
                            UpdateDungeonRoom();
                        }
                        ScreenManager.Instance.AlertScreen("Skill", message.Json);
                    }
                    break;
                case Network.CommandCodes.Dungeon.UseConsumable:
                    {
                        if (message.Success)
                        {
                            AskInventory();
                            UpdateDungeonRoom();
                        }
                        ScreenManager.Instance.AlertScreen("Consumable", message.Json);
                    }
                    break;
                case Network.CommandCodes.Dungeon.EnemyTurn:
                    {
                        if (message.Success)
                        {
                            UpdateDungeonRoom();
                        }
                        MessagingCenter.Send(this, MessengerCodes.EnemyTurnEnded);
                    }
                    break;
                case Network.CommandCodes.Dungeon.DoNothingTurn:
                    {
                        if (message.Success)
                        {
                            UpdateDungeonRoom();
                        }
                    }
                    break;
                case Network.CommandCodes.Dungeon.GetLoots:
                    {
                        if (message.Success)
                        {
                            var loots = JsonConvert.DeserializeObject<List<Network.LootItem>>(message.Json);
                            MessagingCenter.Send(this, MessengerCodes.DungeonLootsReceived, loots);
                        }
                    }
                    break;
                case Network.CommandCodes.Dungeon.LootItem:
                    {
                        if (message.Success)
                        {
                            GetDungeonRoomLoots();
                        }
                        ScreenManager.Instance.AlertScreen("Looting", message.Json);
                    }
                    break;
                case Network.CommandCodes.Dungeon.LeaveFinishedRoom:
                    {
                        if (message.Success)
                        {
                            MessagingCenter.Send(this, MessengerCodes.SoulConnected);
                            AskCurrencies();
                            AskInventory();
                            AskSoulCurrentDatas();
                        }
                        ScreenManager.Instance.AlertScreen("Room", message.Json);
                    }
                    break;
                case Network.CommandCodes.Dungeon.BuyShopItem:
                    {
                        if (message.Success)
                        {
                            AskCurrencies();
                            AskInventory();
                            UpdateDungeonRoom();
                        }
                        ScreenManager.Instance.AlertScreen("Shop", message.Json);
                    }
                    break;

                default:
                    return false;
            }

            _gameTimedOut = DateTime.Now;

            return true;
        }

        #region Server Commands

        public void AskSoulList()
        {
            SendRequest(Network.CommandCodes.Server.SoulList);
        }

        public void CreateSoul(string soulname)
        {
            SendRequest(Network.CommandCodes.Server.CreateSoul, new string[1] { soulname });
        }

        public void ConnectSoul(Guid id)
        {
            SendRequest(Network.CommandCodes.Server.ConnectSoul, new string[1] { id.ToString() });
        }

        public void DisconnectSoul()
        {
            SendRequest(Network.CommandCodes.Server.DisconnectSoul);
        }

        #endregion

        #region Listing Commands

        public void AskArmorList()
        {
            SendRequest(Network.CommandCodes.Listing.Armor);
        }

        public void AskConsumableList()
        {
            SendRequest(Network.CommandCodes.Listing.Consumable);
        }

        public void AskBagList()
        {
            SendRequest(Network.CommandCodes.Listing.Bag);
        }

        public void AskWeaponList()
        {
            SendRequest(Network.CommandCodes.Listing.Weapon);
        }

        public void AskBookList()
        {
            SendRequest(Network.CommandCodes.Listing.Book);
        }

        public void AskMonsterList()
        {
            SendRequest(Network.CommandCodes.Listing.Monster);
        }

        public void AskDungeonList()
        {
            SendRequest(Network.CommandCodes.Listing.Dungeon);
        }

        public void AskMarketItems()
        {
            SendRequest(Network.CommandCodes.Listing.Market);
        }

        #endregion

        #region Player Commands 

        public void AskSoulCurrentDatas()
        {
            SendRequest(Network.CommandCodes.Player.CurrentSoulDatas);
        }

        public void BuyMarketItem(Guid slotId, int quantity, bool isBit)
        {
            SendRequest(Network.CommandCodes.Player.BuyMarketItem, new string[3] { slotId.ToString(), quantity.ToString(), isBit.ToString() });
        }

        public void EquipItem(Guid slotId)
        {
            SendRequest(Network.CommandCodes.Player.EquipItem, new string[1] { slotId.ToString() });
        }

        public void UseSpiritPills(DataModels.Soul.Stats statType, int quantity)
        {
            SendRequest(Network.CommandCodes.Player.UseSpiritPills, new string[2] { statType.ToString(), quantity.ToString() });
        }

        public void LearnSkill(Guid bookId, int rank)
        {
            SendRequest(Network.CommandCodes.Player.LearnSkill, new string[2] { bookId.ToString(), rank.ToString() });
        }

        public void AskCurrencies()
        {
            SendRequest(Network.CommandCodes.Player.Currencies);
        }

        public void AskInventory()
        {
            SendRequest(Network.CommandCodes.Player.Inventory);
        }

        public void AskKnowledges()
        {
            SendRequest(Network.CommandCodes.Player.Knowledges);
        }

        public void AskEquipment()
        {
            SendRequest(Network.CommandCodes.Player.Equipment);
        }

        #endregion

        #region Dungeon Commands

        public void EnterDungeon(Guid dungeonId)
        {
            SendRequest(Network.CommandCodes.Dungeon.Enter, new string[1] { dungeonId.ToString() });
        }

        public void OpenDungeonRoom()
        {
            SendRequest(Network.CommandCodes.Dungeon.EnterRoom);
        }

        public void UpdateDungeonRoom()
        {
            SendRequest(Network.CommandCodes.Dungeon.UpdateRoom);
        }

        public void EnemyTurn()
        {
            SendRequest(Network.CommandCodes.Dungeon.EnemyTurn);
        }

        public void DoNothingTurn()
        {
            SendRequest(Network.CommandCodes.Dungeon.DoNothingTurn);
        }

        public void BuyShopItem(Guid tempId, int quantity)
        {
            SendRequest(Network.CommandCodes.Dungeon.BuyShopItem, new string[2] { tempId.ToString(), quantity.ToString() });
        }

        public void DungeonUseSkill(Guid knowledgeId, Guid enemyId)
        {
            SendRequest(Network.CommandCodes.Dungeon.UseSkill, new string[2] { knowledgeId.ToString(), enemyId.ToString() });
        }

        public void DungeonUseConsumable(Guid slotId, Guid enemyId)
        {
            SendRequest(Network.CommandCodes.Dungeon.UseConsumable, new string[2] { slotId.ToString(), enemyId.ToString() });
        }

        public void GetDungeonRoomLoots()
        {
            SendRequest(Network.CommandCodes.Dungeon.GetLoots);
        }

        public void LootDungeonItem(Guid lootId)
        {
            SendRequest(Network.CommandCodes.Dungeon.LootItem, new string[1] { lootId.ToString() });
        }

        public void ExitDungeon()
        {
            SendRequest(Network.CommandCodes.Dungeon.Exit);
        }

        public void LeaveFinishedRoom()
        {
            SendRequest(Network.CommandCodes.Dungeon.LeaveFinishedRoom);
        }

        #endregion

        private void SendRequest(int code, string[] args = null)
        {
            if (!_isGameRunning) return;

            var retMess = new Network.Message
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
                        MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, sockE.Message);
                    }
                }, null);

                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, "Connection lost, please reconnect..");
                }
            }
            catch (SocketException sockE)
            {
                MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, $"{sockE.StackTrace} : {sockE.Message}");
            }
            catch (NullReferenceException nullE)
            {
                MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, $"{nullE.StackTrace} : {nullE.Message}");
            }
            catch (ObjectDisposedException disposedE)
            {
                MessagingCenter.Send(this, MessengerCodes.GameServerDisconnected, $"{disposedE.StackTrace} : {disposedE.Message}");
            }
        }

        #endregion
    }
}
