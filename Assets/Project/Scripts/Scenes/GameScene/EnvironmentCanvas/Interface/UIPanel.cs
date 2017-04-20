using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPanel : MonoBehaviour 
{
    public virtual void Initialize()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Enable()
    {
        EnableUIPanel(true);
    }

    public virtual void Disable()
    {
        EnableUIPanel(false);
    }

    protected void EnableUIPanel(bool p_value)
    {
        gameObject.SetActive(p_value);
    }
}
