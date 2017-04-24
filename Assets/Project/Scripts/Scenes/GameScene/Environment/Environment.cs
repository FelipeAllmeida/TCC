using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour 
{
    #region Event Data
    public event Action<Entity> onRequestSetInterfaceSelectedUnit;
    #endregion

    #region Private Serialized-Data
    [SerializeField] private WorldManager _worldManager;
    #endregion

    #region Private Data
    private int _mainPlayer;
    private Dictionary<int, Player> _dictPlayers = new Dictionary<int, Player>();
    private InputManager _inputManager;
    #endregion

    public void AInitialize()
    {
        _worldManager.Initialize(1000, 7, 1000);
        InitializeInputManager();
        InitializeDictPlayers();
    }

    #region TEST-ONLY
    private void InitializeDictPlayers()
    {
        _dictPlayers.Add(_mainPlayer, new Player());
        InitializeMainPlayerEvents();
        _dictPlayers[_mainPlayer].Initialize(_mainPlayer, Vector3.zero);
    }

    #endregion

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

    public void OnExecuteMainPlayerTargetUnitCommand(int p_entityID, CommandType p_commandType, params object[] p_args)
    {
        _dictPlayers[_mainPlayer].ExecuteTargetUnitCommnad(p_entityID, p_commandType, p_args);
    }

    public void AUpdate()
    {
        _inputManager.CheckInput();
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
