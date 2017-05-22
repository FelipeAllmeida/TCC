using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    #region Singleton
    private static DataManager _instance;
    public static DataManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataManager();
            }
            return _instance;
        }
    }
    #endregion

    #region Private Data
    private Dictionary<string, EntityVO> _dictEntityVO;

    private LocalConnector _localConnector;

    #region DAO
    private EntityDAO _entityDAO;
    #endregion


    #endregion

    public void AInitialize(Action p_callbackFinish)
    {
        _localConnector = new LocalConnector();
        _localConnector.AInitialize();
        InitializeEntityDAO(p_callbackFinish);        
    }

    #region EntityDAO

    private void InitializeEntityDAO(Action p_callbackFinish)
    {
        _entityDAO = new EntityDAO();
        _entityDAO.AInitialize();
        InitializeListEntitiesVO(p_callbackFinish);
    }

    private void InitializeListEntitiesVO(Action p_callbackFinish)
    {
        _entityDAO.GetListAllEntities(delegate (List<EntityVO> p_listEntitiesVO)
        {
            _dictEntityVO = new Dictionary<string, EntityVO>();
            for (int i = 0;i < p_listEntitiesVO.Count;i++)
            {
                EntityVO __entityVO = p_listEntitiesVO[i];
                _dictEntityVO.Add(__entityVO.entitySpecificType, __entityVO);
            }
            if (p_callbackFinish != null) p_callbackFinish();
        });
    }

    public EntityVO GetEntityVO(string p_entitySpecificType)
    {
        return (_dictEntityVO.ContainsKey(p_entitySpecificType) == true) ? _dictEntityVO[p_entitySpecificType] : null;
    }

    #endregion
}
