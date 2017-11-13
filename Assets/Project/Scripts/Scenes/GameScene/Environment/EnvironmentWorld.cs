using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Framework;
using System.Linq;

public class EnvironmentWorld : MonoBehaviour 
{
    #region Event Data
    public event Action<bool, Entity> onRequestSetInterfaceSelectedUnit;
    public event Action<int> onRequestUpdateResourcesUI;
    public event Action<Vector3> onRequestSetCameraFocusObject;

    public event Action onGameStarted;
    #endregion

    #region Public Data
    public PhotonView photonView;
    #endregion

    #region Private Serialized-Data
    [SerializeField] private WorldManager _worldManager;
    [SerializeField] private GameObject _resourcePrefab;
    #endregion

    #region Private Data
    //private int _mainPlayer;
    private Player _mainPlayer;
    private InputManager _inputManager;
    #endregion

    public void AInitialize()
    {       
        _worldManager.Initialize(100, 7, 100);
        photonView = GetComponent<PhotonView>();
        InitializeInputManager();
        Timer.WaitSeconds(1f, delegate
        {
            InitializePlayers();
        });
    }

    private void InitializePlayers()
    {
        Debug.Log("InitializePlayers: " + PhotonUtility.IsMasterClient);
        Debug.Log("Send Rate: " + PhotonNetwork.sendRate);
        if (PhotonUtility.IsMasterClient)
        {
            PhotonNetwork.sendRate = 30;
            PhotonNetwork.sendRateOnSerialize = 30;
            PhotonPlayer[] __arrayPlayers = PhotonUtility.playerList;

            List<Player> __listPlayers = new List<Player>();
            for (int i = 0;i < __arrayPlayers.Length;i++)
            {
                PhotonPlayer __photonPlayer = __arrayPlayers[i];

                Player __player = PoolManager.PhotonSpawn(PoolType.PLAYER, Vector3.zero, null).GetComponent<Player>();
                __player.photonView.TransferOwnership(__photonPlayer.ID);
                __listPlayers.Add(__player);
            }

            photonView.RPC("RPC_SetMainPlayer", PhotonTargets.All);

            for (int i = 0; i < __listPlayers.Count; i++)
            {
                Vector3 __randomStartPos = new Vector3(UnityEngine.Random.Range(-40f, 40f), 0f, UnityEngine.Random.Range(-40f, 40f));
                Color __playerColor = ((PlayerNetwork.Colors)PhotonUtility.playerList.ToList().Find(x => x.ID == __listPlayers[i].photonView.ownerId).CustomProperties["Color"]).ToColor();
                __listPlayers[i].photonView.RPC("RPC_Initialize", PhotonTargets.All, i, __playerColor.ToArray(), __randomStartPos);
                if (__listPlayers[i].photonView.isMine)
                {
                    if (onRequestSetCameraFocusObject != null) onRequestSetCameraFocusObject(__randomStartPos);
                }
            }

            for (int i = 0; i < 15; i++)
            {
                Resource __resource =SpawnResource(_resourcePrefab, new Vector3(UnityEngine.Random.Range(-40f, 40f), 0f, UnityEngine.Random.Range(-40f, 40f)));
                __resource.onResourceDepleted += () =>
                {
                    PhotonUtility.Destroy(__resource.gameObject);
                };
            }
        }

        //PhotonUtility.RaiseEvent(0, null, true);

        //Vector3 __randomStartPos = new Vector3(UnityEngine.Random.Range(-40f, 40f), 0f, UnityEngine.Random.Range(-40f, 40f));
        // InitializeMainPlayerEvents();
        //_dictPlayers[_mainPlayer].RPC_Initialize(__randomStartPos);
    }

    [PunRPC]
    private void RPC_SetMainPlayer()
    {
        GameObject[] __playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0;i < __playerObjects.Length;i++)
        {
            Player __player = __playerObjects[i].GetComponent<Player>();
            __player.transform.SetParent(transform);
            if (__player.photonView.isMine)
            {
                _mainPlayer = __player;
                InitializeMainPlayerEvents(_mainPlayer);
                break;
            }
        }

        if (onGameStarted != null)
            onGameStarted();
    }

    private void InitializeMainPlayerEvents(Player p_player)
    {
        p_player.onRequestShowSelectUnitUI += delegate (bool p_isPlayer, Entity p_unit)
        {
            if (onRequestSetInterfaceSelectedUnit != null) onRequestSetInterfaceSelectedUnit(p_isPlayer, p_unit);
        };

        p_player.onRequestUpdateResourcesUI += delegate (int p_crystalAmount)
        {
            if (onRequestUpdateResourcesUI != null) onRequestUpdateResourcesUI(p_crystalAmount);
        };
    }

    private void InitializeInputManager()
    {
        _inputManager = new InputManager();
        _inputManager.onMouseLeftClick += delegate (InputInfo p_inputInfo)
        {
            if (_mainPlayer != null)
            {
                _mainPlayer.HandleMouseLeftClick(p_inputInfo);
            }
        };

        _inputManager.onMouseRightClick += delegate (InputInfo p_inputInfo)
        {
            if (_mainPlayer != null)
            {
                _mainPlayer.HandleMouseRightClick(p_inputInfo);
            }
        };
    }

    public void OnExecuteMainPlayerTargetUnitCommand(string p_entityID, CommandType p_commandType, params object[] p_args)
    {
        _mainPlayer.ExecuteTargetUnitCommnad(p_entityID, p_commandType, p_args);
    }

    public void AUpdate()
    {
        
        if (_inputManager != null)
        {
            _inputManager.CheckInput();        
        }

        if (_mainPlayer != null)
        {
            _mainPlayer.AUpdate();
        }
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        _worldManager.BuildWorld();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public Resource SpawnResource(GameObject p_resource, Vector3 p_position)
    {
        Resource __resource = PoolManager.PhotonSpawn(PoolType.RESOURCE, p_position, null, false).GetComponent<Resource>();
        __resource.photonView.RPC("RPC_Initialize", PhotonTargets.All, UnityEngine.Random.Range(10,35));
        return __resource;
    }
    public void DespawnResource(GameObject p_resource)
    {
        PhotonUtility.Destroy(p_resource);
    }
}
