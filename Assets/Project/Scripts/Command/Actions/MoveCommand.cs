using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command 
{
    public MoveCommand(Entity p_actor, Vector3 p_targetPosition)
    {
        _actor = p_actor;
        _targetPosition = p_targetPosition;
    }

    public override CommandType Execute()
    {
        _previousPosition = _actor.transform.position;
        if (_actor.GetEntityType() == EntityType.UNIT)
        {
            (_actor as EntityUnit).GetNavMeshAgent().destination = _targetPosition;
            return CommandType.MOVE;
        }
        else
        {
            return CommandType.NONE;
        }
    }

    public override void Stop()
    {
        if (_actor.GetEntityType() == EntityType.UNIT)
        {
            (_actor as EntityUnit).GetNavMeshAgent().isStopped = true;
            (_actor as EntityUnit).GetNavMeshAgent().ResetPath();
        }
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
