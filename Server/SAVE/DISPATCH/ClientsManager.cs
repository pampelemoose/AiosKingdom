using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Server.DispatchServer
{
    public class ClientsManager
    {
        private static ClientsManager _instance;
        public static ClientsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClientsManager();
                }

                return _instance;
            }
        }

        private Dictionary<Guid, Socket> _clients;
        private Dictionary<Guid, Guid> _authTokens;

        private Dictionary<Guid, Guid> _authIds;

        private ClientsManager()
        {
            _clients = new Dictionary<Guid, Socket>();
            _authTokens = new Dictionary<Guid, Guid>();

            _authIds = new Dictionary<Guid, Guid>();
        }

        public Dictionary<Guid, Socket> Clients => _clients;

        public void AddClient(Guid id, Socket socket)
        {
            _clients.Add(id, socket);
        }

        public bool RemoveClient(Guid id)
        {
            if (_clients.ContainsKey(id))
            {
                _clients.Remove(id);

                if (_authTokens.ContainsKey(id))
                {
                    _authTokens.Remove(id);
                }
                return true;
            }

            return false;
        }

        public Socket GetById(Guid id)
        {
            var client = _clients.FirstOrDefault(c => c.Key.Equals(id)).Value;

            if (client != null)
                return client;

            return null;
        }

        public void Clear()
        {
            _clients.Clear();
            _authTokens.Clear();
            _authIds.Clear();
        }

        public void AuthenticateClient(Guid clientId, Guid token)
        {
            _authTokens.Add(clientId, token);
        }

        public void SetAuthId(Guid token, Guid id)
        {
            _authIds.Add(token, id);
        }

        public Guid GetAuthId(Guid token)
        {
            if (_authIds.ContainsKey(token))
            {
                return _authIds[token];
            }

            return Guid.Empty;
        }

        public void RemoveAuthId(Guid token)
        {
            if (_authIds.ContainsKey(token))
            {
                _authIds.Remove(token);
            }
        }

        public bool IsAuth(Guid clientId, Guid unknown)
        {
            if (_authTokens.ContainsKey(clientId))
            {
                var token = _authTokens[clientId];

                return token.Equals(unknown);
            }

            return false;
        }
    }
}
