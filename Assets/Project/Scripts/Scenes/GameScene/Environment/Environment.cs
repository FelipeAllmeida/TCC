using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Framework;

public class Environment : MonoBehaviour 
{
    #region Event Data
    public event Action<bool, Entity> onRequestSetInterfaceSelectedUnit;
    public event Action<int> onRequestUpdateResourcesUI;
    public event Action<Vector3> onRequestSetCameraFocusObject;

    public event Action onGameStarted;
    #endregion

    #region Private Serialized-Data
    [SerializeField] private WorldManager _worldManager;
    [SerializeField] private GameObject _resourcePrefab;
    #endregion

    #region Private Data
    private int _mainPlayer;
    private Dictionary<int, Player> _dictPlayers = new Dictionary<int, Player>();
    private InputManager _inputManager;
    private int _currentConnectedPlayers;
    #endregion

    public void AInitialize()
    {       
        _worldManager.Initialize(100, 7, 100);
        InitializeInputManager();
        Timer.WaitSeconds(1f, delegate
        {
            InitializeDictPlayers();
        });
    }

    private void InitializeDictPlayers()
    {
        GameObject[] __listPlayersObjects = GameObject.FindGameObjectsWithTag("Player");
        _mainPlayer = -1;
        bool __isServer = false;
        for (int i = 0;i < __listPlayersObjects.Length;i++)
        {
            Player __player = __listPlayersObjects[i].GetComponent<Player>();
            __player.SetTeam(i);
            __player.transform.SetParent(transform);
            if (__player.GetIsLocalPlayer() == true)
            {
                _mainPlayer = i;
                if (__player.isServer == true)
                {
                    __isServer = true;
                }
            }
            _dictPlayers.Add(i, __player);        
        }

        if (__isServer == true)
        {
            for (int i = 0;i < 15; i++)
            {
                _dictPlayers[_mainPlayer].RpcSpawnResource(_resourcePrefab, new Vector3(UnityEngine.Random.Range(-40f, 40f), 0f, UnityEngine.Random.Range(-40f, 40f)));
                _resourcePrefab.GetComponent<Resource>().onResourceDepleted += delegate
                {
                    //_dictPlayers[_mainPlayer].DespawnResource(_resourcePrefab);
                };
            }
        }

        Vector3 __randomStartPos = new Vector3(UnityEngine.Random.Range(-40f, 40f), 0f, UnityEngine.Random.Range(-40f, 40f));
        InitializeMainPlayerEvents();
        _dictPlayers[_mainPlayer].Initialize(__randomStartPos);
        if (onRequestSetCameraFocusObject != null) onRequestSetCameraFocusObject(__randomStartPos);
        if (onGameStarted != null) onGameStarted();
    }

    private void InitializeMainPlayerEvents()
    {
        _dictPlayers[_mainPlayer].onRequestShowSelectUnitUI += delegate (bool p_isPlayer, Entity p_unit)
        {
            if (onRequestSetInterfaceSelectedUnit != null) onRequestSetInterfaceSelectedUnit(p_isPlayer, p_unit);
        };

        _dictPlayers[_mainPlayer].onRequestUpdateResourcesUI += delegate (int p_crystalAmount)
        {
            if (onRequestUpdateResourcesUI != null) onRequestUpdateResourcesUI(p_crystalAmount);
        };
    }

    private void InitializeInputManager()
    {
        _inputManager = new InputManager();
        _inputManager.onMouseLeftClick += delegate (InputInfo p_inputInfo)
        {
            if (_dictPlayers.ContainsKey(_mainPlayer) == true)
            {
                _dictPlayers[_mainPlayer].HandleMouseLeftClick(p_inputInfo);
            }
        };

        _inputManager.onMouseRightClick += delegate (InputInfo p_inputInfo)
        {
            if (_dictPlayers.ContainsKey(_mainPlayer) == true)
            {
                _dictPlayers[_mainPlayer].HandleMouseRightClick(p_inputInfo);
            }
        };
    }

    public void OnExecuteMainPlayerTargetUnitCommand(string p_entityID, CommandType p_commandType, params object[] p_args)
    {
        _dictPlayers[_mainPlayer].ExecuteTargetUnitCommnad(p_entityID, p_commandType, p_args);
    }

    public void AUpdate()
    {
        
        if (_inputManager != null)
        {
            _inputManager.CheckInput();        
        }
        if (_dictPlayers != null && _dictPlayers.ContainsKey(_mainPlayer))
        {
            _dictPlayers[_mainPlayer].AUpdate();
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
}
