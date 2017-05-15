using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
    #region Event Data
    public event Action<Entity> onRequestShowSelectUnitUI;
    #endregion

    #region Private Data
    private Dictionary<int, Entity> _dictEntity;
    private Entity _currentSelectedUnit;
    private FakeEntity _fakeEntity;

    private int _team;
    private int _idCounter = 0;
    private bool _isSpawningEntityBuilding = false;

    // private List<Unit> _listSelectedUnits = new List<Unit>();
    #endregion

    public void Initialize(int p_team, Vector3 p_startPosition)
    {
        _team = p_team;
        #region TEST ONLY
        CreateNewEntity("BUILDING_CENTER", Vector3.zero);
        #endregion
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

    private void CreateNewEntity(string p_entitySpecificType, Vector3 p_position)
    {
        Debug.Log("Create New Entity -> " + p_entitySpecificType + ": " + p_entitySpecificType);
        Entity __entity = SpawnUnit(p_entitySpecificType);
        __entity.Initialize(_idCounter++, _team, DataManager.instance.GetEntityVO(p_entitySpecificType));
        __entity.transform.position = p_position;
        ListenEntityEventsBuilding(__entity);
        WorldManager.AddEntityToFloor(__entity.GetCurrentFloor(), __entity);
        AddEntityToDict(__entity);
    }

    private void ListenEntityEventsBuilding(Entity p_entity)
    {
        p_entity.onRequestCreateEntity += CreateNewEntity;
    }

    public void HandleMouseLeftClick(InputInfo p_inputInfo)
    {
        if (_isSpawningEntityBuilding == true)
        {
            Vector3 __buildPos = _fakeEntity.transform.position;
            float __distance = Vector3.Distance(_dictEntity[_fakeEntity.GetConstructorID()].GetEntityPosition(), __buildPos);
            Action __callbackBuildEntity = delegate
            {
                Debug.Log("__callbackBuildEntity");
                _dictEntity[_fakeEntity.GetConstructorID()].AddUnitToSpawnList(_fakeEntity.GetEntityType());
            };
            PoolManager.instance.Despawn(PoolType.FAKE_ENTITY, _fakeEntity.gameObject);
            _isSpawningEntityBuilding = false;
            Debug.Log("Distance: " + __distance);
            if (__distance < _dictEntity[_fakeEntity.GetConstructorID()].GetRange())
            {
                Debug.Log("(__distance < _dictEntity[_fakeEntity.GetConstructorID()].GetRange()");
                __callbackBuildEntity();
            }
            else
            {
                Debug.Log("else");
                (_dictEntity[_fakeEntity.GetConstructorID()] as EntityUnit).MoveTo(__buildPos, __callbackBuildEntity);
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
                if (_currentSelectedUnit.GetUnitTeam() == _team)
                {
                    if (_currentSelectedUnit.GetEntityType() == EntityType.UNIT)
                    {
                        if (p_inputInfo.hit != null && (p_inputInfo.hit.tag == "Entity" || p_inputInfo.hit.tag == "Ground"))
                        {
                            (_currentSelectedUnit as EntityUnit).MoveTo(p_inputInfo.worldClickPoint);                
                        }
                    }
                }
            }
        }
    }

    public void ExecuteTargetUnitCommnad(int p_unitID, CommandType p_commandType, params object[] p_args)
    {
        switch (p_commandType)
        {
            case CommandType.BUILD:
                if (_dictEntity[p_unitID].GetEntityType() == EntityType.BUILDING)
                {
                    (_dictEntity[p_unitID] as EntityBuilding).AddUnitToSpawnList(p_args[0].ToString());
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

    private void AddEntityToDict(Entity p_entity)
    {
        if (_dictEntity == null)
        {
            _dictEntity = new Dictionary<int, Entity>();
        }

        _dictEntity.Add(p_entity.GetEntityID(), p_entity);
    }

    private Entity SpawnUnit(string p_entityType)
    {
        switch (p_entityType)
        {
            case "UNIT_WORKER":
                return PoolManager.instance.Spawn(PoolType.ENTITY_UNIT_WORKER, null, false).GetComponent<Entity>();
            case "BUILDING_CENTER":
                return PoolManager.instance.Spawn(PoolType.ENTITY_BUILDING_CENTER, null, true).GetComponent<Entity>();
            default:
                return null;
        }
    }
}
