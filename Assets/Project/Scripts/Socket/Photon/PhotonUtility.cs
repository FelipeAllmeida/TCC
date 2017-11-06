using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonUtility : MonoBehaviour
{
    #region Singleton
    private static PhotonUtility _instance;

    public static PhotonUtility InstanceInitialize()
    {
        GameObject __loadedGameObjectPrefab = (GameObject)Resources.Load("PhotonUtility");
        GameObject __gameObject = Instantiate(__loadedGameObjectPrefab);

        PersistentObject.instance.AddGameObject(__gameObject);

        _instance = __gameObject.GetComponent<PhotonUtility>();
        _instance.Initialize();

        return _instance;
    }
    #endregion

    #region Events
    public static Action onConnectedToMaster;
    public static Action onCreatedRoom;
    public static Action onJoinedLobby;
    public static Action onJoinedRoom;
    public static Action onReceivedRoomListUpdate;
    public static Action<PhotonPlayer> onPhotonPlayerConnected;
    public static Action<PhotonPlayer> onPhotonPlayerDisconnected;
    public static Action<PhotonPlayer> onMasterClientSwitched;


    public Action<object> onPhotonCreateRoomFailed;
    #endregion

    #region Custom Events
    public static Action onAllPlayersCreated;
    #endregion

    #region Public Data
    public static bool automaticallySyncScene
    {
        get
        {
            return PhotonNetwork.automaticallySyncScene;
        }
        set
        {
            PhotonNetwork.automaticallySyncScene = value;
        }
    }

    public static ExitGames.Client.Photon.Hashtable CustomProperties
    {
        get
        {
            return PhotonNetwork.player.CustomProperties;
        }
    }

    public static bool IsRoomOpen
    {
        get
        {
            return PhotonNetwork.room.IsOpen;
        }
        set
        {
            PhotonNetwork.room.IsOpen = value;
        }
    }

    public static bool IsRoomVisible
    {
        get
        {
            return PhotonNetwork.room.IsVisible;
        }
        set
        {
            PhotonNetwork.room.IsVisible = value;
        }
    }

    public static bool IsMasterClient
    {
        get
        {
            return PhotonNetwork.isMasterClient;
        }
    }

    public static string playerName
    {
        get
        {
            return PhotonNetwork.playerName;
        }
        set
        {
            PhotonNetwork.playerName = value;
        }
    }

    public static PhotonPlayer player
    {
        get
        {
            return PhotonNetwork.player;
        }
    }

    public static PhotonPlayer[] playerList
    {
        get
        {
            return PhotonNetwork.playerList;
        }
    }
    #endregion

    private void Initialize()
    {
        PhotonNetwork.ConnectUsingSettings("DEVELOPMENT");
        PhotonNetwork.OnEventCall += OnEvent;
    }

    #region Methods
    public static bool CreateRoom(string p_roomName)
    {
        return PhotonNetwork.CreateRoom(p_roomName);
    }

    public static bool CreateRoom(string p_roomName, RoomOptions p_roomOptions, TypedLobby p_typedLobby)
    {
        return PhotonNetwork.CreateRoom(p_roomName, p_roomOptions, p_typedLobby);
    }

    public static void Destroy(GameObject p_gameObject)
    {
        PhotonNetwork.Destroy(p_gameObject);
    }

    public static void Destroy(PhotonView p_photonView)
    {
        PhotonNetwork.Destroy(p_photonView);
    }

    public static GameObject Instantiate(string p_prefabPath, Vector3 p_position, Quaternion p_quaternion, byte p_group)
    {
        return PhotonNetwork.Instantiate(p_prefabPath, p_position, p_quaternion, p_group);
    }

    public static void LoadLevel(int p_level)
    {
        PhotonNetwork.LoadLevel(p_level);
    }

    public static void LoadLevel(string p_level)
    {
        PhotonNetwork.LoadLevel(p_level);
    }

    public static bool JoinRoom(string p_roomName)
    {
        return PhotonNetwork.JoinRoom(p_roomName);
    }

    public static bool JoinLobby(TypedLobby p_typedLobby)
    {
        return PhotonNetwork.JoinLobby(p_typedLobby);
    }

    public static bool LeaveRoom()
    {
        return PhotonNetwork.LeaveRoom();
    }

    public static RoomInfo[] GetRoomList()
    {
        return PhotonNetwork.GetRoomList();
    }

    public static void RaiseEvent(byte p_eventCode, object p_eventContent, bool p_sendReliable, RaiseEventOptions p_raiseEventOptions = null)
    {
        Debug.Log("Raise Event: " + p_eventCode);
        PhotonNetwork.RaiseEvent(p_eventCode, p_eventContent, p_sendReliable, p_raiseEventOptions);
    }
    #endregion

    #region Handlers
    private void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        if (onConnectedToMaster != null) onConnectedToMaster();
    }

    private void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        if (onJoinedLobby != null) onJoinedLobby();
    }

    private void OnPhotonCreateRoomFailed(object[] p_codeAndMessage)
    {
        Debug.Log("[" + p_codeAndMessage[0] + "]: " + p_codeAndMessage[1]);
        if (onPhotonCreateRoomFailed != null) onPhotonCreateRoomFailed(p_codeAndMessage);
    }

    private void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        if (onCreatedRoom != null) onCreatedRoom();
    }

    private void OnReceivedRoomListUpdate()
    {
        Debug.Log("OnReceivedRoomListUpdate");
        if (onReceivedRoomListUpdate != null) onReceivedRoomListUpdate();
    }

    private void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        if (onJoinedRoom != null) onJoinedRoom();
    }

    private void OnPhotonPlayerConnected(PhotonPlayer p_newPlayer)
    {
        if (onPhotonPlayerConnected != null) onPhotonPlayerConnected(p_newPlayer);
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer p_otherPlayer)
    {
        if (onPhotonPlayerDisconnected != null) onPhotonPlayerDisconnected(p_otherPlayer);
    }

    private void OnMasterClientSwitched(PhotonPlayer p_newMasterClient)
    {
        if (onMasterClientSwitched != null) onMasterClientSwitched(p_newMasterClient);
    }
    #endregion

    #region Custom Events
    private void OnEvent(byte p_eventCode, object p_content, int p_senderID)
    {
        Debug.Log("On Event: " + p_eventCode);
        if (p_eventCode == 0) // All players created
        {
            //Exemple
            //PhotonPlayer sender = PhotonPlayer.Find(p_senderID);  // who sent this?
            //byte[] selected = (byte[])p_content;
            //foreach (byte unitId in selected)
            //{
            //    // do something
            //}
            if (onAllPlayersCreated != null)
                onAllPlayersCreated();
        }
        
    }
    #endregion
}
