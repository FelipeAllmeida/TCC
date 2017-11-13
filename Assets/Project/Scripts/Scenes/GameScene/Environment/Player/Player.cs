using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Player : MonoBehaviour
{
    #region Event Data
    public event Action<bool, Entity> onRequestShowSelectUnitUI;
    public event Action<int> onRequestUpdateResourcesUI;
    #endregion

    #region Public Data
    public PhotonView photonView;
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
    private Entity _currentSelectedEntity;
    private FakeEntity _fakeEntity;

    private string _playerName;
    private Color _playerColor;

    private int _team;
    private int _idCounter = 0;

    [SerializeField] private int _resources;

    private bool _isSpawningEntityBuilding = false;

    // private List<Unit> _listSelectedUnits = new List<Unit>();
    #endregion

    [PunRPC]
    public void RPC_Initialize(int p_team, float[] p_arrayColor, Vector3 p_startPosition)
    {
        _playerName = PhotonNetwork.playerName;
        _team = p_team;
        _playerColor = p_arrayColor.ToColor();
        InitializeDictResources();

        if (photonView.isMine)
        {
            CreateEntity("BUILDING_CENTER", team, _playerColor, p_startPosition);

            CreateEntity("UNIT_WORKER", team, _playerColor, p_startPosition - Vector3.forward);
            CreateEntity("UNIT_WORKER", team, _playerColor, p_startPosition + Vector3.left - Vector3.forward);
            CreateEntity("UNIT_WORKER", team, _playerColor, p_startPosition + Vector3.right - Vector3.forward);
        }
    }

    private void InitializeDictResources()
    {
        _dictResourcesAmount.Add(ResourceType.CRYSTAL, 200);

        if (onRequestUpdateResourcesUI != null) onRequestUpdateResourcesUI(_dictResourcesAmount[ResourceType.CRYSTAL]);
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

    #region InputManager Handlers
    public void HandleMouseLeftClick(InputInfo p_inputInfo)
    {
        GameObject __hit = p_inputInfo.hit;
        if (_isSpawningEntityBuilding == true)
        {
            if (_currentSelectedEntity != null)
            {
                Vector3 __buildPos = p_inputInfo.worldClickPoint;
                float __distance = Vector3.Distance(_dictEntity[_fakeEntity.GetConstructorID()].GetEntityPosition(), __buildPos);
                Action __callbackBuildEntity = () =>
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
                    //photonView.RPC("RPC_MoveEntity", PhotonTargets.All, _dictEntity[_fakeEntity.GetConstructorID()].photonView.viewID, __buildPos);
                    MoveEntity(_dictEntity[_fakeEntity.GetConstructorID()], __buildPos);
                }
            }
        }
        else
        {
            switch (__hit.tag)
            {
                case "Entity":
                    if (_currentSelectedEntity != null)
                    {
                        _currentSelectedEntity.SetSelected(false);
                    }
                    _currentSelectedEntity = __hit.GetComponent<Entity>();
                    _currentSelectedEntity.SetSelected(true);
                    bool __isPlayer = (_currentSelectedEntity.GetTeam() == team) ? true : false;
                    if (onRequestShowSelectUnitUI != null)
                        onRequestShowSelectUnitUI(__isPlayer, _currentSelectedEntity);
                    break;
                case "Resource":
                    break;
                case "Ground":
                    if (_currentSelectedEntity != null)
                    {
                        _currentSelectedEntity.SetSelected(false);
                    }
                    _currentSelectedEntity = null;
                    if (onRequestShowSelectUnitUI != null)
                        onRequestShowSelectUnitUI(false, _currentSelectedEntity);

                    break;
            }
        }
    }

    public void HandleMouseRightClick(InputInfo p_inputInfo)
    {
        GameObject __hit = p_inputInfo.hit;

        if (__hit == null)
            return;

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
            if (_currentSelectedEntity != null)
            {
                if (_currentSelectedEntity.GetTeam() == _team)
                {
                    float __distance;
                    Action __callbackFinishCommand;
                    switch (__hit.tag)
                    {
                        case "Entity":
                            Entity __entity = __hit.GetComponent<Entity>();
                            if (__entity.GetTeam() == _currentSelectedEntity.GetTeam())
                            //ALLY
                            {
                                if (_currentSelectedEntity.GetEntityType() == EntityType.UNIT)
                                {
                                    if (p_inputInfo.hit != null && p_inputInfo.hit.tag == "Entity")
                                    {
                                        MoveEntity(_currentSelectedEntity, p_inputInfo.worldClickPoint);
                                    }
                                }
                            }
                            else
                            //ENEMY
                            {
                                Debug.Log("Attack Enemy");
                                __distance = Vector3.Distance(_dictEntity[_currentSelectedEntity.GetEntityID()].GetEntityPosition(), __entity.GetEntityPosition());
                                GameObject __entityObject = _dictEntity[_currentSelectedEntity.GetEntityID()].gameObject;
                                GameObject __targetEntityObject = __hit.gameObject;
                                __callbackFinishCommand = () =>
                                {
                                    __callbackFinishCommand = null;
                                    AttackEntity(__entityObject, __targetEntityObject);
                                };
                                if (__distance < _dictEntity[_currentSelectedEntity.GetEntityID()].GetRange())
                                {
                                    __callbackFinishCommand();
                                }
                                else
                                {
                                    _dictEntity[_currentSelectedEntity.GetEntityID()].SetCurrentCommandFinishCallback(__callbackFinishCommand);
                                    MoveEntity(_dictEntity[_currentSelectedEntity.GetEntityID()], __entity.GetEntityPosition());
                                }
                            }
                            break;
                        case "Ground":
                            MoveEntity(_currentSelectedEntity, p_inputInfo.worldClickPoint);
                            break;
                        case "Resource":
                            Resource __resource = __hit.GetComponent<Resource>();
                            __distance = Vector3.Distance(_dictEntity[_currentSelectedEntity.GetEntityID()].GetEntityPosition(), __resource.transform.position);
                            __callbackFinishCommand = () =>
                            {
                                __callbackFinishCommand = null;
                                GatherResource(_currentSelectedEntity, __resource);
                            };

                            if (__distance < _dictEntity[_currentSelectedEntity.GetEntityID()].GetRange())
                            {
                                __callbackFinishCommand();
                            }
                            else
                            {
                                _dictEntity[_currentSelectedEntity.GetEntityID()].SetCurrentCommandFinishCallback(__callbackFinishCommand);
                                MoveEntity(_currentSelectedEntity, p_inputInfo.worldClickPoint);
                            }
                            break;
                    }
                }
            }
        }
    }
    #endregion

    private void HandleRequestCreateNewEntity(string p_entitySpecificType, int p_team, Color p_color, Vector3 p_startPos)
    {
        CreateEntity(p_entitySpecificType, p_team, p_color, p_startPos);
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

    private Entity CreateEntity(string p_entitySpecificType, int p_team, Color p_playerColor, Vector3 p_position)
    {
        EntityVO __entityVO = DataManager.instance.GetEntityVO(p_entitySpecificType);

        if (__entityVO != null)
        {
            //new logic
            bool __isBuilding = (__entityVO.entityType == EntityType.BUILDING) ? true : false;

            Entity __entity = PoolManager.PhotonSpawn(PoolType.ENTITY, p_position, photonView.viewID, __isBuilding).GetComponent<Entity>();
            __entity.photonView.RPC("RPC_Initialize", PhotonTargets.All, Guid.NewGuid().ToString(), p_team, p_playerColor.ToArray(), __entityVO.entitySpecificType);

            //old logic
            //Entity __entity = PoolManager.instance.Spawn(PoolType.ENTITY, transform, __isBuilding).GetComponent<Entity>();
            //__entity.transform.SetParent(transform);
            //__entity.transform.position = p_position;

            ////__entity.parentNetId = p_id;
            //NetworkServer.Spawn(__entity.gameObject);
            //__entity.RPC_Initialize(Guid.NewGuid().ToString(), _playerColor, __entityVO.entitySpecificType);
            photonView.RPC("RPC_AddEntityToDict", PhotonTargets.All, __entity.photonView.viewID);
            return __entity;    
        }
        return null;
    }

    private void ListenEntityEvents(Entity p_entity)
    {
        p_entity.onRequestCreateEntity += HandleRequestCreateNewEntity;
        p_entity.onAddResource += HandleAddResource;

        p_entity.onDeath += delegate
        {
            if (_currentSelectedEntity == p_entity)
                if (onRequestShowSelectUnitUI != null) onRequestShowSelectUnitUI(false, null);
            DestroyEntity(p_entity);
        };
    }

    #region Do On Clients Command

    [PunRPC]
    private void RPC_AddEntityToDict(int p_viewID)
    {
        Entity __entity = PhotonView.Find(p_viewID).gameObject.GetComponent<Entity>();

        if (_dictEntity == null)
        {
            _dictEntity = new Dictionary<string, Entity>();
        }
        _dictEntity.Add(__entity.GetEntityID(), __entity);
        ListenEntityEvents(__entity);
    }

    private void MoveEntity(Entity p_entity, Vector3 p_targetPosition)
    {
        p_entity.MoveTo(p_targetPosition);
    }

    private void AttackEntity(GameObject p_entity, GameObject p_other)
    {
        Debug.Log("RpcAttackEntity");
        Entity __entity = p_entity.GetComponent<Entity>();

        if (__entity == null)
            return;

        Entity __other = p_other.GetComponent<Entity>();

        if (__other == null)
            return;

        __entity.AttackEntity(__other);
    }

    //[Server]
    //public void RpcSpawnResource(GameObject p_resource, Vector3 p_position)
    //{
    //    Resource __resource = Instantiate(p_resource, p_position, new Quaternion(1f, 1f, 1f, 1f)).GetComponent<Resource>();
    //    __resource.Initialize(UnityEngine.Random.Range(80, 290));
    //    __resource.transform.position = p_position;
    //    NetworkServer.Spawn(__resource.gameObject);
    //}

    //[Command]
    //public void CmdSpawnResource(GameObject p_resource, Vector3 p_position)
    //{
    //    Resource __resource = Instantiate(p_resource, p_position, new Quaternion(1f, 1f, 1f, 1f)).GetComponent<Resource>();
    //    __resource.Initialize(UnityEngine.Random.Range(80, 290));
    //    __resource.transform.position = p_position;
    //    NetworkServer.Spawn(__resource.gameObject);
    //}

    //[Server]
    //public void DespawnResource(GameObject p_resource)
    //{
    //    NetworkServer.Destroy(p_resource);
    //}
    #endregion

    #region Do On Server Commands
    private void DestroyEntity(Entity p_entity) // p_viewID)
    {
        PhotonUtility.Destroy(p_entity.gameObject);
        //photonView.RPC("RPC_DestroyEntity", PhotonTargets.MasterClient, p_viewID);
    }

    [PunRPC]
    private void RPC_DestroyEntity(int p_viewID)
    {
        PhotonView __photonView = PhotonView.Find(p_viewID);
        if (__photonView != null)
        {
            PhotonUtility.Destroy(__photonView.gameObject);
        }
    }

    private void GatherResource(Entity p_entity, Resource p_resource)
    {
        if (p_entity == null)
            return;

        if (p_resource == null)
            return;

        p_entity.TakeResource(p_resource);
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