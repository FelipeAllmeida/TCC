using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    #region Event Data
    public event Action<bool, Entity> onRequestShowSelectUnitUI;
    public event Action<int> onRequestUpdateResourcesUI;
    #endregion

    #region Public Get-Only
    public int team
    {
        get
        {
            return _team;
        }
    }
    #endregion

    #region Private Data
    private Dictionary<string, Entity> _dictEntity = new Dictionary<string, Entity>();
    private Dictionary<ResourceType, int> _dictResourcesAmount = new Dictionary<ResourceType, int>();
    private Entity _currentSelectedUnit;
    private FakeEntity _fakeEntity;

    private string _playerName;
    private int _team;
    private int _idCounter = 0;

    [SerializeField] private int _resources;

    private bool _isSpawningEntityBuilding = false;

    private Color _playerColor;

    // private List<Unit> _listSelectedUnits = new List<Unit>();
    #endregion

    public void SetPlayerNameAndColor(string p_name, Color p_color)
    {
        Debug.Log("PlayerName: " + p_name + " | " + p_color);
        _playerName = p_name;
        _playerColor = p_color;
    }
    
    public void Initialize(int p_team, Vector3 p_startPosition)
    {
        _team = p_team;
        InitializeDictResources();
        CmdCreateNewEntity(netId, "BUILDING_CENTER", p_startPosition);

        CmdCreateNewEntity(netId, "UNIT_WORKER", p_startPosition - Vector3.forward);
        CmdCreateNewEntity(netId, "UNIT_WORKER", p_startPosition + Vector3.left - Vector3.forward);
        CmdCreateNewEntity(netId, "UNIT_WORKER", p_startPosition + Vector3.right - Vector3.forward);
    }

    private void InitializeDictResources()
    {
        _dictResourcesAmount.Add(ResourceType.CRYSTAL, 200);

        if (onRequestUpdateResourcesUI != null) onRequestUpdateResourcesUI(_dictResourcesAmount[ResourceType.CRYSTAL]);
    }

    public void SetTeam(int p_team)
    {
        _team = p_team;
    }

    public bool GetIsLocalPlayer()
    {
        return isLocalPlayer;
    }

    public void AUpdate()
    {
        if (_isSpawningEntityBuilding == true)
        {
            _fakeEntity.AUpdate();
        }

        foreach (var content in _dictEntity)
        {
            content.Value.AUpdate();
        }
    }

    [Command]
    private void CmdCreateNewEntity(NetworkInstanceId p_id, string p_entitySpecificType, Vector3 p_position)
    {
        Debug.Log("Create New Entity -> " + p_entitySpecificType);
        SpawnEntity(p_id, p_entitySpecificType, p_position);        
    }


    [ClientRpc]
    private void RpcListenEntityEvents(GameObject p_entity)
    {
        p_entity.GetComponent<Entity>().onRequestCreateEntity += HandleCmdCreateNewEntity;
        p_entity.GetComponent<Entity>().onAddResource += HandleAddResource;
    }

    private void HandleCmdCreateNewEntity(string p_entitySpecificType, Vector3 p_startPos)
    {
        CmdCreateNewEntity(netId, p_entitySpecificType, p_startPos);
    }

    private void HandleAddResource(ResourceType p_resourceType, int p_resourceAmount)
    {
        if (_dictResourcesAmount.ContainsKey(p_resourceType) == true)
        {
            _dictResourcesAmount[p_resourceType] += p_resourceAmount;
        }
        else
        {
            _dictResourcesAmount.Add(p_resourceType, p_resourceAmount);
        }
        if (onRequestUpdateResourcesUI != null)
            onRequestUpdateResourcesUI(_dictResourcesAmount[ResourceType.CRYSTAL]);
    }

    public void HandleMouseLeftClick(InputInfo p_inputInfo)
    {
        GameObject __hit = p_inputInfo.hit;
        if (_isSpawningEntityBuilding == true)
        {
            switch (__hit.tag)
            {
                case "Entity":
                    Vector3 __buildPos = _fakeEntity.transform.position;
                    float __distance = Vector3.Distance(_dictEntity[_fakeEntity.GetConstructorID()].GetEntityPosition(), __buildPos);
                    Action __callbackBuildEntity = delegate
                    {
                        __callbackBuildEntity = null;
                        _dictEntity[_fakeEntity.GetConstructorID()].AddEntityToSpawnList(_fakeEntity.GetEntityType(), __buildPos);
                    };
                    PoolManager.instance.Despawn(PoolType.FAKE_ENTITY, _fakeEntity.gameObject);
                    _isSpawningEntityBuilding = false;
                    if (__distance < _dictEntity[_fakeEntity.GetConstructorID()].GetRange())
                    {
                        __callbackBuildEntity();
                    }
                    else
                    {
                        _dictEntity[_fakeEntity.GetConstructorID()].SetCurrentCommandFinishCallback(__callbackBuildEntity);
                        CmdMoveEntity(_dictEntity[_fakeEntity.GetConstructorID()].gameObject, __buildPos);
                    }
                    break;
            }
        }
        else
        {
            switch (__hit.tag)
            {
                case "Entity":
                    _currentSelectedUnit = __hit.GetComponent<Entity>();
                    bool __isPlayer = (_currentSelectedUnit.GetTeam() == team) ? true : false;
                    if (onRequestShowSelectUnitUI != null) onRequestShowSelectUnitUI(__isPlayer, _currentSelectedUnit);
                    break;
                case "Resource":
                    break;
                case "Ground":
                    _currentSelectedUnit = null;
                    if (onRequestShowSelectUnitUI != null)
                        onRequestShowSelectUnitUI(false, _currentSelectedUnit);

                    break;
            }
        }
    }

    public void HandleMouseRightClick(InputInfo p_inputInfo)
    {
        GameObject __hit = p_inputInfo.hit;

        if (__hit == null) return;

        if (_isSpawningEntityBuilding == true)
        {
            if (_isSpawningEntityBuilding == true)
            {
                PoolManager.instance.Despawn(PoolType.FAKE_ENTITY, _fakeEntity.gameObject);
                _isSpawningEntityBuilding = false;            
            }
        }
        else
        {
            if (_currentSelectedUnit != null)
            {
                Debug.Log("entity team: " + _currentSelectedUnit.GetTeam() + " | player team: " + _team);
                
                if (_currentSelectedUnit.GetTeam() == _team)
                {
                    if (p_inputInfo.hit != null && p_inputInfo.hit.tag != "UI")
                    {
                        _currentSelectedUnit.StopCurrentCommand();
                    }
                    float __distance;
                    Action __callbackFinishCommand;
                    switch (__hit.tag)
                    {
                        case "Entity":
                            Entity __entity = __hit.GetComponent<Entity>();
                            if (__entity.GetTeam() == _currentSelectedUnit.GetTeam())
                            //ALLY
                            {
                                if (_currentSelectedUnit.GetEntityType() == EntityType.UNIT)
                                {
                                    if (p_inputInfo.hit != null && p_inputInfo.hit.tag == "Entity")
                                    {
                                        CmdMoveEntity(_currentSelectedUnit.gameObject, p_inputInfo.worldClickPoint);
                                    }                                    
                                }
                            }
                            else
                            //ENEMY
                            {
                                __distance = Vector3.Distance(_dictEntity[_currentSelectedUnit.GetEntityID()].GetEntityPosition(), __entity.GetEntityPosition());
                                __callbackFinishCommand = delegate
                                {
                                    __callbackFinishCommand = null;
                                    GameObject __entityObject = _dictEntity[_currentSelectedUnit.GetEntityID()].gameObject;
                                    GameObject __targetEntityObject = __hit.gameObject;
                                    CmdAttackEntity(__entityObject, __targetEntityObject);
                                };
                                if (__distance < _dictEntity[_currentSelectedUnit.GetEntityID()].GetRange())
                                {
                                    __callbackFinishCommand();
                                }
                                else
                                {
                                    _dictEntity[_currentSelectedUnit.GetEntityID()].SetCurrentCommandFinishCallback(__callbackFinishCommand);
                                    CmdMoveEntity(_dictEntity[_currentSelectedUnit.GetEntityID()].gameObject, __entity.GetEntityPosition());
                                }
                            }
                            break;
                        case "Ground":
                            CmdMoveEntity(_currentSelectedUnit.gameObject, p_inputInfo.worldClickPoint);
                            break;
                        case "Resource":                           
                            Resource __resource = __hit.GetComponent<Resource>();
                            __distance = Vector3.Distance(_dictEntity[_currentSelectedUnit.GetEntityID()].GetEntityPosition(), __resource.transform.position);
                            __callbackFinishCommand = delegate
                            {
                                __callbackFinishCommand = null;
                                GameObject __entityObject = _dictEntity[_currentSelectedUnit.GetEntityID()].gameObject;
                                GameObject __resourceObject = __resource.gameObject;
                                CmdGatherResource(__entityObject, __resourceObject);
                            };

                            if (__distance < _dictEntity[_currentSelectedUnit.GetEntityID()].GetRange())
                            {
                                __callbackFinishCommand();
                            }
                            else
                            {
                                _dictEntity[_currentSelectedUnit.GetEntityID()].SetCurrentCommandFinishCallback(__callbackFinishCommand);
                                CmdMoveEntity(_currentSelectedUnit.gameObject, p_inputInfo.worldClickPoint);
                            }
                            break;
                    }
                }
            }
        }
    }

    public void ExecuteTargetUnitCommnad(string p_entityID, CommandType p_commandType, params object[] p_args)
    {
        Debug.Log("ExecuteTargetUnitCommnad: " + p_entityID);
        Debug.Log("_dictEntity.Count: " + _dictEntity.Count);
        foreach (var k in _dictEntity)
        {
            Debug.Log("entity id: " + k.Key + " |");
        }
        switch (p_commandType)
        {
            case CommandType.BUILD:
                if (_dictEntity[p_entityID].GetEntityType() == EntityType.BUILDING)
                {                    
                    TrySpawnEntity(p_entityID, p_args[0].ToString(), _dictEntity[p_entityID].GetEntityPosition());
                }
                else if (_dictEntity[p_entityID].GetEntityType() == EntityType.UNIT)
                {
                    Debug.Log("Spawn Fake Entity"); 
                    if (_isSpawningEntityBuilding == false)
                    {
                        _fakeEntity = PoolManager.instance.Spawn(PoolType.FAKE_ENTITY).GetComponent<FakeEntity>();
                        _fakeEntity.AInitialize(p_entityID, p_args[0].ToString());
                        _isSpawningEntityBuilding = true;    
                    }
                }             
                break;
            case CommandType.MOVE:
                break;
        }
    }

    private void TrySpawnEntity(string p_builderEntityID, string p_entityToSpawn, Vector3 p_targetPosition)
    {
        int __resourceCost = DataManager.instance.GetEntityVO(p_entityToSpawn).resourceCost;
        if (_dictResourcesAmount[ResourceType.CRYSTAL] >= __resourceCost)
        {
            _dictResourcesAmount[ResourceType.CRYSTAL] -= __resourceCost;
            Vector3 __position = _dictEntity[p_builderEntityID].GetEntityPosition();
            _dictEntity[p_builderEntityID].AddEntityToSpawnList(p_entityToSpawn, __position);
            if (onRequestUpdateResourcesUI != null)
                onRequestUpdateResourcesUI(_dictResourcesAmount[ResourceType.CRYSTAL]);

        }
    }

    private Entity SpawnEntity(NetworkInstanceId p_id, string p_entitySpecificType, Vector3 p_position)
    {
        EntityVO __entityVO = DataManager.instance.GetEntityVO(p_entitySpecificType);
        Debug.Log("SpawnUnit: " + p_entitySpecificType);
        if (__entityVO != null)
        {
            bool __isBuilding = (__entityVO.entityType == EntityType.BUILDING) ? true : false;
            Entity __entity = PoolManager.instance.Spawn(PoolType.ENTITY, transform, __isBuilding).GetComponent<Entity>();
            __entity.transform.SetParent(transform);
            __entity.transform.position = p_position;
            
            __entity.parentNetId = p_id;
            NetworkServer.Spawn(__entity.gameObject);
            __entity.RpcInitialize(Guid.NewGuid().ToString(), _playerColor, __entityVO.entitySpecificType);
            RpcAddEntityToDict(__entity.gameObject);
            RpcListenEntityEvents(__entity.gameObject);
            return __entity;    
        }
        return null;
    }

    #region Do On Clients Command
    [ClientRpc]
    private void RpcAddEntityToDict(GameObject p_entity)
    {
        if (_dictEntity == null)
        {
            _dictEntity = new Dictionary<string, Entity>();
        }
        _dictEntity.Add(p_entity.GetComponent<Entity>().GetEntityID(), p_entity.GetComponent<Entity>());
    }

    [ClientRpc]
    private void RpcMoveEntity(GameObject p_entity, Vector3 p_targetPosition)
    {
        Entity __entity = p_entity.GetComponent<Entity>();

        if (__entity == null)
            return;

        __entity.MoveTo(p_targetPosition);
    }

    [ClientRpc]
    private void RpcAttackEntity(GameObject p_entity, GameObject p_other)
    {
        Entity __entity = p_entity.GetComponent<Entity>();

        if (__entity == null)
            return;

        Entity __other = p_other.GetComponent<Entity>();

        if (__other == null)
            return;

        __entity.AttackEntity(__other);
    }
    #endregion

    #region Do On Server Commands
    [Command]
    private void CmdMoveEntity(GameObject p_entity, Vector3 p_targetPosition)
    {
        RpcMoveEntity(p_entity, p_targetPosition);
    }

    [Command]
    private void CmdGatherResource(GameObject p_entity, GameObject p_resource)
    {
        Entity __entity = p_entity.GetComponent<Entity>();

        if (__entity == null)
            return;

        Resource __resource = p_resource.GetComponent<Resource>();

        if (__resource == null)
            return;

        __entity.TakeResource(__resource);
    }

    [Command]
    private void CmdAttackEntity(GameObject p_entity, GameObject p_other)
    {
        RpcAttackEntity(p_entity, p_other);        
    }
    #endregion
}



//[Command]
//void CmdFire(Vector3 origin, Vector3 direction)
//{
//    shotEffects.PlayShotEffects();
//    //transform.rotation;
//    // Create the Bullet from the Bullet Prefab
//    GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, weaponsLocation.transform.rotation);
//    //do things
//    bullet.GetComponent<Bullet>().setPlayerParent(this);
//    // Add velocity to the bullet
//    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletVelocity;

//    // Spawn the bullet on the Clients
//    NetworkServer.Spawn(bullet);

//    // Destroy the bullet after 2 seconds
//    Destroy(bullet, 10.0f);
//}