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
    private int _team;
    private Entity _currentSelectedUnit;
    private int _idCounter = 0;
    // private List<Unit> _listSelectedUnits = new List<Unit>();
    #endregion

    public void Initialize(int p_team, Vector3 p_startPosition)
    {
        _team = p_team;
        CreateEntityCenterBuilding(p_startPosition);
    }

    private void CreateEntityCenterBuilding(Vector3 p_position)
    {
        EntityBuilding __unitCenter = PoolManager.instance.Spawn(PoolType.CENTRAL_UNIT).GetComponent<EntityBuilding>();
        CreateNewEntity(__unitCenter, Vector3.zero);
    }

    private void CreateNewEntity(Entity p_entity, Vector3 p_position)
    {
        p_entity.Initialize(_idCounter++, _team);
        p_entity.transform.position = p_position;
        WorldManager.AddEntityToFloor(p_entity.GetCurrentFloor(), p_entity);
        AddEntityToDict(p_entity);
    }

    public void HandleMouseLeftClick(InputInfo p_inputInfo)
    {
        _currentSelectedUnit = p_inputInfo.hit;

        if (onRequestShowSelectUnitUI != null) onRequestShowSelectUnitUI(_currentSelectedUnit);
    }

    public void HandleMouseRightClick(InputInfo p_inputInfo)
    {
        if (_currentSelectedUnit != null)
        {
            if (_currentSelectedUnit.GetUnitTeam() == _team)
            {
                _currentSelectedUnit.MoveTo(p_inputInfo.worldClickPoint);
            }
        }
    }

    public void ExecuteTargetUnitCommnad(int p_unitID, CommandType p_commandType, params object[] p_args)
    {
        switch (p_commandType)
        {
            case CommandType.BUILD:
                
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
