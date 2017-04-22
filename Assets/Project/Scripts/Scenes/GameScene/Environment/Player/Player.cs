using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
    #region Event Data
    public event Action<Unit> onRequestShowSelectUnitUI;
    #endregion

    #region Private Data
    private int _team;
    private Unit _currentSelectedUnit;
   // private List<Unit> _listSelectedUnits = new List<Unit>();
    #endregion

    public void HandleMouseLeftClick(InputInfo p_inputInfo)
    {
        _currentSelectedUnit = p_inputInfo.hit;

        if (onRequestShowSelectUnitUI != null) onRequestShowSelectUnitUI(_currentSelectedUnit);
    }

    public void HandleMouseRightClick(InputInfo p_inputInfo)
    {
        if (_currentSelectedUnit != null)
        {
            if (_currentSelectedUnit.GetUnitTeam() == _team)
            {
                _currentSelectedUnit.MoveTo(p_inputInfo.worldClickPoint);
            }
        }
    }
}
