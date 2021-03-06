using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace Maana.GraphQL
{
    public class GraphQLClient
    {
        private readonly string _url;

        public GraphQLClient(string url)
        {
            this._url = url;
        }

        private class GraphQLQuery
        {
            // ReSharper disable once InconsistentNaming
            [UsedImplicitly] public string query;
            // ReSharper disable once InconsistentNaming
            [UsedImplicitly] public object variables;
        }

        public string CkglUrl(string endpoint)
        {
            return $"{_url}/service/{endpoint}/graphql";
        }
        
        private UnityWebRequest QueryRequest(string endpoint, string query, object variables, string token = null)
        {
            var fullQuery = new GraphQLQuery()
            {
                query = query,
                variables = variables,
            };

            var json = JsonConvert.SerializeObject(fullQuery);
            
            var request = UnityWebRequest.Post(CkglUrl(endpoint), UnityWebRequest.kHttpVerbPOST);

            var payload = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(payload);
            request.SetRequestHeader("Content-Type", "application/json");
            if(token != null) request.SetRequestHeader("Authorization", "Bearer " + token);

            return request;
        }

        private IEnumerator SendRequest(
            string endpoint,
            string query,
            object variables = null,
            Action<GraphQLResponse> callback = null,
            string token = null)
        {
            var request = QueryRequest(endpoint, query, variables, token);

            using (var www = request)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                yield return www.SendWebRequest();

                stopwatch.Stop();
                Debug.Log($"[GraphQL] {stopwatch.Elapsed}");

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    callback?.Invoke(new GraphQLResponse("", www.error));
                    yield break;
                }

                var responseString = www.downloadHandler.text;

                var result = new GraphQLResponse(responseString);

                callback?.Invoke(result);
            }

            request.Dispose();
        }

        public void Query(
            string endpoint,
            string query,
            object variables = null,
            string sToken = null,
            Action<GraphQLResponse> callback = null)
        {
            Debug.Log($"GraphQL Query: {endpoint} {query} {variables}");
            Coroutiner.StartCoroutine(SendRequest(endpoint, query, variables, callback, sToken));
        }
    }
}