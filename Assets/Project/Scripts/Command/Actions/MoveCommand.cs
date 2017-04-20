using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command 
{
    public MoveCommand(Unit p_actor, Vector3 p_targetPosition)
    {
        _actor = p_actor;
        _targetPosition = p_targetPosition;
    }

    public override CommandType Execute()
    {
        _previousPosition = _actor.transform.position;
        _actor.GetNavMeshAgent().destination = _targetPosition;
        return CommandType.MOVE;
    }

    public override void Stop()
    {
        _actor.GetNavMeshAgent().Stop();
        _actor.GetNavMeshAgent().ResetPath();
    }

    public override void Undo()
    {
        _actor.GetNavMeshAgent().destination = _previousPosition;
    }

    private Unit _actor;

    private Vector3 _previousPosition;
    private Vector3 _targetPosition;

}
