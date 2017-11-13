using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour 
{
    public enum Colors
    {
        BLUE,
        RED,
        YELLOW,
        GREEN,
        WHITE,
        ORANGE,
        BLACK
    }

    public static PlayerNetwork instance;
    public string playerName { get; private set; }

    private int _playersInGame;
    public int PlayersInGame
    {
        get
        {
            return _playersInGame;
        }
    }

    public PhotonView photonView;

    private void Awake()
    {
        instance = this;
        playerName = string.Format("Player#{0}", Random.Range(1000, 9999));
        photonView = GetComponent<PhotonView>();
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene p_scene, LoadSceneMode p_loadSceneMode)
    {
        //if (p_scene.name == "Game")
        //{
        //    if (PhotonUtility.IsMasterClient)
        //    {
        //        photonView.RPC("RPC_OtherSceneLoaded", PhotonTargets.Others);
        //    }
        //    else
        //    {
        //        photonView.RPC("RPC_MasterSceneLoaded", PhotonTargets.Others);
        //    }
        //}
    }

    private void MasterLoadedGame()
    {
        _playersInGame = 1;
        photonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }

    private void NonMasterLoadedGame()
    {
        photonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonUtility.LoadLevel("Game");
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        _playersInGame++;
        if (PlayersInGame == PhotonUtility.playerList.Length)
        {
            Debug.Log("All players are in the game scene");
        }
    }
}
