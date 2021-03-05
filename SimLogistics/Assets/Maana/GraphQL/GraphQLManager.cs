using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Maana.GraphQL
{
    public sealed class GraphQLManager : MonoBehaviour
    {
        public UnityEvent connectionReadyEvent = new UnityEvent();
        public UnityEvent connectionNotReadyEvent = new UnityEvent();
        public UnityEvent<string> connectionErrorEvent = new UnityEvent<string>();
        
        private GraphQLClient _client;
        private OAuthFetcher _fetcher;
        private readonly List<QueuedQuery> _queuedQueries = new List<QueuedQuery>();

        private bool HasToken => _fetcher is {Token: { }};
        private bool _isConnected;
        
        // Can be called multiple times
        public void Connect(string url, string authDomain, string authClientId, string authClientSecret,
            string authIdentifier,
            float refreshMinutes = 20f)
        {
            if (string.IsNullOrEmpty(url)) throw new Exception("No URL specified for GraphQL Manager");

            lock (_queuedQueries)
            {
                _queuedQueries.Clear();
            }

            _isConnected = false;
            if (_client != null)
            {
                connectionNotReadyEvent.Invoke();
            }

            if (refreshMinutes == 0f) refreshMinutes = 20f;
            
            _client = new GraphQLClient(url);
            _fetcher = new OAuthFetcher(authDomain, authClientId, authClientSecret, authIdentifier, refreshMinutes);
            _fetcher.TokenReceivedEvent.AddListener(TokenReceived);
            _fetcher.TokenErrorEvent.AddListener(TokenError);
        }

        private void TokenReceived()
        {
            // First-time connection or refresh?
            if (!_isConnected)
            {
                // First-time connection or refresh?
                _isConnected = true;
                connectionReadyEvent.Invoke();
            }

            lock (_queuedQueries)
            {
                foreach (var queuedQuery in _queuedQueries)
                {
                    // Query(queuedQuery.Query, queuedQuery.Variables, queuedQuery.Callback);
                    _client.Query(queuedQuery.Query, queuedQuery.Variables, _fetcher.Token.access_token, queuedQuery.Callback);
                }

                _queuedQueries.Clear();
            }
        }

        private void TokenError(string error)
        {
            _isConnected = false;
            connectionErrorEvent.Invoke(error);
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