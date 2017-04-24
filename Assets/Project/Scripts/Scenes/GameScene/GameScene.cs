using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class GameScene : State<TCC.StateType>
{
    #region Private Serialized-Data
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private Environment _environment;
    [SerializeField] private EnvironmentCanvas _environmentCanvas;
    #endregion
    
    public override void AInitialize()
    {
        _cameraManager.Initialize(new Vector3(100f, 8f, 100f));
        _environment.AInitialize();
        _environmentCanvas.AInitialize();
        ListenEnvironmentEvents();
    }

    private void ListenEnvironmentEvents()
    {
        _environment.onRequestSetInterfaceSelectedUnit += delegate (Entity p_unit)
        {
            _environmentCanvas.SetInterfaceSelectedUnit(p_unit);
        };

        _environmentCanvas.onClickInterfaceCommand += delegate (int p_entityID, CommandType p_commandType, object[] p_args)
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
