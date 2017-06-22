using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using Framework;

public enum UnitMovementType
{
    GROUND,
    AIR
}

public enum EntityType
{
    BUILDING,
    UNIT
}

public class Entity : NetworkBehaviour 
{
    #region Events
    public event Action onEntityReachedDestination;

    public event Action<Entity> onEntityCreated;
    public event Action<string, Vector3> onRequestCreateEntity;
    #endregion

    #region Protected Data
    [SerializeField] private EntityType _entityType;
    [SyncVar] private string _id;
    [SerializeField] private int _team;
    [SerializeField] private int _currentFloor = 0;

    [SyncVar] public NetworkInstanceId parentNetId;

    protected float _currentHealth;
    protected float _maxHealth;
    protected float _range;

    private int _gatherEfficiency = 1;
    private int _resourceCapacity;


    protected string _entityName;

    protected EntityVO __entityVO;


    private TweenNodule _currentUnitCreationNodule;

    private List<string> _listEntitiesToSpawn = new List<string>();

    private Dictionary<ResourceType, int> _dictResourcesAmount = new Dictionary<ResourceType, int>();

    private NavMeshAgent _navMeshAgent;

    private float _unitBuildPercentage;

    private bool _isBuilding = false;


    protected UnitMovementType _unitMovementType;
    #endregion

    #region Protected-Serialized Data
    [SerializeField] protected CommandController _commandController;
    [SerializeField] protected List<string> _listAvaiableEntities;
    #endregion

    public override void OnStartClient()
    {
        GameObject parentObject = ClientScene.FindLocalObject(parentNetId);
        transform.SetParent(parentObject.transform);
        _team = parentObject.GetComponent<Player>().team;
    }

    [ClientRpc]
    public void RpcInitialize(string p_unitID, Color p_color, string p_entitySpecificType)
    {
        Debug.Log("Initialize Entity: " + p_unitID + " | team : " + _team + " | " + p_color);
        _id = p_unitID;

        InitializeEntityData(p_entitySpecificType, p_color);
    }

    private void InitializeEntityData(string p_entitySpecificType, Color p_entityColor)
    {
        __entityVO = DataManager.instance.GetEntityVO(p_entitySpecificType);
        Debug.Log("CmdInitializeEntityData");
        SetEntityName(__entityVO.entityName);
        SetEntityColor(p_entityColor);
        Debug.Log("_entityVO listAvaiableCommands Count: " + __entityVO.listAvaiableCommands.Count);
        _commandController.SetListAvaiableCommands(__entityVO.listAvaiableCommands);
        Debug.Log("_entityVO builds Count: " + __entityVO.listAvaiableEntitiesToBuild.Count);
        _listAvaiableEntities = __entityVO.listAvaiableEntitiesToBuild;
        _currentHealth = _maxHealth = __entityVO.maxHealth;
        _range = __entityVO.range;
        _entityType = __entityVO.entityType;
        _resourceCapacity = __entityVO.resourceCapacity;
        transform.localScale = __entityVO.size;

        switch (__entityVO.entityType)
        {
            case EntityType.BUILDING:
                gameObject.AddComponent<NavMeshSourceTag>();
                gameObject.transform.position += new Vector3(0f, GetComponent<Renderer>().bounds.extents.y, 0f);
                break;
            case EntityType.UNIT:        
                _navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
                _navMeshAgent.speed = __entityVO.speed;
                _navMeshAgent.angularSpeed = 360f;
                _navMeshAgent.acceleration = 10f;
                break;
        }
    }

    public void AUpdate()
    {
        _commandController.AUpdate();
    }

    public int GetUnitTeam()
    {
        return _team;
    }

    public string GetEntityID()
    {
        return _id;
    }

    public float GetRange()
    {
        return _range;
    }

    protected void SetEntityName(string p_name)
    {
        _entityName = p_name;
        gameObject.name = p_name;
    }

    public void SetEntityPosition(Vector3 p_position)
    {
        if (_entityType == EntityType.BUILDING)
        {
            p_position.y += GetComponent<Renderer>().bounds.extents.y;
        }
        transform.position = p_position;
    }

