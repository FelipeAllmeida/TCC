using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVO  
{
    public string entityName;
    public EntityType entityType;
    public string entitySpecificType;

    public float maxHealth;
    public float speed;
    public float range;
    public float damage;
    public float attackSpeed;
    public float timeToSpawn;

    public Vector3 size;

    public int resourceCapacity;
    public int resourceCost;

    public List<CommandType> listAvaiableCommands;

    public List<string> listAvaiableEntitiesToBuild = null;


}
