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

    public override void Execute()
    {
        _previousPosition = _actor.transform.position;
        if (_actor.GetUnitType() == UnitMovementType.GROUND)
        {
            _actor.GetNavMeshAgent().destination = _targetPosition;
        }
        else if (_actor.GetUnitType() == UnitMovementType.AIR)
        {

        }
    }

    public override void Undo()
    {
        if (_actor.GetUnitType() == UnitMovementType.GROUND)
        {
            _actor.GetNavMeshAgent().destination = _previousPosition;
        }
        else if (_actor.GetUnitType() == UnitMovementType.AIR)
        {

        }
    }

    private Unit _actor;

    private Vector3 _previousPosition;
    private Vector3 _targetPosition;

}
