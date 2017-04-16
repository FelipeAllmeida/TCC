using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    CENTRAL_UNIT,
    GATHERING_UNIT,
}

[Serializable]
public class PoolPrefab
{
    public PoolType poolType;
    public GameObject prefab;
}

public class PoolManager : MonoBehaviour 
{
    #region Singleton
    private PoolManager _instance;
    public PoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = gameObject.GetComponent<PoolManager>();
                _instance.Initialize();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }
    #endregion

    #region Serialized Field Data
    [SerializeField] private List<PoolPrefab> _listPoolPrefabs = new List<PoolPrefab>();
    #endregion

    #region Private Data
    private Dictionary<PoolType, List<GameObject>> _dictPoolInstances;
    #endregion

    public void Initialize()
    {
        _dictPoolInstances = new Dictionary<PoolType, List<GameObject>>();
        foreach (PoolType __poolType in Enum.GetValues(typeof(PoolType)))
        {
            List<GameObject> __listGameObject = new List<GameObject>();
            _dictPoolInstances.Add(__poolType, __listGameObject);
            GameObject __go = new GameObject(__poolType.ToString());
            __go.transform.SetParent(transform);
        }
    }

    public GameObject Spawn(PoolType p_poolType)
    {
        if (_dictPoolInstances[p_poolType].Count == 0)
        {
            return Instantiate(_listPoolPrefabs.Find(x => x.poolType == p_poolType).prefab);
        }
        else
        {
            GameObject __go = _dictPoolInstances[p_poolType][0];
            __go.SetActive(true);
            _dictPoolInstances[p_poolType].RemoveAt(0);
            return __go;
        }
    }

    public void Despawn(PoolType p_poolType, GameObject p_gameObject)
    {
        _dictPoolInstances[p_poolType].Add(p_gameObject);
        p_gameObject.SetActive(false);
        p_gameObject.transform.position = new Vector3(10000f, 10000f, 10000f);
    }
}
