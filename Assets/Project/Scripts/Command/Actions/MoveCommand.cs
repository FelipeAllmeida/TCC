using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command 
{
    private Action _actionReachedDestination;
    public MoveCommand(Entity p_actor, Vector3 p_targetPosition, Action p_callbackFinish = null)
    {
        _actor = p_actor;
        _targetPosition = p_targetPosition;
        _actionReachedDestination = p_callbackFinish;
    }

    public override CommandType Execute()
    {
        _previousPosition = _actor.transform.position;

        (_actor as EntityUnit).GetNavMeshAgent().destination = _targetPosition;
        return CommandType.MOVE;
    }

    public override void AUpdate()
    {
        if (((_actor as EntityUnit).GetNavMeshAgent().remainingDistance < (_actor as EntityUnit).GetRange()))
        {
            if (_actionReachedDestination != null)
            {
                _actionReachedDestination();
                _actionReachedDestination = null;
            }
        }
    }

    public override void Stop()
    {  
        (_actor as EntityUnit).GetNavMeshAgent().isStopped = true;
        (_actor as EntityUnit).GetNavMeshAgent().ResetPath();
        _actionReachedDestination = null;
    }

    public override void Undo()
    {
        if (_actor.GetEntityType() == EntityType.UNIT)
        {
            (_actor as EntityUnit).GetNavMeshAgent().destination = _previousPosition;
        }
    }

    private Entity _actor;

    private Vector3 _previousPosition;
    private Vector3 _targetPosition;

}
