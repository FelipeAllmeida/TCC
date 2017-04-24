using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour 
{
    #region Events
    public event Action<int, CommandType, object[]> onClickCommand;
    #endregion

    [Header("Interface Panels")]
    [SerializeField] private UnitSelectionPanel _unitSelectionPanel;

    public void AInitialize()
    {
        InitializeSelectionPanel();
    }

    private void InitializeSelectionPanel()
    {
        _unitSelectionPanel.Initialize();
        _unitSelectionPanel.onClickCommand += delegate (int p_entityID, CommandType p_commandType, object[] p_args)
        {
            if (onClickCommand != null) onClickCommand(p_entityID, p_commandType, p_args);
        };

    }

    public void AUpdate()
    {

    }

    #region Panels
    #region Unit Selection Panel
    public void SetSelectedUnit(Entity p_unit)
    {
        _unitSelectionPanel.SetSelectedUnit(p_unit);
    }

    public void DeselectUnit(Entity p_unit)
    {
        _unitSelectionPanel.DeselectUnit();
    }
    #endregion
    #endregion
}
