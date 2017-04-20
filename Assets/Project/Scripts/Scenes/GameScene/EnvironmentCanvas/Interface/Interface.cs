using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour 
{
    [Header("Interface Panels")]
    [SerializeField] private UnitSelectionPanel _unitSelectionPanel;

    public void AInitialize()
    {
        _unitSelectionPanel.Initialize();
    }

    public void AUpdate()
    {

    }

    #region Panels
    #region Unit Selection Panel
    public void SetSelectedUnit(Unit p_unit)
    {
        _unitSelectionPanel.SetSelectedUnit(p_unit);
    }

    public void DeselectUnit(Unit p_unit)
    {
        _unitSelectionPanel.DeselectUnit();
    }
    #endregion
    #endregion
}
