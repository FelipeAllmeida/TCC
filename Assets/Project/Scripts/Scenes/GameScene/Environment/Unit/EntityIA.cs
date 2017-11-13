using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;



public class EntityIA : Entity
{
    internal enum IAType
    {
        BOSS_1 = 0
    }

    internal enum StepType
    {
        SEARCHING,
        FOLLOWING,
        ATTACKING,
        DEATH
    }

    #region Enumerators
    private IAType iaType;
    #endregion

    #region Private Serialized Data
    private SphereCollider _collider;
    #endregion

    #region Private Data
    private EntityStep<StepType> _currentStep;
    private Entity _currentTarget;
    #endregion

    public override void AUpdate()
    {
        if (_currentStep != null)
        {
            _currentStep.AUpdate();
        }
        base.AUpdate();
    }

    private void ChangeStep(StepType p_stepType)
    {
        switch (p_stepType)
        {
            case StepType.SEARCHING:
                break;
            case StepType.ATTACKING:
                break;
            case StepType.FOLLOWING:
                break;
            case StepType.DEATH:
                break;
            default:
                throw new ArgumentException(p_stepType + " does not have a handler");
        }
    }
}
