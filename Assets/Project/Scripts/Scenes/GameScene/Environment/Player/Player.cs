using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    #region Event Data
    public event Action<Entity> onRequestShowSelectUnitUI;
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
    private Entity _currentSelectedUnit;
    private FakeEntity _fakeEntity;

    private string _playerName;
    private int _team;
    private int _idCounter = 0;

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
        CmdCreateNewEntity(netId, "BUILDING_CENTER", p_startPosition);
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
        SpawnUnit(p_id, p_entitySpecificType, p_position);        
    }

    [ClientRpc]
    private void RpcListenEntityEvents(GameObject p_entity)
    {
        p_entity.GetComponent<Entity>().onRequestCreateEntity += HandleCmdCreateNewEntity;
    }

    private void HandleCmdCreateNewEntity(string p_entitySpecificType, Vector3 p_startPos)
    {
        CmdCreateNewEntity(netId, p_entitySpecificType, p_startPos);
    }

    public void HandleMouseLeftClick(InputInfo p_inputInfo)
    {
        if (_isSpawningEntityBuilding == true)
        {
            Vector3 __buildPos = _fakeEntity.transform.position;
            float __distance = Vector3.Distance(_dictEntity[_fakeEntity.GetConstructorID()].GetEntityPosition(), __buildPos);
            Action __callbackBuildEntity = delegate
            {
                _dictEntity[_fakeEntity.GetConstructorID()].AddUnitToSpawnList(_fakeEntity.GetEntityType(), __buildPos);
            };
            PoolManager.instance.Despawn(PoolType.FAKE_ENTITY, _fakeEntity.gameObject);
            _isSpawningEntityBuilding = false;
            if (__distance < _dictEntity[_fakeEntity.GetConstructorID()].GetRange())
            {
                __callbackBuildEntity();
            }
            else
            {
                _dictEntity[_fakeEntity.GetConstructorID()].SetEntityReachedDestinationCallback(__callbackBuildEntity);
                CmdMoveEntity(_dictEntity[_fakeEntity.GetConstructorID()].gameObject, __buildPos);
            }
        }
        else
        {
            _currentSelectedUnit = p_inputInfo.hit.GetComponent<Entity>();

            if (onRequestShowSelectUnitUI != null) onRequestShowSelectUnitUI(_currentSelectedUnit);
        }
    }

    public void HandleMouseRightClick(InputInfo p_inputInfo)
    {
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
                Debug.Log("entity team: " + _currentSelectedUnit.GetUnitTeam() + " | player team: " + _team);
                if (_currentSelectedUnit.GetUnitTeam() == _team)
                {
                    if (_currentSelectedUnit.GetEntityType() == EntityType.UNIT)
                    {
                        if (p_inputInfo.hit != null && (p_inputInfo.hit.tag == "Entity" || p_inputInfo.hit.tag == "Ground"))
                        {
                            CmdMoveEntity(_currentSelectedUnit.gameObject, p_inputInfo.worldClickPoint);
                        }
                    }
                }
            }
        }
    }

    public void ExecuteTargetUnitCommnad(string p_unitID, CommandType p_commandType, params object[] p_args)
    {
        Debug.Log("ExecuteTargetUnitCommnad: " + p_unitID);
        Debug.Log("_dictEntity.Count: " + _dictEntity.Count);
        foreach (var k in _dictEntity)
        {
            Debug.Log("entity id: " + k.Key + " |");
        }
        switch (p_commandType)
        {
            case CommandType.BUILD:
                if (_dictEntity[p_unitID].GetEntityType() == EntityType.BUILDING)
                {
                    _dictEntity[p_unitID].AddUnitToSpawnList(p_args[0].ToString(), _dictEntity[p_unitID].GetEntityPosition());
                }
                else if (_dictEntity[p_unitID].GetEntityType() == EntityType.UNIT)
                {
                    Debug.Log("Spawn Fake Entity"); 
                    if (_isSpawningEntityBuilding == false)
                    {
                        _fakeEntity = PoolManager.instance.Spawn(PoolType.FAKE_ENTITY).GetComponent<FakeEntity>();
                        _fakeEntity.AInitialize(p_unitID, p_args[0].ToString());
                        _isSpawningEntityBuilding = true;    
                    }
                }             
                break;
            case CommandType.MOVE:
                break;
        }
    }


    private Entity SpawnUnit(NetworkInstanceId p_id, string p_entitySpecificType, Vector3 p_position)
    {
        EntityVO __entityVO = DataManager.instance.GetEntityVO(p_entitySpecificType);
        Debug.Log("SpawnUnit: " + p_entitySpecificType);
        if (__entityVO != null)
        {
            bool __isBuilding = (__entityVO.entityType == EntityType.BUILDING) ? true : false;
            Debug.Log("SpawnUnit is Building: " + __isBuilding);
            Entity __entity = PoolManager.instance.Spawn(PoolType.ENTITY, transform, __isBuilding).GetComponent<Entity>();
            Debug.Log("Who im I: " + gameObject.name);
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

    #endregion

    #region Do On Server Commands
    [Command]
    private void CmdMoveEntity(GameObject p_entity, Vector3 p_targetPosition)
    {
        Entity __entity = p_entity.GetComponent<Entity>();

        if (__entity == null) return;

        __entity.MoveTo(p_targetPosition);
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