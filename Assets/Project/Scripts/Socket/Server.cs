using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Linq;
using Framework;

#region Enumerators
internal enum ServerState
{
    NONE,
    LOBBY,
    GAME
}
#endregion

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

    #region Events
    public event Action<string> onSocketResponse;
    #endregion

    #region Private Data
    private SocketController _socketController;
    private SocketConnector _socketConnector;

    private ServerState _currentServerState =  ServerState.NONE;

    private ServerData _serverData;

    private float _responseTreatTime = 0.01f;

    private TimerNodule _treatResponseNodule;

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
        _socketConnector = new SocketConnector();
        _socketConnector.AInitialize();
        _socketConnector.StartReadSocketDataThread();
        TreatResponseRecursion();
        ListenSocketConnectorEvents();
    }

    private void ListenSocketConnectorEvents()
    {
        _socketConnector.onSocketResponse += delegate (string p_value)
        {
            Debug.Log("Reciving: " + p_value);
            if (onSocketResponse != null)
                onSocketResponse(p_value);
        };
    }


    #region Update Methods
    private void TreatResponseRecursion()
    {
        if (_treatResponseNodule != null) _treatResponseNodule.Stop();
        _treatResponseNodule = Timer.WaitSeconds(_responseTreatTime, delegate
        {
            TreatClientStreams();
            TreatResponseRecursion();
        });
    }

    private void StopTreatResponseRecursion()
    {
        if (_treatResponseNodule != null) _treatResponseNodule.Stop();
    }
    #endregion

    #region Intialization Methods
    public void InitializeServer(SocketInitializationData p_socketInitializationData, Action p_callbackFinish = null)
    {
        _socketController = new SocketController();
        _serverData = new ServerData();
        _socketController.Initialize(p_socketInitializationData);
        _isMaster = true;
        _currentServerState = ServerState.LOBBY;

        _socketController.onSocketResponseToMasterClient += delegate (string p_value)
        {
            if (onSocketResponse != null)
                onSocketResponse(p_value);
        };
        _socketController.StartSocket(delegate
        {
            Debug.Log("Socket started");
            _socketController.StartAcceptTcpClientThread(delegate
            {
                Debug.Log("All clients connected");
                StartWaitAllClientStream();
            });
        }, null);

        if (p_callbackFinish != null) p_callbackFinish();
        
    }
    #endregion

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

    #region Stream Handler
    private void TreatClientStreams()
    {
        if (_currentServerState == ServerState.NONE || _socketController == null)
            return;

        List<string> __listKeys = _socketController.dictClients.Keys.ToList<string>();
        for (int i = 0; i < __listKeys.Count;i++)
        {
            SocketController.ClientData __client = _socketController.dictClients[__listKeys[i]];
            if (__client.clientToGetResponse == string.Empty) continue;
            JSONNode __output = ParseOutput(__client.clientToGetResponse);
            if (__output != null)
            {
                HandleResponses(__client, __output);            
            }
        }

        string __streamToSend = string.Empty;
        switch (_currentServerState)
        {
            case ServerState.LOBBY:
                __streamToSend = StreamParser.GetInformUserStream(_serverData.dictUserData);
                break;
        }
       // UnityEngine.Debug.LogFormat("__streamToSend: {0}", __streamToSend);

        for (int i = 0;i < __listKeys.Count;i++)
        {
            SocketController.ClientData __client = _socketController.dictClients[__listKeys[i]];
            __client.clientToSendResponse = __streamToSend;
            _socketController.dictClients[__listKeys[i]] = __client;
        }
        Debug.Log("sending: " + __streamToSend);
        _socketController.StreamToClients();
    }

    private JSONNode ParseOutput(string __streamOutput)
    {
        if (__streamOutput == null)
            return null;
        JSONNode __node = JSON.Parse(__streamOutput);
        return (__node != null) ? __node : null;
    }

    private void HandleResponses(SocketController.ClientData p_client, JSONNode p_output)
    {
        foreach (var content in p_output.Keys)
        {
            string __content = content.ToString();
            switch (__content)
            {
                case "AddUser": 
                    HandleAddUser(p_client, p_output[content]);
                    break;
                case "Game":
                    break;
            }
        }
    }

    private void HandleAddUser(SocketController.ClientData p_client, JSONNode p_addUserNode)
    {
        if (_currentServerState != ServerState.LOBBY)
        {
            _currentServerState = ServerState.LOBBY;
        }

        string __userName = p_addUserNode["user"];

        ServerData.UserData __newUser = new ServerData.UserData(p_client.id);
        __newUser.userName = __userName;
        
        _serverData.dictUserData.Add(p_client.id, __newUser);

        p_client.clientToGetResponse = string.Empty;
        _socketController.dictClients[p_client.id] = p_client;
    }
    #endregion
}
