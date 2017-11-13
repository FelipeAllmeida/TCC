﻿using System;
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

public class Entity : MonoBehaviour 
{
    #region Events
    public event Action onCurrentCommandFinish;

    public event Action<Entity> onEntityCreated;
    public event Action<Entity> onDeath;

    public event Action<string, int, Color, Vector3> onRequestCreateEntity;

    public event Action<ResourceType, int> onAddResource;
    #endregion

    #region Public Data
    public PhotonView photonView;
    #endregion

    #region Protected Data
    [SerializeField] protected EntityType _entityType;
    [SerializeField] protected int _team;
    [SerializeField] protected int _currentFloor = 0;
    [SerializeField] protected GameObject _selectedGameObject;
    [SerializeField] protected NavMeshAgent _navMeshAgent;

    protected string _id;

    protected float _currentHealth;
    protected float _maxHealth;
    protected float _range;
    protected float _damage;
    protected float _attackSpeed;

    protected int _gatherEfficiency = 1;
    protected int _resourceCapacity;


    protected string _entityName;
    protected Color _entityColor;

    protected Vector3 _syncPosition;
    protected Quaternion _syncQuaternion;

    protected EntityVO __entityVO;


    protected TweenNodule _currentUnitCreationNodule;

    protected List<string> _listEntitiesToSpawn = new List<string>();


    protected float _unitBuildPercentage;

    protected bool _isBuilding = false;

    protected UnitMovementType _unitMovementType;
    #endregion

    #region Protected-Serialized Data
    [SerializeField] protected CommandController _commandController;
    [SerializeField] protected List<string> _listAvaiableEntities;
    #endregion

    //public override void OnStartClient()
    //{
    //    //GameObject parentObject = ClientScene.FindLocalObject(parentNetId);
    //    //transform.SetParent(parentObject.transform);
    //    //_team = parentObject.GetComponent<Player>().team;
    //}

    [PunRPC]
    public void RPC_Initialize(string p_unitID, int p_team, float[] p_arrayColor, string p_entitySpecificType)
    {
       // Debug.Log("Initialize Entity: " + p_unitID + " | team : " + _team + " | " + p_color);
        _id = p_unitID;
        _team = p_team;

        InitializeEntityData(p_entitySpecificType, p_arrayColor.ToColor());
    }

    protected void InitializeEntityData(string p_entitySpecificType, Color p_entityColor)
    {
        __entityVO = DataManager.instance.GetEntityVO(p_entitySpecificType);
       // Debug.Log("CmdInitializeEntityData");
        SetEntityName(__entityVO.entityName);
        SetEntityColor(p_entityColor);
        // Debug.Log("_entityVO listAvaiableCommands Count: " + __entityVO.listAvaiableCommands.Count);
        _commandController.AInitialize(__entityVO.listAvaiableCommands);
       // Debug.Log("_entityVO builds Count: " + __entityVO.listAvaiableEntitiesToBuild.Count);
        _listAvaiableEntities = __entityVO.listAvaiableEntitiesToBuild;
        _currentHealth = _maxHealth = __entityVO.maxHealth;
        _range = __entityVO.range;
        _damage = __entityVO.damage;
        _attackSpeed = __entityVO.attackSpeed;
        _entityType = __entityVO.entityType;
        _resourceCapacity = __entityVO.resourceCapacity;
        transform.localScale = __entityVO.size;

        _selectedGameObject.SetActive(false);
        Vector3 __localPos = _selectedGameObject.transform.localPosition;
        __localPos.y = - transform.localScale.y / 2f;
        _selectedGameObject.transform.localPosition = __localPos;
        _selectedGameObject.transform.localScale = new Vector3(__entityVO.size.x + 1.5f, __entityVO.size.x + 1.5f, 1f);

        _navMeshAgent.speed = __entityVO.speed;
        _navMeshAgent.angularSpeed = 360f;
        _navMeshAgent.acceleration = 10f;
    }

