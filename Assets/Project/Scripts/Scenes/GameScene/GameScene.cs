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
        _environment.onRequestSetInterfaceSelectedUnit += delegate (Unit p_unit)
        {
            _environmentCanvas.SetInterfaceSelectedUnit(p_unit);
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
