using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Framework;

public class Environment : MonoBehaviour 
{
    #region Event Data
    public event Action<Entity> onRequestSetInterfaceSelectedUnit;
    public event Action<Vector3> onRequestSetCameraFocusObject;
    #endregion

    #region Private Serialized-Data
    [SerializeField] private WorldManager _worldManager;
    #endregion

    #region Private Data
    private int _mainPlayer;
    private Dictionary<int, Player> _dictPlayers = new Dictionary<int, Player>();
    private InputManager _inputManager;
    private int _currentConnectedPlayers;
    #endregion

    public void AInitialize()
    {
        _worldManager.Initialize(1000, 7, 1000);
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
        for (int i = 0;i < __listPlayersObjects.Length;i++)
        {
            Player __player = __listPlayersObjects[i].GetComponent<Player>();
            __player.SetTeam(i);
            __player.transform.SetParent(transform);
            if (__player.GetIsLocalPlayer())
            {
                _mainPlayer = i;
            }
            _dictPlayers.Add(i, __player);        
        }
        Vector3 __randomStartPos = new Vector3(UnityEngine.Random.Range(-40f, 40f), 0f, UnityEngine.Random.Range(-40f, 40f));
        _dictPlayers[_mainPlayer].Initialize(_mainPlayer, __randomStartPos);
        InitializeMainPlayerEvents();
        if (onRequestSetCameraFocusObject != null) onRequestSetCameraFocusObject(__randomStartPos);
    }

    private void InitializeMainPlayerEvents()
    {
        _dictPlayers[_mainPlayer].onRequestShowSelectUnitUI += delegate (Entity p_unit)
        {
            if (onRequestSetInterfaceSelectedUnit != null) onRequestSetInterfaceSelectedUnit(p_unit);
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
