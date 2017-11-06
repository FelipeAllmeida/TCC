using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class GameScene : State<TCC.StateType>
{
    #region Private Serialized-Data
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private EnvironmentWorld _environment;
    [SerializeField] private EnvironmentCanvas _environmentCanvas;
    #endregion

    #region Private Data
    
    #endregion

    public override void AInitialize()
    {
        DataManager.instance.AInitialize(delegate
        {
            _cameraManager.Initialize(new Vector3(100f, 8f, 100f));
            ListenEnvironmentEvents();
            _environmentCanvas.AInitialize();
            _environment.AInitialize();
        });
    }

    private void ListenEnvironmentEvents()
    {
        _environment.onRequestSetInterfaceSelectedUnit += delegate (bool p_isPlayer, Entity p_unit)
        {
            _environmentCanvas.SetInterfaceSelectedUnit(p_isPlayer, p_unit);
        };

        _environment.onRequestUpdateResourcesUI += delegate (int p_crystalAmount)
        {
            _environmentCanvas.UpdateResourcesPanel(p_crystalAmount);
        };

        _environment.onGameStarted += delegate
        {
            _environmentCanvas.RemoveInitializationBackground();
        };

        _environment.onRequestSetCameraFocusObject += delegate (Vector3 p_focusObjectPos)
        {
            _cameraManager.SetCameraPosToObject(p_focusObjectPos);
        };

        _environmentCanvas.onClickInterfaceCommand += delegate (string p_entityID, CommandType p_commandType, object[] p_args)
        {
            _environment.OnExecuteMainPlayerTargetUnitCommand(p_entityID, p_commandType, p_args);
        };
    }

    public override void AUpdate()
    {        
        _cameraManager.UpdateMainCameraPosition();
        _environment.AUpdate();
        _environmentCanvas.AUpdate();
    }

    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }
}
