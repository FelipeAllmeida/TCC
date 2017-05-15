using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityUnit : Entity 
{
    #region Protected-Serialized Data
    [SerializeField] protected NavMeshAgent _navMeshAgent;
    #endregion

    public override void Initialize(int p_unitID, int p_unitTeam, EntityVO p_entityVO)
    {
        base.Initialize(p_unitID, p_unitTeam, p_entityVO);
    }

    public override void AUpdate()
    {
        base.AUpdate();
    }

    public void MoveTo(Vector3 p_targetPosition, Action p_callbackFinish = null)
    {
        _commandController.MoveTo(this, p_targetPosition, p_callbackFinish);
    }

    public void StopMoving()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
    }

    public virtual NavMeshAgent GetNavMeshAgent()
    {
        return _navMeshAgent;
    }
}
