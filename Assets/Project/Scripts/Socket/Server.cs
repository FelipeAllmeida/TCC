using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    #region Singleton
    private static Server _instance;
    public static Server instance
    {
        get
        {
            if (_instance == null)
            {
                return InstanceInitialize();
            }
            else
            {
                return _instance;
            }
        }
    }

    private static Server InstanceInitialize()
    {
        GameObject __poolManagerObject = (GameObject)Resources.Load("Server");

        _instance = __poolManagerObject.GetComponent<Server>();
        _instance.Initialize();

        return _instance;
    }
    #endregion

    #region Private Data
    private SocketController _socketController;
    private SocketConnector _socketConnector;

    private static bool _isMaster = false;
    public static bool isMaster
    {
    get
        {
            return _isMaster;
        }
    }
    #endregion

    public void Initialize()
    {
        _socketConnector.AInitialize();
    }

    public void InitializeServer(SocketInitializationData p_socketInitializationData, Action p_callbackFinish = null)
    {
        _socketController = new SocketController();
        _socketController.Initialize(p_socketInitializationData);
        _isMaster = true;
        StartWaitAllClientStream();
        if (p_callbackFinish != null) p_callbackFinish();
        
    }

    #region Connection
    public void TryToConnectToSocket(string p_ipAddress, int p_port, Action p_callbackSuccess, Action p_callbackFailed)
    {
        _socketConnector.TryToConnectToSocket(p_ipAddress, p_port, p_callbackSuccess, p_callbackFailed);
    }
    #endregion

    #region Stream Flow
    public void SendDataStreamToServer(string p_stream)
    {
        if (_isMaster == true)
        {
            _socketController.SetMasterClientStream(p_stream);
        }
        else
        {
            _socketConnector.SendData(p_stream);
        }
    }

    private void StartWaitAllClientStream()
    {        
        _socketController.StartWaitAllClientsStreamThread();
    }

    private void StopWaitAllClientStream()
    {
        _socketController.StopWaitAllClientsStreamThread();
    }
    #endregion
}
