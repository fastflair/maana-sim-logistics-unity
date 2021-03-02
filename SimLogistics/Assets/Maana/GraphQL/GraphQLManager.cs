using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Maana.GraphQL
{
    public sealed class GraphQLManager : MonoBehaviour
    {
        public UnityEvent onConnected = new UnityEvent();
        public UnityEvent onDisconnected = new UnityEvent();
        public UnityEvent<string> onConnectionError = new UnityEvent<string>();
        
        private GraphQLClient _client;
        private OAuthFetcher _fetcher;
        private readonly List<QueuedQuery> _queuedQueries = new List<QueuedQuery>();

        private bool HasToken => _fetcher is {Token: { }};

        public void Connect(string url, string authDomain, string authClientId, string authClientSecret,
            string authIdentifier,
            float refreshMinutes = 20f)
        {
            if (string.IsNullOrEmpty(url)) throw new Exception("No URL specified for GraphQL Manager");

            lock (_queuedQueries)
            {
                _queuedQueries.Clear();
            }

            if (_client != null)
            {
                onDisconnected.Invoke();
            }
            
            _client = new GraphQLClient(url);
            _fetcher = new OAuthFetcher(authDomain, authClientId, authClientSecret, authIdentifier, refreshMinutes);
            _fetcher.TokenReceivedEvent.AddListener(TokenReceived);
            _fetcher.TokenErrorEvent.AddListener(TokenError);
        }

        private void TokenReceived()
        {
            onConnected.Invoke();
            
            lock (_queuedQueries)
            {
                foreach (var queuedQuery in _queuedQueries)
                {
                    Query(queuedQuery.Query, queuedQuery.Variables, queuedQuery.Callback);
                }

                _queuedQueries.Clear();
            }
        }

        private void TokenError(string error)
        {
            onConnectionError.Invoke(error);
        }

        public void Query(string query, object variables = null, Action<GraphQLResponse> callback = null)
        {
            if (HasToken)
                _client.Query(query, variables, _fetcher.Token.access_token, callback);
            else
                lock (_queuedQueries)
                {
                    _queuedQueries.Add(new QueuedQuery(query, variables, callback));
                }
        }

        private class QueuedQuery
        {
            public readonly Action<GraphQLResponse> Callback;
            public readonly string Query;
            public readonly object Variables;

            public QueuedQuery(string query, object variables = null, Action<GraphQLResponse> callback = null)
            {
                Query = query;
                Variables = variables;
                Callback = callback;
            }
        }
    }
}