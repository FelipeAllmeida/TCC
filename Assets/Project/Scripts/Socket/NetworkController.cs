using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : NetworkManager
{
    public static event Action onStartClient;
    public static event Action onStartServer;

    public static event Action onClientConnect;
    public static event Action onServerConnect;

    private static bool _isOn = false;
    public static bool isOn
    {
        get
        {
            return _isOn;
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect: " + conn.connectionId + " | " + conn.address);
        if (onClientConnect != null) onClientConnect();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect: " + conn.connectionId + " | " + conn.address);
        _isOn = true;
        if (onServerConnect != null) onServerConnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        _isOn = false;
    }

    public override void OnStartServer()
    {
        if (onStartServer != null) onStartServer();
    }

    public override void OnStartClient(NetworkClient client)
    {
        if (onStartClient != null) onStartClient();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        Debug.Log("OnServerAddPlayer");
    }
}
