using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command 
{
    public virtual CommandType Execute()
    {
        return CommandType.NONE;
    }

    public virtual void AUpdate()
    {

    }

    public virtual void Stop()
    {
    
    }

    public virtual void Undo()
    {
      
    }
}