    private void OnPhotonSerializeView(PhotonStream p_stream, PhotonMessageInfo p_info)
    {
        if (p_stream.isWriting)
        {
            p_stream.SendNext(_currentHealth);
            p_stream.SendNext(transform.position);
            p_stream.SendNext(transform.rotation);
            //if (_commandController.GetCurrentCommand() == CommandType.MOVE)
            //{
            //    p_stream.SendNext(_navMeshAgent.destination);
            //}
        }
        else
        {
            _currentHealth = (float)p_stream.ReceiveNext();
            Vector3.Lerp(transform.position, (Vector3)p_stream.ReceiveNext(), 0.1f);
            Quaternion.Lerp(transform.rotation, (Quaternion)p_stream.ReceiveNext(), 0.1f);
            //if (_commandController.GetCurrentCommand() == CommandType.MOVE)
            //{
            //    _navMeshAgent.destination = (Vector3)p_stream.ReceiveNext();
            //}
            //else
            //{
            //    _navMeshAgent.isStopped = true;
            //}
        }
    }

    public void AUpdate()
    {
        _commandController.AUpdate();
    }

    public void InflictDamage(float p_damage)
    {
        photonView.RPC("RPC_InflictDamage", PhotonTargets.All, photonView.viewID, p_damage);
    }

    [PunRPC]
    private void RPC_InflictDamage(int p_viewID, float p_damage)
    {
        PhotonView __photonView = PhotonView.Find(p_viewID);
        if (__photonView == null) return;

         _currentHealth -= p_damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            if (photonView.isMine)
                if (onDeath != null) onDeath(this);
        }
    }

    protected void SetEntityName(string p_name)
    {
        _entityName = p_name;
        gameObject.name = p_name;
    }

    public void SetSelected(bool p_value)
    {
        _selectedGameObject.SetActive(p_value);
    }

    public void SetEntityPosition(Vector3 p_position)
    {
        if (_entityType == EntityType.BUILDING)
        {
            p_position.y += GetComponent<Renderer>().bounds.extents.y;
        }
        transform.position = p_position;
    }

    public void SetCurrentCommandFinishCallback(Action p_callback)
    {
        _commandController.onCurrentCommandFinish = p_callback;
    }

    public void SetEntityColor(Color p_color)
    {
        _entityColor = p_color;
        GetComponent<Renderer>().material.color = p_color;
    }

    public void AddResource(ResourceType p_resourceType, int p_resourceAmount)
    {
        if (onAddResource != null) onAddResource(p_resourceType, p_resourceAmount);
    }

    public int GetTeam()
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

    public float GetDamage()
    {
        return _damage;
    }

    public float GetAttackSpeed()
    {
        return _attackSpeed;
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

    public string GetEntityName()
    {
        return _entityName;
    }

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

    public float GetMaxHealth()
    {
        return _maxHealth;
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
    public void AddEntityToSpawnList(string p_unitType, Vector3 p_position)
    {
        _listEntitiesToSpawn.Add(p_unitType);
        SpawnUnitList();
    }

    protected void SpawnUnitList()
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

    protected void CreateNewUnit(string p_unitType, Action p_callbackFinish)
    {
        if (_currentUnitCreationNodule != null)
            _currentUnitCreationNodule.Stop();
        Debug.Log("Start Spawning: " + p_unitType);
        float __duration = DataManager.instance.GetEntityVO(p_unitType).timeToSpawn;
        _currentUnitCreationNodule = ATween.FloatTo(0f, 1f, __duration, TweenEase.LINEAR, delegate (float p_value)
        {
            // x  =0.5
            // (x * 10) / 10
            float __percentage = (p_value * __duration) / __duration;
            _unitBuildPercentage = __percentage;
        });
        _currentUnitCreationNodule.onFinished += delegate
        {
            Debug.Log("Unit Spawned: " + p_unitType);
            _unitBuildPercentage = 0f;
            
            if (onRequestCreateEntity != null) onRequestCreateEntity(p_unitType, _team, _entityColor, transform.position - new Vector3(0f, 0f, 1 + (transform.localScale.x / 2f)));
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
    public void AttackEntity(Entity p_other)
    {
        _commandController.AttackEntity(this, p_other);
    }

    public void MoveTo(Vector3 p_targetPosition)
    {
        _commandController.MoveTo(this, p_targetPosition);
    }

    public void StopCurrentCommand()
    {
        _commandController.StopCurrentCommand();
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return _navMeshAgent;
    }

    public void TakeResource(Resource p_resource)
    {
        Debug.Log("Take Resource");
        _commandController.GatherResource(this, p_resource);
    }
    #endregion
}



