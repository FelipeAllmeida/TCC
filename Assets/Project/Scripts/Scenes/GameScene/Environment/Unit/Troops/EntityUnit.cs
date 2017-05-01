using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityUnit : Entity 
{
    #region Protected-Serialized Data
    [SerializeField] protected NavMeshAgent _navMeshAgent;
    #endregion

    public override void Initialize(int p_unitID, int p_unitTeam)
    {
        base.Initialize(p_unitID, p_unitTeam);
    }

    public override void AUpdate()
    {

    }

    public virtual void MoveTo(Vector3 p_targetPosition)
    {
        _commandController.MoveTo(this, p_targetPosition);
    }

    public virtual void StopMoving()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
    }

    public virtual NavMeshAgent GetNavMeshAgent()
    {
        return _navMeshAgent;
    }
}
