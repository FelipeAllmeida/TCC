using System;
using System.Collections;
using System.Collections.Generic;

public class EntityDAO : DataAccessObject
{
    public override void AInitialize()
    {
        base.AInitialize();
    }

    public void GetListAllEntities(Action<List<EntityVO>> p_callbackFinish)
    {
        string __table = "Entity";

        string[] __arraySelectKeys = new string[] { "entityName", "type", "maxHealth", "timeToSpawn", "resourceCost" };

        Dictionary<string, string> __dictWhere = new Dictionary<string, string>();
        __dictWhere.Add("1", "1");

        SelectDataFromTableAsync(SelectType.NONE, true, __table, __arraySelectKeys, __dictWhere, delegate (List<Dictionary<string, string>> p_listData)
        {
            List<EntityVO> __listEntityVO = new List<EntityVO>();
            for (int i = 0;i < p_listData.Count;i++)
            {
                EntityVO __newEntityVO = new EntityVO();

                __newEntityVO.entityName = p_listData[i]["entityName"];
                __newEntityVO.entityType = (EntityType)Enum.Parse(typeof(EntityType), p_listData[i]["type"]);
                __newEntityVO.maxHealth = float.Parse(p_listData[i]["maxHealth"]);
                __newEntityVO.speed = float.Parse(p_listData[i]["speed"]);
                __newEntityVO.timeToSpawn = float.Parse(p_listData[i]["timeToSpawn"]);
                __newEntityVO.resourceCost = Int32.Parse(p_listData[i]["resourceCost"]);

                __listEntityVO.Add(__newEntityVO);
            }
            if (p_callbackFinish != null) p_callbackFinish(__listEntityVO);
        });
    }

    public void GetListCommandByEntity(Action<Dictionary<string, List<CommandType>>> p_callbackFinish)  
    {
    
    }
}