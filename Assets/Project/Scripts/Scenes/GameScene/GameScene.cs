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
    }

    public override void AUpdate()
    {        
        _cameraManager.UpdateMainCameraPosition();
        _environment.AUpdate();
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
