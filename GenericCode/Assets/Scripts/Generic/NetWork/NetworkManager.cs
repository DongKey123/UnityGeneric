using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkManager : LazySingleton<NetworkManager>
{
    public string Scheme { get { return scheme; } }
    public string IP { get { return ip; } }
    public int Port { get { return port; } }


    private string token = string.Empty;
    private const string scheme = "http";
    private const string ip = "localhost";
    private const int port = 8080;

    private const double timeout = 5;

    public string AccessToken = string.Empty;
    public string RefreshToken = string.Empty;

    private const string GET = "GET";
    private const string POST = "POST";
    private const string PUT = "PUT";
    private const string DELETE = "DELETE";
}
