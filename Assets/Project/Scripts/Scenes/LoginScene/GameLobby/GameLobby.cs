using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLobby : MonoBehaviour
{
    [SerializeField] private SignInScreen _signInScreen;
    [SerializeField] private LobbyScreen _lobbyScreen;

    public void AInitialize()
    {
        _signInScreen.Initialize();
        _signInScreen.EnableScreen(true);

        _lobbyScreen.Initialize();
        _lobbyScreen.EnableScreen(false);

        ListenScreenEvents();
    }

    private void ListenScreenEvents()
    {
        _signInScreen.onSuccessfullyConnect += HandleOnSuccessfullyConnect;
    }

    private void HandleOnSuccessfullyConnect()
    {
        _signInScreen.EnableScreen(false);
        _lobbyScreen.EnableScreen(true);
    }
}
