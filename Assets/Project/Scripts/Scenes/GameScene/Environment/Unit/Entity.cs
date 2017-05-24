using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

public abstract class Entity : MonoBehaviour 
{
    #region Events
    public event Action<string, Vector3> onRequestCreateEntity;
    #endregion

    #region Protected Data
    [SerializeField] private EntityType _entityType;
    [SerializeField] protected int _id;
    [SerializeField] protected int _team;
    [SerializeField] protected int _currentFloor = 0;


    protected float _currentHealth;
    protected float _maxHealth;
    protected float _range;

    protected string _entityName;

    protected EntityVO _entityVO;


    private TweenNodule _currentUnitCreationNodule;

    private List<string> _listEntitiesToSpawn = new List<string>();

    private float _unitBuildPercentage;

    private bool _isBuilding = false;


    protected UnitMovementType _unitMovementType;
    #endregion

    #region Protected-Serialized Data
    [SerializeField] protected CommandController _commandController;
    [SerializeField] protected List<string> _listAvaiableEntities;
    #endregion

    public virtual void Initialize(int p_unitID, int p_unitTeam, EntityVO p_entityVO)
    {
        _id = p_unitID;
        _team = p_unitTeam;
        _entityVO = p_entityVO;
        InitializeEntityData();

    }

    protected void InitializeEntityData()
    {
        SetEntityName(_entityVO.entityName);
        _commandController.SetListAvaiableCommands(_entityVO.listAvaiableCommands);
        _listAvaiableEntities = _entityVO.listAvaiableEntitiesToBuild;
        _currentHealth = _maxHealth = _entityVO.maxHealth;
        _range = _entityVO.range;
    }

    public virtual void AUpdate()
    {
        _commandController.AUpdate();
    }

    public virtual int GetUnitTeam()
    {
        return _team;
    }

    public virtual int GetEntityID()
    {
        return _id;
    }

    public virtual float GetRange()
    {
        return _range;
    }

    protected virtual void SetEntityName(string p_name)
    {
        _entityName = p_name;
    }

    public virtual void SetEntityPosition(Vector3 p_position)
    {
        if (_entityType == EntityType.BUILDING)
        {
            p_position.y += GetComponent<Renderer>().bounds.extents.y;
        }
        transform.position = p_position;
    }

    public Vector3 GetEntityPosition()
    {
        return transform.position;
    }

    public virtual string GetEntityName()
    {
        return _entityName;
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
    public void AddUnitToSpawnList(string p_unitType)
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
}



