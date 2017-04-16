using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitMovementType
{
    GROUND,
    AIR
}

public enum UnitBuildingType
{
    CENTER_BUILDING_UNIT
}

public enum UnitType
{
    GATHERING_UNIT
}

public abstract class Unit : MonoBehaviour 
{
    #region Protected Data
    protected int _id;
    protected int _team;

    protected float _currentHealth;
    protected float _maxHealth;

    protected Command _command;

    protected UnitMovementType _unitMovementType;
    #endregion

    #region Protected-Serialized Data
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
        _command = new MoveCommand(this, p_targetPosition);
        _command.Execute();
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

    public virtual int GetUnitID()
    {
        return _id;
    }

    public virtual NavMeshAgent GetNavMeshAgent()
    {
        return _navMeshAgent;
    }

    public virtual UnitMovementType GetUnitType()
    {
        return _unitMovementType;
    }
}

