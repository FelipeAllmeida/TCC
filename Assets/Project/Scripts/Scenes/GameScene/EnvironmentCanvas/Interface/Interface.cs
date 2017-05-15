﻿using System;
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
    [SerializeField] private EntityDisplayPanel _entityDisplayPanel;

    public void AInitialize()
    {
        InitializeSelectionPanel();
        InitializeEntityDisplayPanel();
    }

    private void InitializeSelectionPanel()
    {
        _unitSelectionPanel.Initialize();
        _unitSelectionPanel.onClickCommand += delegate (int p_entityID, CommandType p_commandType, object[] p_args)
        {
            if (onClickCommand != null) onClickCommand(p_entityID, p_commandType, p_args);
        };

    }

    private void InitializeEntityDisplayPanel()
    {
        _entityDisplayPanel.Initialize();
    }

    public void AUpdate()
    {
        _unitSelectionPanel.AUpdate();
        _entityDisplayPanel.AUpdate();
    }

    #region Panels
    public void SetSelectedUnit(Entity p_unit)
    {
        _unitSelectionPanel.SetSelectedUnit(p_unit);
        _entityDisplayPanel.SetSelectedUnit(p_unit);
    }

    public void DeselectUnit(Entity p_unit)
    {
        _unitSelectionPanel.DeselectUnit();
        _entityDisplayPanel.DeselectUnit();
    }
    #endregion
}
