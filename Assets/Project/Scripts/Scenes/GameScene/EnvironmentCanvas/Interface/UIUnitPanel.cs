﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIUnitPanelStateType
{
    NO_UNIT_SELECTED,
    UNIT_SELECTED,
}

public abstract class UIUnitPanel : UIPanel 
{
   
    #region Protected Data
    protected UIUnitPanelStateType _currentUnitPanelState;
    protected Unit _selectedUnit;
    #endregion

    public override void Initialize()
    {
        base.Initialize();
        _currentUnitPanelState = UIUnitPanelStateType.NO_UNIT_SELECTED;
    }

    public virtual void SetSelectedUnit(Unit p_unit)
    {
        _selectedUnit = p_unit;
        _currentUnitPanelState = UIUnitPanelStateType.UNIT_SELECTED;
    }

    public virtual void DeselectUnit()
    {
        _selectedUnit = null;
        _currentUnitPanelState = UIUnitPanelStateType.NO_UNIT_SELECTED;
    }

    public virtual UIUnitPanelStateType GetUIPanelStateType()
    {
        return _currentUnitPanelState;
    }
}
