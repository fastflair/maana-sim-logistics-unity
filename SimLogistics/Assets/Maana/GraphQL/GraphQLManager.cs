using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maana.GraphQL
{
    public sealed class GraphQLManager : MonoBehaviour
    {
        private GraphQLClient _client;
        private OAuthFetcher _fetcher;
        private List<QueuedQuery> _queuedQueries = new List<QueuedQuery>();

        private bool HasToken => _fetcher is {Token: { }};

        public void Connect(string url, string authDomain, string authClientId, string authClientSecret,
            string authIdentifier,
            float refreshMinutes = 20f)
        {
            if (string.IsNullOrEmpty(url)) throw new Exception("No URL specified for GraphQL Manager");

            _client = new GraphQLClient(url);
            _fetcher = new OAuthFetcher(authDomain, authClientId, authClientSecret, authIdentifier, refreshMinutes);
            _fetcher.TokenReceivedEvent.AddListener(TokenReceived);
        }

        private void TokenReceived()
        {
            foreach (var queuedQuery in _queuedQueries)
                Query(queuedQuery.Query, queuedQuery.Variables, queuedQuery.Callback);

            _queuedQueries = new List<QueuedQuery>();
        }

        public void Query(string query, object variables = null, Action<GraphQLResponse> callback = null)
        {
            if (HasToken)
                _client.Query(query, variables, _fetcher.Token.access_token, callback);
            else
                _queuedQueries.Add(new QueuedQuery(query, variables, callback));
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