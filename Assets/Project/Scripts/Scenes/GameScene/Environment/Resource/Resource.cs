using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public enum ResourceType
{
    CRYSTAL
}

public class Resource : MonoBehaviour 
{
    public event Action onResourceDepleted;
    #region Private Data
    private ResourceType _resourceType;

    [SerializeField] private float _extractionTime = 1.5f;

    [SerializeField] private int _resourceQuantity = 250;

    private bool _isDepleted = false;
    public bool isDepleted { get { return _isDepleted; } }

    private TimerNodule _gatherNodule;
    #endregion

    #region Public Methods


    #region Action Methods
    public void Initialize(int p_resourceQuantity = 250)
    {
        _resourceQuantity = p_resourceQuantity;
    }

    public int GatherResource(int p_amount)
    {
        if (_isDepleted == true)
            return 0;

        int __amountRemoved = 0;
        if (_resourceQuantity >= p_amount)
        {
            _resourceQuantity -= p_amount;
            __amountRemoved = p_amount;
        }
        else
        {            
            __amountRemoved = _resourceQuantity;
            _resourceQuantity = 0;
            _isDepleted = true;
            if (onResourceDepleted != null)
                onResourceDepleted();
        }

        return __amountRemoved;
    }
    #endregion

    #region Get Methods
    public float GetExtractionTime()
    {
        return _extractionTime;
    }

    public int GetResourceQuantity()
    {
        return _resourceQuantity;
    }

    public ResourceType GetResourceType()
    {
        return _resourceType;
    }
    #endregion
    #endregion


}
