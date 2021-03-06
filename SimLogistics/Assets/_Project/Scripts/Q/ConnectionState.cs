using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConnectionState
{
    public string id;
    public string url;
    public string apiEndpoint;
    public string authDomain;
    public string authClientId;
    public string authClientSecret;
    public string authIdentifier;
    public float refreshMinutes;
}