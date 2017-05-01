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
    private bool _isBuildingEntity = false;

    // private List<Unit> _listSelectedUnits = new List<Unit>();
    #endregion

    public void Initialize(int p_team, Vector3 p_startPosition)
    {
        _team = p_team;
        CreateEntityCenterBuilding(p_startPosition);
    }

    public void AUpdate()
    {
        if (_isBuildingEntity == true)
        {
            _fakeEntity.AUpdate();
        }
    }

    private void CreateEntityCenterBuilding(Vector3 p_position)
    {
        EntityBuilding __unitCenter = PoolManager.instance.Spawn(PoolType.ENTITY_BUILDING_CENTER, null, true).GetComponent<EntityBuilding>();
        CreateNewEntity(__unitCenter, Vector3.zero);
    }

    private void CreateNewEntity(Entity p_entity, Vector3 p_position)
    {
        p_entity.Initialize(_idCounter++, _team);
        if (p_entity.GetEntityType() == EntityType.BUILDING)
        {
            p_position.y += p_entity.GetComponent<Renderer>().bounds.extents.y;
        }
        p_entity.transform.position = p_position;
        WorldManager.AddEntityToFloor(p_entity.GetCurrentFloor(), p_entity);
        AddEntityToDict(p_entity);
    }

    public void HandleMouseLeftClick(InputInfo p_inputInfo)
    {
        if (_isBuildingEntity == true)
        {

        }
        else
        {
            _currentSelectedUnit = p_inputInfo.hit.GetComponent<Entity>();

            if (onRequestShowSelectUnitUI != null) onRequestShowSelectUnitUI(_currentSelectedUnit);
        }
    }

    public void HandleMouseRightClick(InputInfo p_inputInfo)
    {
        if (_isBuildingEntity == true)
        {
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
                if (p_args[0] is EntityUnitType)
                {
                    _fakeEntity = PoolManager.instance.Spawn(PoolType.FAKE_ENTITY_UNIT_WORKER).GetComponent<FakeEntity>();
                    _isBuildingEntity = true;
                }
                else if (p_args[0] is EntityBuildingType)
                {
                
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
}
