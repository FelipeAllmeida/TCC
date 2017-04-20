using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentCanvas : MonoBehaviour 
{
    #region Private Serialized-Data
    [SerializeField] private Interface _interface;
    #endregion

    public void AInitialize()
    {
        _interface.AInitialize();
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
    public void SetInterfaceSelectedUnit(Unit p_unit)
    {
        _interface.SetSelectedUnit(p_unit);
    }
    #endregion
}
