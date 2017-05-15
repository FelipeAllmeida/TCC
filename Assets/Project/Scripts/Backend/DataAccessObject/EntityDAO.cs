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

        string[] __arraySelectKeys = new string[] { "entityName", "type", "entityType", "maxHealth", "speed", "range", "timeToSpawn", "resourceCost" };

        Dictionary<string, string> __dictWhere = new Dictionary<string, string>();
        __dictWhere.Add("1", "1");

        SelectDataFromTableAsync(SelectType.NONE, true, __table, __arraySelectKeys, __dictWhere, delegate (List<Dictionary<string, string>> p_listData)
        {
            List<EntityVO> __listEntityVO = new List<EntityVO>();

            

            for (int i = 0; i < p_listData.Count;i++)
            {
                EntityVO __newEntityVO = new EntityVO();

                __newEntityVO.entityName = p_listData[i]["entityName"];
                __newEntityVO.entityType = (EntityType)Enum.Parse(typeof(EntityType), p_listData[i]["type"]);
                __newEntityVO.entitySpecificType = p_listData[i]["entityType"];       
                __newEntityVO.maxHealth = float.Parse(p_listData[i]["maxHealth"]);
                __newEntityVO.speed = float.Parse(p_listData[i]["speed"]);
                __newEntityVO.range = float.Parse(p_listData[i]["range"]);
                __newEntityVO.timeToSpawn = float.Parse(p_listData[i]["timeToSpawn"]);
                __newEntityVO.resourceCost = Int32.Parse(p_listData[i]["resourceCost"]);

                __newEntityVO.listAvaiableCommands = GetListCommandByEntity(__newEntityVO.entitySpecificType);
                __newEntityVO.listAvaiableEntitiesToBuild = GetListAvaiableEntitiesToBuildByEntity(__newEntityVO.entitySpecificType);

                __listEntityVO.Add(__newEntityVO);
            }
            if (p_callbackFinish != null) p_callbackFinish(__listEntityVO);
        });
    }

    public List<CommandType> GetListCommandByEntity(string p_entitySpecificType)  
    {
        string __table = "EntityCommand";

        string[] __arraySelectKeys = new string[] { "command"};

        Dictionary<string, string> __dictWhere = new Dictionary<string, string>();
        __dictWhere.Add("entityType", p_entitySpecificType);

        List<Dictionary<string, string>> __listData =  SelectDataFromTable(SelectType.NONE, false, __table, __arraySelectKeys, __dictWhere);
        List<CommandType> __listCommands = new List<CommandType>();

        for (int i = 0;i < __listData.Count;i++)
        {
            CommandType __commandType = (CommandType)Enum.Parse(typeof(CommandType), __listData[i]["command"]);
            __listCommands.Add(__commandType);
        }

        return __listCommands;
    }

    public List<string> GetListAvaiableEntitiesToBuildByEntity(string p_entitySpecificType)
    {
        string __table = "EntityBuild";

        string[] __arraySelectKeys = new string[] { "build" };

        Dictionary<string, string> __dictWhere = new Dictionary<string, string>();
        __dictWhere.Add("entityType", p_entitySpecificType);

        List<Dictionary<string, string>> __listData = SelectDataFromTable(SelectType.NONE, false, __table, __arraySelectKeys, __dictWhere);
        List<string> __listAvaiableEntitiesToBuild = new List<string>();

        for (int i = 0; i < __listData.Count;i++)
        {
            string __buildingType =  __listData[i]["build"];
            __listAvaiableEntitiesToBuild.Add(__buildingType);
        }

        return __listAvaiableEntitiesToBuild;
    }
}