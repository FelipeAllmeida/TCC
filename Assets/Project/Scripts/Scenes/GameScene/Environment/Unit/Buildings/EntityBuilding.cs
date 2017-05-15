using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitBuildingStateType
{
    UPGRADING_STRUCTURE,
    PRODUCING_UNIT,
    NONE
}

public class EntityBuilding : Entity
{

    #region Private Data
    //private EntityStateType<UnitBuildingStateType> _currentEntityState;

    #endregion

    public override void Initialize(int p_unitID, int p_unitTeam, EntityVO p_entityVO)
    {
        base.Initialize(p_unitID, p_unitTeam, p_entityVO);
    }

    public override void AUpdate()
    {
        base.AUpdate();
    }
}
