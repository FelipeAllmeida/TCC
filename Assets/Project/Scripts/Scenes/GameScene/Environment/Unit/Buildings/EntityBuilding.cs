using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public enum UnitBuildingStateType
{
    UPGRADING_STRUCTURE,
    PRODUCING_UNIT,
    NONE
}

public class EntityBuilding : Entity
{
    #region Private Serialized-Data
    [SerializeField] private List<EntityUnitType> _listAvaiableUnits;
    #endregion

    #region Private Data
    private UnitBuildingStateType _currentState = UnitBuildingStateType.NONE;

    private TimerNodule _currentUnitCreationNodule;

    private List<EntityUnitType> _listUnitsToSpawn = new List<EntityUnitType>(); 
    #endregion

    public override void Initialize(int p_unitID, int p_unitTeam)
    {
        base.Initialize(p_unitID, p_unitTeam);
    }

    public override void AUpdate()
    {
      
    }

    public void AddUnitToSpawnList(EntityUnitType p_unitType)
    {
        _listUnitsToSpawn.Add(p_unitType);
    }

    public List<EntityUnitType> GetListAvaiableUnits()
    {
        return _listAvaiableUnits;
    }

    private void CreateNewUnit(EntityUnitType p_unitType, Action p_callbackFinish)
    {
        if (_currentUnitCreationNodule != null) _currentUnitCreationNodule.Stop();
        _currentUnitCreationNodule = Timer.WaitSeconds(1, delegate
        {
            switch (p_unitType)
            {
                case EntityUnitType.UNIT_WORKER:
                    break;
            }
            if (p_callbackFinish != null) p_callbackFinish();
        });
    }
}
