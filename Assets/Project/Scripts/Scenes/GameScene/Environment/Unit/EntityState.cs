using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityState<T> 
{
    public abstract void Start(params object[] p_args);

    public virtual void Resume()
    {
    
    }

    public virtual void Pause()
    {
    
    }

    public abstract void Finish();

    public abstract T GetEntityState();
}

public class EntityStateType<T> : EntityState<T>
{
    protected T _state;

    public override void Start(params object[] p_args)
    {
    }

    public override void Finish()
    {
    }

    public override T GetEntityState()
    {
        return _state;
    }
}
