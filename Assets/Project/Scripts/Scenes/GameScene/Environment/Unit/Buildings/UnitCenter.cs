using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public enum UnitCenterStateType
{
    UPGRADING_STRUCTURE,
    PRODUCING_UNIT,
    NONE
}

public class UnitCenter : Unit
{
    #region Private Data
    private UnitCenterStateType _currentState = UnitCenterStateType.NONE;

    private TimerNodule _currentUnitCreationNodule;
    private List<UnitType> _listUnitsToSpawn = new List<UnitType>();

    
    #endregion

    public override void Initialize(int p_unitID, int p_unitTeam)
    {
        base.Initialize(p_unitID, p_unitTeam);
    }

    public override void AUpdate()
    {
      
    }


    public void AddUnitToSpawnList(UnitType p_unitType)
    {
        _listUnitsToSpawn.Add(p_unitType);
    }

    private void CreateNewUnit(UnitType p_unitType, Action p_callbackFinish)
    {
        if (_currentUnitCreationNodule != null) _currentUnitCreationNodule.Stop();
        _currentUnitCreationNodule = Timer.WaitSeconds(1, delegate
        {
            switch (p_unitType)
            {
                case UnitType.GATHERING_UNIT:
                    break;
            }
            if (p_callbackFinish != null)
                p_callbackFinish();
        });
    }
}
