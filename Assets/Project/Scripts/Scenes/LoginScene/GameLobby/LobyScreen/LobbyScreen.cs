using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class LobbyScreen : MonoBehaviour
{
    #region Events
    public event Action onJoinedRoom;
    #endregion

    #region Private Serialized-Data
    [SerializeField] private RoomLayoutGroup _roomLayoutGroup;
    private RoomLayoutGroup RoomLayoutGroup;

    [SerializeField] private Text _enterRoomText;
    [SerializeField] private Text _createRoomText;
    [SerializeField] private Button _enterRoomButton;
    [SerializeField] private Button _createRoomButton;
    #endregion

    #region Private Data
    #endregion

    public void AInitialize()
    {
        _roomLayoutGroup.AInitialize();

        _enterRoomButton.interactable = false;
        _createRoomButton.interactable = false;

        ListenEvents();
    }

    private void ListenEvents()
    {
        PhotonUtility.onConnectedToMaster += () =>
        {
            PhotonUtility.playerName = PlayerNetwork.instance.playerName;

            Color __newColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
            PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Color", __newColor.ToArray()} });
            PhotonUtility.automaticallySyncScene = true;
            PhotonUtility.JoinLobby(TypedLobby.Default);
        };

        PhotonUtility.onJoinedRoom += () =>
        {
            if (onJoinedRoom != null) onJoinedRoom();
        };

        PhotonUtility.onJoinedLobby += () =>
        {
            _createRoomButton.interactable = true;
            _enterRoomButton.interactable = true;
        };

        _createRoomButton.onClick.AddListener(delegate
        {
            RoomOptions __roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };
            PhotonUtility.CreateRoom(_createRoomText.text, __roomOptions, TypedLobby.Default);
        });

        _roomLayoutGroup.onClickJoinRoom -= HandleOnClickJoinRoom;
        _roomLayoutGroup.onClickJoinRoom += HandleOnClickJoinRoom;
    }

    private void HandleOnClickJoinRoom(string p_roomName)
    {
        bool __joinedRoom = PhotonUtility.JoinRoom(p_roomName);
        if (__joinedRoom == true)
        {

        }
        else
        {
            Debug.LogError("Not able to join room: " + p_roomName);
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
