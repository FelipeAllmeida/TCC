using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server 
{
    #region Private Data
    private SocketController _socketController;
    #endregion
    public void InitializeServer(SocketInitializationData p_socketInitializationData, Action p_callbackFinish = null)
    {
        _socketController.Initialize(p_socketInitializationData);
    }
}
