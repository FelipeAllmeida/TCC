using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentCanvas : MonoBehaviour 
{
    #region Private Serialized-Data
    [SerializeField] private Interface _interface;
    #endregion

    #region Private Data
    private Vector2 _canvasDimensions;
    #endregion

    public void AInitialize()
    {
        _interface.AInitialize();
        Debug.Log(GetComponent<RectTransform>().sizeDelta);
        Debug.Log(GetComponent<RectTransform>().rect.width + " | " + GetComponent<RectTransform>().rect.height);
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