    public void SetEntityReachedDestinationCallback(Action p_callback)
    {
        onEntityReachedDestination = p_callback;
    }

    public void SetEntityColor(Color p_color)
    {
        GetComponent<Renderer>().material.color = p_color;
    }

    public Vector3 GetEntityPosition()
    {
        return transform.position;
    }

    public Vector3 GetEntitySpawnPosition()
    {
        Vector3 __spawnPosition = transform.position;
        __spawnPosition.z -= GetComponent<Renderer>().bounds.size.z *2;
        return __spawnPosition;
    }

    public virtual string GetEntityName()
    {
        return _entityName;
    }

    public int GetEntityGatherEfficiency()
    {
        return _gatherEfficiency;
    }

    public virtual EntityType GetEntityType()
    {
        return _entityType;
    }

    public virtual UnitMovementType GetUnitMovementType()
    {
        return _unitMovementType;
    }

    public virtual List<CommandType> GetListAvaiableCommands()
    {
        return _commandController.GetListAvaiableCommands();
    }

    public List<string> GetListAvaiableEntities()
    {
        return _listAvaiableEntities;
    }

    public int GetCurrentFloor()
    {
        return _currentFloor;
    }


    #region Spawn Entities
    public void AddUnitToSpawnList(string p_unitType, Vector3 p_position)
    {
        _listEntitiesToSpawn.Add(p_unitType);
        SpawnUnitList();
    }

    private void SpawnUnitList()
    {
        if (_listEntitiesToSpawn.Count > 0)
        {
            _isBuilding = true;
            CreateNewUnit(_listEntitiesToSpawn[0], delegate
            {
                _listEntitiesToSpawn.RemoveAt(0);
                SpawnUnitList();
            });
        }
        else
        {
            _isBuilding = false;
        }
    }

    private void CreateNewUnit(string p_unitType, Action p_callbackFinish)
    {
        if (_currentUnitCreationNodule != null)
            _currentUnitCreationNodule.Stop();
        Debug.Log("Start Spawning: " + p_unitType);
        float __duration = DataManager.instance.GetEntityVO(p_unitType).timeToSpawn;
        _currentUnitCreationNodule = ATween.FloatTo(0f, 1f, __duration, TweenEase.LINEAR, delegate (float p_value)
        {
            float __percentage = (p_value * __duration) / __duration;
            _unitBuildPercentage = __percentage;
        });
        _currentUnitCreationNodule.onFinished += delegate
        {
            Debug.Log("Unit Spawned: " + p_unitType);
            _unitBuildPercentage = 0f;
            if (onRequestCreateEntity != null) onRequestCreateEntity(p_unitType, transform.position);
            if (p_callbackFinish != null) p_callbackFinish();
        };
    }

    #region Get Methods

    public float GetUnitBuildPercentage()
    {
        return _unitBuildPercentage;
    }

    public bool GetIsBuilding()
    {
        return _isBuilding;
    }
    #endregion
    #endregion

    protected abstract class EntityState<T>
    {
        public event Action<T> onRequestChangeStep;

        public abstract void Start(Entity p_entity);

        public virtual void Pause()
        {

        }

        public virtual void Resume()
        {

        }

        public virtual void AUpdate()
        {

        }

        public abstract void Finish();

        public abstract T GetStepType();

        protected void ChangeStep(T p_stepType)
        {
            if (onRequestChangeStep != null)
                onRequestChangeStep(p_stepType);
        }
    }

    #region Commands

    public void MoveTo(Vector3 p_targetPosition)
    {
        _commandController.MoveTo(this, p_targetPosition, onEntityReachedDestination);
    }

    public void StopMoving()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return _navMeshAgent;
    }

    public void AddResource(ResourceType p_resourceType, int p_resourceAmount)
    {
        if (_dictResourcesAmount.ContainsKey(p_resourceType) == true)
        {
            _dictResourcesAmount[p_resourceType] += p_resourceAmount;
        }
        else
        {
            _dictResourcesAmount.Add(p_resourceType, p_resourceAmount);
        }
    }
    #endregion
}



