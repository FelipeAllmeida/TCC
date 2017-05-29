﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class LobbyHookPlayer : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager p_manager, GameObject p_lobbyPlayer, GameObject p_player)
    {
        LobbyPlayer __lobbyPlayer = p_lobbyPlayer.GetComponent<LobbyPlayer>();
        Player __player = p_player.GetComponent<Player>();
        __player.gameObject.name = __lobbyPlayer.playerName;
        __player.SetPlayerNameAndColor(__lobbyPlayer.playerName, __lobbyPlayer.playerColor);
        DataManager.connectedPlayers++;
    }
}
