using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    NAVMESH,
    CENTRAL_UNIT,
    GATHERING_UNIT,
    DYNAMIC_ACTION_BUTTON
}

[Serializable]
public class PoolData
{
    public PoolType poolType;
    public GameObject prefab;
    [HideInInspector] public GameObject prefabParent;
}

public class PoolManager : MonoBehaviour 
{
    #region Singleton
    private static PoolManager _instance;
    public static PoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                return InstanceInitialize();
            }
            else
            {
                return _instance;
            }
        }
    }

    private static PoolManager InstanceInitialize()
    {
        GameObject __poolManagerObject = (GameObject)Resources.Load("PoolManager");

        _instance = __poolManagerObject.GetComponent<PoolManager>();
        _instance.Initialize();

        return _instance;
    }
    #endregion

    #region Serialized Field Data
    [SerializeField] private List<PoolData> _listPoolData = new List<PoolData>();
    #endregion

    #region Private Data
    private Dictionary<PoolType, PoolData> _dictPoolInstances;
    private GameObject _parent;
    #endregion

    public void Initialize()
    {
        if (_parent == null)
        {
            _parent = new GameObject("PoolManager");
        }
        _dictPoolInstances = new Dictionary<PoolType, PoolData>();
        for (int i = 0;i < _listPoolData.Count;i++)
        {
            PoolData __poolData = _listPoolData[i];
            __poolData.prefabParent = new GameObject(__poolData.poolType.ToString());
            __poolData.prefabParent.transform.SetParent(_parent.transform);
           _dictPoolInstances.Add(__poolData.poolType, __poolData);
        }
    }

    public GameObject Spawn(PoolType p_poolType, Transform p_parent = null)
    {
        return Spawn(p_poolType, Vector3.zero, p_parent);
    }

    public GameObject Spawn(PoolType p_poolType, Vector3 p_position, Transform p_parent = null)
    {
        if (_dictPoolInstances[p_poolType].prefabParent.transform.childCount == 0)
        {
            return Instantiate(_dictPoolInstances[p_poolType].prefab, p_parent);
        }
        else
        {
            GameObject __go = _dictPoolInstances[p_poolType].prefabParent.transform.GetChild(0).gameObject;
            __go.transform.SetParent(p_parent);
            __go.transform.localPosition = p_position;
            __go.SetActive(true);
            return __go;
        }
    }

    public void Despawn(PoolType p_poolType, GameObject p_gameObject)
    {
        p_gameObject.transform.position = new Vector3(1000f, 1000f, 1000f);
        p_gameObject.transform.SetParent(_dictPoolInstances[p_poolType].prefabParent.transform);
        p_gameObject.SetActive(false);
    }
}
