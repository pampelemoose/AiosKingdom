using Newtonsoft.Json;
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
                case Network.CommandCodes.Client_Authenticate:
                    {
                        var authToken = JsonConvert.DeserializeObject<Guid>(message.Json);

                        if (!Guid.Empty.Equals(authToken))
                        {
                            _dispatchAuthToken = authToken;
                            MessagingCenter.Send(this, MessengerCodes.LoginSuccessful);
                        }
                        else
                        {
                            Disconnect();
                            MessagingCenter.Send(this, MessengerCodes.LoginFailed, "Credentials not matching any existing account.");
                        }
                    }
                    break;
                case Network.CommandCodes.Client_ServerList:
                    {
                        var servers = JsonConvert.DeserializeObject<List<Network.GameServerInfos>>(message.Json);
                        _serverListRequested = false;
                        MessagingCenter.Send(this, MessengerCodes.ServerListReceived, servers);
                    }
                    break;
                case Network.CommandCodes.Client_AnnounceGameConnection:
                    {
                        var connection = JsonConvert.DeserializeObject<Network.GameServerConnection>(message.Json);
                        MessagingCenter.Send(this, MessengerCodes.GameServerDatasReceived, connection);
                    }
                    break;
                default:
                    return false;
            }

            _dispatchTimedOut = DateTime.Now;

            return true;
        }

        public void ConnectToServer(string username, string password)
        {
            try
            {
                _dispatch = new TcpClient();
                var result = _dispatch.BeginConnect("127.0.0.1", 1337, (asyncResult) =>
                {
                    try
                    {
                        Task.Factory.StartNew(RunDispatch, TaskCreationOptions.LongRunning);
                        AskLogin(username, password);
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

        private void AskLogin(string username, string password)
        {
            var args = new string[2];
            args[0] = username;
            args[1] = DataModels.User.EncryptPassword(password);
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_Authenticate,
                Json = JsonConvert.SerializeObject(args)
            };
            SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
        }

        private bool _serverListRequested = false;
        public void AskServerInfos()
        {
            if (_serverListRequested) return;

            var args = new string[0];
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_ServerList,
                Json = JsonConvert.SerializeObject(args),
                Token = _dispatchAuthToken
            };
            SendJsonToDispatch(JsonConvert.SerializeObject(retMess));
            _serverListRequested = true;
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

        private void SendJsonToDispatch(string json)
        {
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
            if (_game.Connected)
            {
                _game.Client.Close();
                _game.Close();
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
        }

        private bool ProcessGameMessage(Network.Message message)
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
                        SendJsonToGame(JsonConvert.SerializeObject(retMess));
                    }
                    break;
                case Network.CommandCodes.Client_Authenticate:
                    {
                        var authToken = JsonConvert.DeserializeObject<Guid>(message.Json);

                        if (!Guid.Empty.Equals(authToken))
                        {
                            _gameAuthToken = authToken;
                            var args = new string[0];
                            var retMess = new Network.Message
                            {
                                Code = Network.CommandCodes.Client_SoulList,
                                Json = JsonConvert.SerializeObject(args),
                                Token = _gameAuthToken
                            };
                            SendJsonToGame(JsonConvert.SerializeObject(retMess));
                            //MessagingCenter.Send(this, MessengerCodes.LoginSuccessful);
                        }
                        else
                        {
                            //MessagingCenter.Send(this, MessengerCodes.LoginFailed, "Credentials not matching any existing account.");
                        }
                    }
                    break;
                case Network.CommandCodes.Client_CreateSoul:
                    {
                        if (message.Success)
                        {
                            var args = new string[0];
                            var retMess = new Network.Message
                            {
                                Code = Network.CommandCodes.Client_SoulList,
                                Json = JsonConvert.SerializeObject(args),
                                Token = _gameAuthToken
                            };
                            SendJsonToGame(JsonConvert.SerializeObject(retMess));
                        }
                        else
                        {
                            MessagingCenter.Send(this, MessengerCodes.SoulCreationFailed, message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Client_SoulList:
                    {
                        var souls = JsonConvert.DeserializeObject<List<DataModels.Soul>>(message.Json);
                        MessagingCenter.Send(this, MessengerCodes.SoulListReceived, souls);
                    }
                    break;
                case Network.CommandCodes.Client_ConnectSoul:
                    {
                        if (message.Success)
                        {
                            MessagingCenter.Send(this, MessengerCodes.SoulConnected);
                            AskArmorList();
                            AskConsumableList();
                            AskBagList();
                            AskBookList();
                            AskMonsterList();
                            AskDungeonList();
                        }
                        else
                        {
                            MessagingCenter.Send(this, MessengerCodes.SoulConnectionFailed, message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Client_SoulDatas:
                    {
                        var soul = JsonConvert.DeserializeObject<DataModels.Soul>(message.Json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                        DatasManager.Instance.Soul = soul;
                        MessagingCenter.Send(this, MessengerCodes.SoulUpdated);
                    }
                    break;
                case Network.CommandCodes.Client_CurrentSoulDatas:
                    {
                        var soulDatas = JsonConvert.DeserializeObject<Network.SoulDatas>(message.Json);
                        DatasManager.Instance.Datas = soulDatas;
                        MessagingCenter.Send(this, MessengerCodes.SoulDatasUpdated);
                    }
                    break;
                case Network.CommandCodes.Client_MarketList:
                    {
                        var items = JsonConvert.DeserializeObject<List<DataModels.MarketSlot>>(message.Json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                        DatasManager.Instance.MarketItems = items;
                        MessagingCenter.Send(this, MessengerCodes.MarketUpdated);
                    }
                    break;
                case Network.CommandCodes.Client_BuyMarketItem:
                    {
                        if (message.Success)
                        {
                            AskSoulDatas();
                            AskMarketItems();
                        }
                    }
                    break;
                case Network.CommandCodes.Client_EquipItem:
                    {
                        if (message.Success)
                        {
                            AskSoulDatas();
                            AskSoulCurrentDatas();
                        }
                    }
                    break;
                case Network.CommandCodes.Client_UseSpiritPills:
                    {
                        if (message.Success)
                        {
                            AskSoulDatas();
                            AskSoulCurrentDatas();
                        }
                    }
                    break;
                case Network.CommandCodes.Client_LearnSkill:
                    {
                        if (message.Success)
                        {
                            AskSoulDatas();
                        }
                        MessagingCenter.Send(this, MessengerCodes.SkillLearned, message.Json);
                    }
                    break;

                case Network.CommandCodes.ArmorList:
                    {
                        var armors = JsonConvert.DeserializeObject<List<DataModels.Items.Armor>>(message.Json);
                        DatasManager.Instance.Armors = armors;
                    }
                    break;
                case Network.CommandCodes.ConsumableList:
                    {
                        var consumable = JsonConvert.DeserializeObject<List<DataModels.Items.Consumable>>(message.Json);
                        DatasManager.Instance.Consumables = consumable;
                    }
                    break;
                case Network.CommandCodes.BagList:
                    {
                        var bags = JsonConvert.DeserializeObject<List<DataModels.Items.Bag>>(message.Json);
                        DatasManager.Instance.Bags = bags;
                    }
                    break;
                case Network.CommandCodes.BookList:
                    {
                        var books = JsonConvert.DeserializeObject<List<DataModels.Skills.Book>>(message.Json);
                        DatasManager.Instance.Books = books;
                    }
                    break;
                case Network.CommandCodes.MonsterList:
                    {
                        var monsters = JsonConvert.DeserializeObject<List<DataModels.Monsters.Monster>>(message.Json);
                        DatasManager.Instance.Monsters = monsters;
                    }
                    break;
                case Network.CommandCodes.DungeonList:
                    {
                        var dungeons = JsonConvert.DeserializeObject<List<DataModels.Dungeons.Dungeon>>(message.Json);
                        DatasManager.Instance.Dungeons = dungeons;
                    }
                    break;

                case Network.CommandCodes.Dungeon_EnterRoom:
                    {
                        if (message.Success)
                        {
                            MessagingCenter.Send(this, MessengerCodes.EnterDungeon);
                        }
                        else
                        {
                            MessagingCenter.Send(this, MessengerCodes.EnterDungeonFailed, message.Json);
                        }
                    }
                    break;
                case Network.CommandCodes.Dungeon_Exit:
                    {
                        if (message.Success)
                        {
                            MessagingCenter.Send(this, MessengerCodes.SoulConnected);
                        }
                    }
                    break;
                case Network.CommandCodes.Dungeon_UpdateRoom:
                    {
                        if (message.Success)
                        {
                            var adventure = JsonConvert.DeserializeObject<Network.AdventureState>(message.Json);
                            DatasManager.Instance.Adventure = adventure;
                            MessagingCenter.Send(this, MessengerCodes.DungeonUpdated);
                        }
                    }
                    break;
                case Network.CommandCodes.Dungeon_UseSkill:
                    {
                        if (message.Success)
                        {
                            UpdateDungeonRoom();
                        }
                    }
                    break;

                default:
                    return false;
            }

            _gameTimedOut = DateTime.Now;

            return true;
        }

        public void AskArmorList()
        {
            SendRequest(Network.CommandCodes.ArmorList);
        }

        public void AskConsumableList()
        {
            SendRequest(Network.CommandCodes.ConsumableList);
        }

        public void AskBagList()
        {
            SendRequest(Network.CommandCodes.BagList);
        }

        public void AskBookList()
        {
            SendRequest(Network.CommandCodes.BookList);
        }

        public void AskMonsterList()
        {
            SendRequest(Network.CommandCodes.MonsterList);
        }

        public void AskDungeonList()
        {
            SendRequest(Network.CommandCodes.DungeonList);
        }

        public void CreateSoul(string soulname)
        {
            SendRequest(Network.CommandCodes.Client_CreateSoul, new string[1] { soulname });
        }

        public void ConnectSoul(Guid id)
        {
            SendRequest(Network.CommandCodes.Client_ConnectSoul, new string[1] { id.ToString() });
        }

        public void AskSoulDatas()
        {
            SendRequest(Network.CommandCodes.Client_SoulDatas);
        }

        public void AskSoulCurrentDatas()
        {
            SendRequest(Network.CommandCodes.Client_CurrentSoulDatas);
        }

        public void AskMarketItems()
        {
            SendRequest(Network.CommandCodes.Client_MarketList);
        }

        public void BuyMarketItem(Guid slotId)
        {
            SendRequest(Network.CommandCodes.Client_BuyMarketItem, new string[1] { slotId.ToString() });
        }

        public void EquipItem(Guid slotId)
        {
            SendRequest(Network.CommandCodes.Client_EquipItem, new string[1] { slotId.ToString() });
        }

        public void UseSpiritPills(DataModels.Soul.Stats statType, int quantity)
        {
            SendRequest(Network.CommandCodes.Client_UseSpiritPills, new string[2] { statType.ToString(), quantity.ToString() });
        }

        public void LearnSkill(Guid bookId, int rank)
        {
            SendRequest(Network.CommandCodes.Client_LearnSkill, new string[2] { bookId.ToString(), rank.ToString() });
        }

        public void OpenDungeonRoom(Guid dungeonId)
        {
            SendRequest(Network.CommandCodes.Dungeon_EnterRoom, new string[1] { dungeonId.ToString() });
        }

        public void UpdateDungeonRoom()
        {
            SendRequest(Network.CommandCodes.Dungeon_UpdateRoom);
        }

        public void DungeonUseSkill(Guid knowledgeId, Guid enemyId)
        {
            SendRequest(Network.CommandCodes.Dungeon_UseSkill, new string[2] { knowledgeId.ToString(), enemyId.ToString() });
        }

        public void ExitDungeon()
        {
            SendRequest(Network.CommandCodes.Dungeon_Exit);
        }

        private void SendRequest(int code, string[] args = null)
        {
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
        }

        #endregion
    }
}
