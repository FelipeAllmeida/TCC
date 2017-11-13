using System;
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

    public AttackCommand(Entity p_entity, Entity p_other, Action<CommandType> p_commandFinish = null)
    {
        Debug.Log("AttackCommand");
        _actor = p_entity;
        _other = p_other;
        onCommandFinish = p_commandFinish;
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
        if (_actor != null && _other != null)
        {
            if (_actor.GetRange() > Vector3.Distance(_actor.GetEntityPosition(), _other.GetEntityPosition()))
            {
                _other.InflictDamage(_actor.GetDamage());
                _timerNodule = Timer.WaitSeconds(_actor.GetAttackSpeed(), AttackRecursion);
            }
            else
            {
                Stop();
            }
        }
        else
        {
            if (_other == null)
            {
                if (onCommandFinish != null)
                    onCommandFinish(CommandType.ATTACK);
            }
            Stop();
        }
    }
}
