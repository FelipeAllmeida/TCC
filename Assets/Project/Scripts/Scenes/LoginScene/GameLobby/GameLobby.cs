using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLobby : MonoBehaviour
{
    [SerializeField] private LobbyScreen _lobbyScreen;
    [SerializeField] private CurrentRoomScreen _currentRoomScreen;


    private void Start()
    {
        PhotonUtility.InstanceInitialize();

        _lobbyScreen.AInitialize();
        _currentRoomScreen.AInitialize();

        _lobbyScreen.Activate();
        _currentRoomScreen.Disable();

        ListenScreenEvents();
    }

    private void ListenScreenEvents()
    {
        _lobbyScreen.onJoinedRoom += HandleOnJoinedRoom;
        _currentRoomScreen.onLeaveRoom += HandleOnLeaveRoom;
        _currentRoomScreen.onClickStartMatchButton += HandleOnClickStartMatchButton;
    }

    private void HandleOnJoinedRoom()
    {
        _lobbyScreen.Disable();
        _currentRoomScreen.Activate();
    }

    private void HandleOnLeaveRoom()
    {
        _lobbyScreen.Activate();
        _currentRoomScreen.Disable();
    }

    private void HandleOnClickStartMatchButton()
    {
        //_lobbyScreen.Disable();
        //_currentRoomScreen.Disable();
        PhotonUtility.LoadLevel("Game");
    }


}
