using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class AttackCommand : Command 
{
    private Entity _actor;
    private Entity _other;
    private TimerNodule _timerNodule;
    private bool _isFirtAttack = true;

    public AttackCommand(Entity p_entity, Entity p_other)
    {
        Debug.Log("AttackCommand");
        _actor = p_entity;
        _other = p_other;
    }

    public override CommandType Execute()
    {
        AttackRecursion();
        return CommandType.ATTACK;
    }

    public override void Stop()
    {
        if (_timerNodule != null)
            _timerNodule.Stop();
    }

    private void AttackRecursion()
    {
        _other.InflictDamage(_actor.GetDamage());
        _timerNodule = Timer.WaitSeconds(_actor.GetAttackSpeed(), AttackRecursion);
    }
}
