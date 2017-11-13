using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public enum IAType
{
    BOSS_1 = 0
}

public class EntityIA : Entity
{
    public IAType IAType;
    private Entity _currentTarget;
    
    void Start () 
    {
        __entityVO = DataManager.instance.GetEntityVO(IAType.ToString());

        SetEntityName(__entityVO.entityName);
        SetEntityColor(Color.yellow);
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

        _navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        _navMeshAgent.speed = __entityVO.speed;
        _navMeshAgent.angularSpeed = 360f;
        _navMeshAgent.acceleration = 10f;
    }

}
