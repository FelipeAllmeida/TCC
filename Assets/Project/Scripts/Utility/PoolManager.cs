using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public enum PoolType
{
    ENTITY,
    FAKE_ENTITY,
    DYNAMIC_ACTION_BUTTON,
    LOBBY_PLAYER_FIELD,
    PLAYER,
    ROOM_LISTING,
    PLAYER_LISTING,
    RESOURCE
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
            DontDestroyOnLoad(_parent);
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

    public GameObject Spawn(PoolType p_poolType, Transform p_parent = null, bool p_adaptToRenderer = false)
    {
        return Spawn(p_poolType, Vector3.zero, p_parent);
    }

    public GameObject Spawn(PoolType p_poolType, Vector3 p_position, Transform p_parent = null, bool p_adaptToRenderer = false)
    {
        if (_dictPoolInstances.ContainsKey(p_poolType) == false)
            throw new ArgumentOutOfRangeException(string.Format("Pool does not contain {0}.", p_poolType));
        if (_dictPoolInstances[p_poolType].prefabParent.transform.childCount == 0)
        {
            return Instantiate(_dictPoolInstances[p_poolType].prefab, p_parent);
        }
        else
        {
            GameObject __go = _dictPoolInstances[p_poolType].prefabParent.transform.GetChild(0).gameObject;
            __go.transform.SetParent(p_parent);
            Vector3 __position = p_position;
            if (p_adaptToRenderer == true)
            {
                __position.y += __go.GetComponent<Renderer>().bounds.extents.y;
            }
            __go.transform.localPosition = __position;
            __go.SetActive(true);
            return __go;
        }
    }

    public static GameObject PhotonSpawn(PoolType p_poolType, Vector3 p_position, int? p_parentID, bool p_adaptToRenderer = false)
    {
        GameObject __go = PhotonUtility.Instantiate(GetPrefabPath(p_poolType), p_position, new Quaternion(0f, 0f, 0f, 1f), 0);
        //if (p_parentID != null)
            //__go.GetComponent<PhotonView>().RPC("SetParent", PhotonTargets.All, p_parentID);
        Vector3 __position = p_position;
        if (p_adaptToRenderer == true)
        {
            __position.y += __go.GetComponent<Renderer>().bounds.extents.y;
        }
        __go.transform.localPosition = __position;
        __go.SetActive(true);
        return __go;
    }

    private static string GetPrefabPath(PoolType p_poolType)
    {
        string __path = "Prefabs";

        switch (p_poolType)
        {
            case PoolType.ENTITY:
                __path = Path.Combine(__path, "Entity");
                break;
            case PoolType.PLAYER:
                __path = Path.Combine(__path, "Player");
                break;
            case PoolType.RESOURCE:
                __path = Path.Combine(__path, "Resource");
                break;
            default:
                throw new ArgumentException(string.Format("{0} is not a Photon Prefab.", p_poolType));
        }

        return __path;
    }

    public void Despawn(PoolType p_poolType, GameObject p_gameObject)
    {
        p_gameObject.transform.position = new Vector3(1000f, 1000f, 1000f);
        p_gameObject.transform.SetParent(_dictPoolInstances[p_poolType].prefabParent.transform);
        p_gameObject.SetActive(false);
    }
}
