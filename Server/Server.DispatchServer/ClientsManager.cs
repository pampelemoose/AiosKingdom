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

        private Dictionary<Guid, Guid> _userIds;
        private Dictionary<Guid, DateTime> _pings;

        private ClientsManager()
        {
            _clients = new Dictionary<Guid, Socket>();
            _authTokens = new Dictionary<Guid, Guid>();

            _userIds = new Dictionary<Guid, Guid>();
            _pings = new Dictionary<Guid, DateTime>();
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

                if (_pings.ContainsKey(id))
                {
                    _pings.Remove(id);
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
            _userIds.Clear();
            _pings.Clear();
        }

        public void AuthenticateClient(Guid clientId, Guid token)
        {
            _authTokens.Add(clientId, token);
        }

        public void SetUserId(Guid token, Guid id)
        {
            _userIds.Add(token, id);
        }

        public Guid GetUserId(Guid token)
        {
            if (_userIds.ContainsKey(token))
            {
                return _userIds[token];
            }

            return Guid.Empty;
        }

        public void RemoveUserId(Guid token)
        {
            if (_userIds.ContainsKey(token))
            {
                _userIds.Remove(token);
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

        public void Ping(Guid clientId)
        {
            if (_pings.ContainsKey(clientId))
            {
                _pings[clientId] = DateTime.Now;
            }
            else
            {
                _pings.Add(clientId, DateTime.Now);
            }
        }

        public void Timeout(Guid clientId)
        {
            if (_pings.ContainsKey(clientId))
            {
                _pings[clientId] = DateTime.Now + TimeSpan.FromHours(1);
            }
            else
            {
                _pings.Add(clientId, DateTime.Now + TimeSpan.FromHours(1));
            }
        }

        public double GetPing(Guid clientId)
        {
            if (_pings.ContainsKey(clientId))
            {
                var diff = DateTime.Now - _pings[clientId];
                return diff.TotalSeconds;
            }

            return 0.0f;
        }
    }
}
