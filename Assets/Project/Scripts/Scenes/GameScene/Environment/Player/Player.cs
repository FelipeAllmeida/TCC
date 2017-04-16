using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
    #region Private Data
    private int _team;
    private Unit _currentSelectedUnit;
   // private List<Unit> _listSelectedUnits = new List<Unit>();
    #endregion

    public void HandleMouseLeftClick(InputInfo p_inputInfo)
    {
        _currentSelectedUnit = p_inputInfo.hit;
        Debug.Log("_currentSelectedUnit IS NULL: " + ((_currentSelectedUnit == null) ? "yes" : "false"));
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
