using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;



public class EntityBuilding : Entity
{
    #region Private Data
    private UnitBuildingStateType _currentState = UnitBuildingStateType.NONE;

    private TimerNodule _currentUnitCreationNodule;
    private List<EntityType> _listUnitsToSpawn = new List<EntityType>();    
    #endregion

    public override void Initialize(int p_unitID, int p_unitTeam)
    {
        base.Initialize(p_unitID, p_unitTeam);
    }

    public override void AUpdate()
    {
      
    }


    public void AddUnitToSpawnList(EntityType p_unitType)
    {
        _listUnitsToSpawn.Add(p_unitType);
    }

    private void CreateNewUnit(EntityType p_unitType, Action p_callbackFinish)
    {
        if (_currentUnitCreationNodule != null) _currentUnitCreationNodule.Stop();
        _currentUnitCreationNodule = Timer.WaitSeconds(1, delegate
        {
            switch (p_unitType)
            {
                case EntityType.GATHERING_UNIT:
                    break;
            }
            if (p_callbackFinish != null) p_callbackFinish();
        });
    }
}
