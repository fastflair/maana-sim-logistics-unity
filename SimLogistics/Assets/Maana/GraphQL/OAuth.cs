using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

// ReSharper disable InconsistentNaming

namespace Maana.GraphQL
{
    [Serializable]
    public class OAuthToken
    {
        [UsedImplicitly] public string access_token;
        [UsedImplicitly] public string token_type;
        [UsedImplicitly] public string session_state;
        [UsedImplicitly] public string scope;
    }

    public class OAuthFetcher
    {
        private readonly string authDomain;
        private readonly string authClientId;
        private readonly string authClientSecret;
        private readonly string authIdentifier;
        private readonly float refreshMinutes;

        public OAuthFetcher(string authDomain, string authClientId, string authClientSecret, string authIdentifier,
            float refreshMinutes = 20f)
        {
            this.authDomain = authDomain;
            this.authClientId = authClientId;
            this.authClientSecret = authClientSecret;
            this.authIdentifier = authIdentifier;
            this.refreshMinutes = refreshMinutes;
            GetOAuthToken();
        }

        public OAuthToken Token { get; private set; }

        public UnityEvent TokenReceivedEvent { get; } = new UnityEvent();
        public UnityEvent<string> TokenErrorEvent { get; } = new UnityEvent<string>();

        private string StripCredentials(string str)
        {
            return str
                .Replace(authIdentifier, "<chunk redacted>")
                .Replace(authClientSecret, "<chunk redacted>");
        }

        private void BeginTokenUpdater()
        {
            Coroutiner.StartCoroutine(UpdateToken());
        }

        private IEnumerator UpdateToken()
        {
            yield return new WaitForSeconds(refreshMinutes * 60f);
            GetOAuthToken();
        }

        private UnityWebRequest TokenRequest()
        {
            // TODO: Auth0?
            var url = $"https://{authDomain}/auth/realms/{authIdentifier}/protocol/openid-connect/token";

            try
            {
                var formData = new WWWForm();

                formData.AddField("client_id", authClientId);
                formData.AddField("client_secret", authClientSecret);
                formData.AddField("grant_type", "client_credentials");
                formData.AddField("audience", authIdentifier);

                var request = UnityWebRequest.Post(url, formData);

                request.SetRequestHeader("Accept", "application/json");
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

                return request;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                TokenErrorEvent.Invoke($"{url}{Environment.NewLine}{StripCredentials(ex.Message)}");
                return null;
            }
        }

        private IEnumerator SendRequest()
        {
            var request = TokenRequest();
            if (request == null) yield break;
            
            using (var www = request)
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    TokenErrorEvent.Invoke($"{request.url}{Environment.NewLine}{www.error}");
                    yield break;
                }

                Token = JsonUtility.FromJson<OAuthToken>(www.downloadHandler.text);

                TokenReceivedEvent.Invoke();
            }

            request.Dispose();

            BeginTokenUpdater();
        }

        private void GetOAuthToken()
        {
            Coroutiner.StartCoroutine(SendRequest());
        }
    }
}