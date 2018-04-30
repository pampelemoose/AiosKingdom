using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        private Guid _authToken;

        private NetworkManager()
        {
        }

        private void RunDispatch()
        {
            while (_isDispatchRunning)
            {
                var bufferSize = _dispatch.Available;

                if (bufferSize > 0)
                {
                    var client = _dispatch.Client;

                    byte[] buffer = new byte[bufferSize];
                    client.Receive(buffer);

                    var bufferStr = Encoding.UTF8.GetString(buffer);
                    var messages = bufferStr.Split('|');
                    foreach (var message in messages)
                    {
                        if (string.IsNullOrEmpty(message))
                            continue;

                        var fromJson = JsonConvert.DeserializeObject<Network.Message>(message);

                        if (!ProcessDispatchMessage(fromJson))
                        {
                            /*_message = buffer;
                            NotifyPropertyChanged(nameof(Message));*/
                        }
                    }
                }
            }

            _dispatch.Client.Close();
            _dispatch.Close();
        }

        private bool ProcessDispatchMessage(Network.Message message)
        {
            switch (message.Code)
            {
                case Network.CommandCodes.Client_Authenticate:
                    {
                        var authToken = JsonConvert.DeserializeObject<Guid>(message.Json);

                        if (!Guid.Empty.Equals(authToken))
                        {
                            _authToken = authToken;
                            MessagingCenter.Send(this, MessengerCodes.LoginSuccessful);
                        }
                        else
                        {
                            MessagingCenter.Send(this, MessengerCodes.LoginFailed, "Credentials not matching any existing account.");
                        }
                    }
                    break;
                case Network.CommandCodes.Client_ServerList:
                    {
                        var servers = JsonConvert.DeserializeObject<List<Network.GameServerInfos>>(message.Json);
                        DatasManager.Instance.ServerInfos = servers;
                        _serverListRequested = false;
                        MessagingCenter.Send(this, MessengerCodes.InitialDatasReceived);
                    }
                    break;
                /*case Network.ClientCodes.Ping:
                    {
                        _timedOut = DateTime.Now.Ticks;
                        _ping = JsonConvert.DeserializeObject<int>(message.Json);
                        var retMess = new Network.Message
                        {
                            Code = Network.ServerCodes.Pong
                        };
                        _dispatch.Send(JsonConvert.SerializeObject(retMess));
                        PingChanged?.Invoke();
                    }
                    break;
                case Network.ClientCodes.Armor_List:
                    {
                        var datas = JsonConvert.DeserializeObject<List<DataDtos.ArmorDto>>(message.Json);
                        DatasManager.Instance.Armors = datas;
                    }
                    break;
                case Network.ClientCodes.Consumable_List:
                    {
                        var datas = JsonConvert.DeserializeObject<List<DataDtos.ConsumableDto>>(message.Json);
                        DatasManager.Instance.Consumables = datas;
                    }
                    break;
                case Network.ClientCodes.Book_List:
                    {
                        var datas = JsonConvert.DeserializeObject<List<DataModels.Skills.Book>>(message.Json);
                        DatasManager.Instance.Books = datas;
                    }
                    break;
                case Network.ClientCodes.User_Infos:
                    {
                        var soulList = JsonConvert.DeserializeObject<List<DataModels.Soul>>(message.Json);
                        DatasManager.Instance.Souls = soulList;
                        _soulListRequested = false;
                    }
                    break;
                case Network.ClientCodes.Player_Connected:
                    {
                        var soul = JsonConvert.DeserializeObject<DataDtos.SoulDto>(message.Json);
                        DatasManager.Instance.CurrentSoul = soul.Soul;
                        DatasManager.Instance.Equipment = soul.Equipment;
                        DatasManager.Instance.Inventories = soul.Inventories;
                    }
                    break;
                case Network.ClientCodes.Player_Disconnected:
                    {
                        DatasManager.Instance.CurrentSoul = null;
                    }
                    break;
                case Network.ClientCodes.Player_CurrentDatas:
                    {
                        var datas = JsonConvert.DeserializeObject<Network.SoulDatas>(message.Json);
                        DatasManager.Instance.CurrentDatas = datas;
                        _waitingCurrentSoulDatas = false;
                    }
                    break;
                case Network.ClientCodes.Player_Equipment:
                    {
                        var equipment = JsonConvert.DeserializeObject<DataModels.Equipment>(message.Json);
                        DatasManager.Instance.Equipment = equipment;
                    }
                    break;
                case Network.ClientCodes.Player_Inventory:
                    {
                        var inventory = JsonConvert.DeserializeObject<List<DataModels.InventorySlot>>(message.Json);
                        DatasManager.Instance.Inventories = inventory;
                    }
                    break;
                case Network.ClientCodes.Market_Items_List:
                    {
                        var items = JsonConvert.DeserializeObject<List<DataModels.MarketSlot>>(message.Json);
                        DatasManager.Instance.MarketItems = items;
                        _waitingMarketItems = false;
                    }
                    break;
                case Network.ClientCodes.Market_Item_Bought:
                    {
                        var bought = JsonConvert.DeserializeObject<bool>(message.Json);
                        if (bought)
                        {
                            AskMarketItems(DatasManager.Instance.MarketItems.First().Type);
                            AskInventory();
                            AskCurrentSoulDatas();
                        }
                        _buyingMarketItem = false;
                    }
                    break;*/
                default:
                    return false;
            }

            return true;
        }

        #region Dispatch Functions

        public async void ConnectToServer()
        {
            try
            {
                _dispatch = new TcpClient();
                var result = _dispatch.BeginConnect("10.0.2.2", 1337, (asyncResult) =>
                {
                    try
                    {
                        _isDispatchRunning = true;
                        Task.Factory.StartNew(RunDispatch, TaskCreationOptions.LongRunning);
                        MessagingCenter.Send(this, MessengerCodes.ConnectionSuccessful);
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

        public void AskLogin(string username, string password)
        {
            var args = new string[2];
            args[0] = username;
            args[1] = DataModels.User.EncryptPassword(password);
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_Authenticate,
                Json = JsonConvert.SerializeObject(args)
            };
            SendJson(JsonConvert.SerializeObject(retMess));
        }

        private bool _serverListRequested = false;
        public void AskServerInfos(bool reset = true)
        {
            if (_serverListRequested) return;

            var args = new string[0];
            var retMess = new Network.Message
            {
                Code = Network.CommandCodes.Client_ServerList,
                Json = JsonConvert.SerializeObject(args),
                Token = _authToken
            };
            SendJson(JsonConvert.SerializeObject(retMess));
            _serverListRequested = true;
        }

        #endregion

        private void SendJson(string json)
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
                        MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, sockE.Message);
                    }
                }, null);

                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, "Connection lost, please reconnect..");
                }
            }
            catch (SocketException sockE)
            {
                MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, sockE.Message);
            }
        }
    }
}
