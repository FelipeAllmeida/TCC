using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitMovementType
{
    GROUND,
    AIR
}

public enum EntityType
{
    CENTER_BUILDING_UNIT,
    GATHERING_UNIT
}

public enum UnitBuildingStateType
{
    UPGRADING_STRUCTURE,
    PRODUCING_UNIT,
    NONE
}

public abstract class Entity : MonoBehaviour 
{
    #region Protected Data
    protected int _id;
    protected int _team;
    protected int _currentFloor = 0;

    protected float _currentHealth;
    protected float _maxHealth;


    protected UnitMovementType _unitMovementType;
    #endregion

    #region Protected-Serialized Data
    [SerializeField] protected CommandController _commandController;
    [SerializeField] protected NavMeshAgent _navMeshAgent;
    #endregion

    public virtual void Initialize(int p_unitID, int p_unitTeam)
    {
        _id = p_unitID;
        _team = p_unitTeam;
    }

    public virtual void AUpdate()
    {
    
    }

    public virtual void MoveTo(Vector3 p_targetPosition)
    {
        _commandController.MoveTo(this, p_targetPosition);
    }

    public virtual void StopMoving()
    {
        _navMeshAgent.Stop();
        _navMeshAgent.ResetPath();
    }

    public virtual int GetUnitTeam()
    {
        return _team;
    }

    public virtual int GetEntityID()
    {
        return _id;
    }

    public virtual NavMeshAgent GetNavMeshAgent()
    {
        return _navMeshAgent;
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

