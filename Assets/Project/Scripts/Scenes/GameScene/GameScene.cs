using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class GameScene : State<TCC.StateType>
{
    #region Private Serialized-Data
    [SerializeField] private WorldManager _worldManager;
    [SerializeField] private CameraManager _cameraManager;
    #endregion

    #region Private Data
    private int _mainPlayer = 0;
    private Dictionary<int, Player> _dictPlayers = new Dictionary<int, Player>();
    private InputManager _inputManager;
    #endregion
    
    public override void AInitialize()
    {
        _cameraManager.Initialize(new Vector3(20f, 8f, 20f));
        InitializeInputManager();
        InitializeDictPlayers();
    }

    #region TEST-ONLY
    private void InitializeDictPlayers()
    {
        _dictPlayers.Add(_mainPlayer, new Player());
    }
    #endregion

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

    public override void AUpdate()
    {        
        _inputManager.CheckInput();
        _cameraManager.UpdateMainCameraPosition(_inputManager.GetMousePosition());
    }

    public override void Enable()
    {
        gameObject.SetActive(true);
        _worldManager.Initialize (20, 20);
        _worldManager.BuildWorld();
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }
}
