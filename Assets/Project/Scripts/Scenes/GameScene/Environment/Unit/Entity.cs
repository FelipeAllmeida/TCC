using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitMovementType
{
    GROUND,
    AIR
}

public enum EntityType
{
    BUILDING,
    UNIT
}

public enum EntityUnitType
{
    UNIT_WORKER
}

public enum EntityBuildingType
{
    BUILDING_CENTER
}


public abstract class Entity : MonoBehaviour 
{
    #region Protected Data
    [SerializeField] private EntityType _entityType;
    [SerializeField] protected int _id;
    [SerializeField] protected int _team;
    [SerializeField] protected int _currentFloor = 0;

    protected float _currentHealth;
    protected float _maxHealth;


    protected UnitMovementType _unitMovementType;
    #endregion

    #region Protected-Serialized Data
    [SerializeField] protected CommandController _commandController;
    #endregion

    public virtual void Initialize(int p_unitID, int p_unitTeam)
    {
        _id = p_unitID;
        _team = p_unitTeam;
    }

    public virtual void AUpdate()
    {
    
    }

    public virtual int GetUnitTeam()
    {
        return _team;
    }

    public virtual int GetEntityID()
    {
        return _id;
    }

    public virtual EntityType GetEntityType()
    {
        return _entityType;
    }

    public virtual UnitMovementType GetUnitMovementType()
    {
        return _unitMovementType;
    }

    public virtual List<CommandType> GetListAvaiableCommands()
    {
        return _commandController.GetListAvaiableCommands();
    }

    public int GetCurrentFloor()
    {
        return _currentFloor;
    }
}

