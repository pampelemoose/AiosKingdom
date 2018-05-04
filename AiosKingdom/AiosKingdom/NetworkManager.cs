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
        private Guid _dispatchAuthToken;

        private TcpClient _game;
        private bool _isGameRunning;
        private Guid _gameAuthToken;

        private NetworkManager()
        {
        }

        #region Dispatch Functions

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
                            _dispatchAuthToken = authToken;
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
                case Network.CommandCodes.Client_AnnounceGameConnection:
                    {
                        var connection = JsonConvert.DeserializeObject<Network.GameServerConnection>(message.Json);
                        MessagingCenter.Send(this, MessengerCodes.GameServerDatasReceived, connection);
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }

        public void ConnectToServer()
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

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                if (_serverListRequested)
                {
                    _serverListRequested = false;
                    AskServerInfos();
                }
                return false;
            });
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

        #endregion

        #region Game Function

        public void ConnectToGameServer(Network.GameServerConnection connection)
        {
            try
            {
                _game = new TcpClient();
                var result = _game.BeginConnect(/*connection.Host*/"10.0.2.2", connection.Port, (asyncResult) =>
                {
                    try
                    {
                        _isGameRunning = true;
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

        private void RunGame()
        {
            while (_isGameRunning)
            {
                var bufferSize = _game.Available;

                if (bufferSize > 0)
                {
                    var client = _game.Client;

                    byte[] buffer = new byte[bufferSize];
                    client.Receive(buffer);

                    var bufferStr = Encoding.UTF8.GetString(buffer);
                    var messages = bufferStr.Split('|');
                    foreach (var message in messages)
                    {
                        if (string.IsNullOrEmpty(message))
                            continue;

                        var fromJson = JsonConvert.DeserializeObject<Network.Message>(message);

                        if (!ProcessGameMessage(fromJson))
                        {
                            /*_message = buffer;
                            NotifyPropertyChanged(nameof(Message));*/
                        }
                    }
                }
            }

            _game.Client.Close();
            _game.Close();
        }

        private bool ProcessGameMessage(Network.Message message)
        {
            switch (message.Code)
            {
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
                        var souls = JsonConvert.DeserializeObject<List<DataModels.Soul>>(message.Json);
                        DatasManager.Instance.Souls = souls;
                        MessagingCenter.Send(this, MessengerCodes.SoulListReceived);
                    }
                    break;
                case Network.CommandCodes.Client_SoulList:
                    {
                        var souls = JsonConvert.DeserializeObject<List<DataModels.Soul>>(message.Json);
                        DatasManager.Instance.Souls = souls;
                        MessagingCenter.Send(this, MessengerCodes.SoulListReceived);
                    }
                    break;
                default:
                    return false;
            }

            return true;
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
                        //MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, sockE.Message);
                    }
                }, null);

                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    //MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, "Connection lost, please reconnect..");
                }
            }
            catch (SocketException sockE)
            {
                //MessagingCenter.Send(this, MessengerCodes.ConnectionFailed, sockE.Message);
            }
        }

        #endregion
    }
}
