using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentCanvas : MonoBehaviour 
{
    #region Events
    public event Action<int, CommandType, object[]> onClickInterfaceCommand;
    #endregion

    #region Private Serialized-Data
    [SerializeField] private Interface _interface;
    #endregion

    #region Private Data
    private Vector2 _canvasDimensions;
    #endregion

    public void AInitialize()
    {
        InitializeInterface();
       //Debug.Log(GetComponent<RectTransform>().sizeDelta);
       // Debug.Log(GetComponent<RectTransform>().rect.width + " | " + GetComponent<RectTransform>().rect.height);
    }

    private void InitializeInterface()
    {
        _interface.AInitialize();
        _interface.onClickCommand += delegate (int p_entityID, CommandType p_commandType, object[] p_args)
        {
            if (onClickInterfaceCommand != null) onClickInterfaceCommand(p_entityID, p_commandType, p_args);
        };
    }

    public void AUpdate()
    {
        _interface.AUpdate();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    #region Interface
    public void SetInterfaceSelectedUnit(Entity p_unit)
    {
        _interface.SetSelectedUnit(p_unit);
    }
    #endregion
}
