//#define VOX_DEBUG
using System.Collections.Generic;
using System;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;
using Framework;

public abstract class DataAccessObject
{
    #region Enumerators
    public enum SelectType
    {
        MAX,
        MIN,
        NONE
    }
    #endregion
    #region Callback Data
    private Action _callbackCancelDataRequest;
    #endregion
    #region Protected Data
    protected string _baseURL;
    protected string _authorization = "X-Authorization";
    protected string _dateFormat = "yyyy-MM-dd";
    protected string _dateHourFormat = "yyyy-MM-dd HH:mm:ss";
    protected Dictionary<string, string> _headers = new Dictionary<string, string>();
    #endregion
    #region Private Data
#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
    private string _dbName = "punk_hazard.sqdb";
#else
private string _dbName = "temp.sqdb";
#endif
    private string _dbPath;
#if UNITY_STANDALONE_OSX
private string _macDBPath = System.Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/DBData";
#endif
#if UNITY_STANDALONE_WIN
    private string _windowsDBPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/DownTownStudios/PunkHazard/DBData";
#endif
    private IDbConnection _dbConnection;
    private IDbCommand _dbCommand;
    private IDataReader _dbReader;
    private IDbTransaction _dbTransaction;
    private bool _cancelDataRequest;
    #endregion
    #region Initialization
    public virtual void AInitialize()
    {
        _cancelDataRequest = false;
        _callbackCancelDataRequest = null;
        GetDatabasePath();
    }
    #endregion
    #region Utility Methods
    private string BuildQuerySelectCount(string p_tableName, Dictionary<string, string> p_dictWhere)
    {
        string __query = string.Empty;
        int __counter = 0;
        __query = "SELECT COUNT() FROM " + p_tableName + " WHERE ";
        foreach (KeyValuePair<string, string> k_dictWhere in p_dictWhere)
        {
            if (__counter < p_dictWhere.Count - 1)
            {
                __query += "\"" + k_dictWhere.Key + "\" = \"" + k_dictWhere.Value + "\" AND ";
            }
            else
            {
                __query += "\"" + k_dictWhere.Key + "\" = \"" + k_dictWhere.Value + "\"";
            }
            __counter++;
        }
        return __query;
    }
    private string BuildQuerySelect(SelectType p_selectType, bool p_selectAll, string p_tableName, string[] p_arraySelectKeys, Dictionary<string, string> p_dictWhere)
    {
        string __query = string.Empty;
        int __counter = 0;
        __query = "SELECT ";
        if (p_selectType == SelectType.MAX)
        {
            __query += "MAX(";
        }
        else if (p_selectType == SelectType.MIN)
        {
            __query += "MIN(";
        }
        if (p_selectAll == true)
        {
            __query += "*";
        }
        else
        {
            for (int i = 0; i < p_arraySelectKeys.Length; i++)
            {
                if (i < p_arraySelectKeys.Length - 1)
                {
                    __query += p_arraySelectKeys[i] + ", ";
                }
                else
                {
                    __query += p_arraySelectKeys[i];
                    if (p_selectType == SelectType.NONE)
                    {
                        __query += " ";
                    }
                    else
                    {
                        __query += ") ";
                    }
                }
            }
        }
        __query += " FROM " + p_tableName + " WHERE ";
        __counter = 0;
        foreach (KeyValuePair<string, string> k_dictData in p_dictWhere)
        {
            if (__counter < p_dictWhere.Count - 1)
            {
                __query += "\"" + k_dictData.Key + "\" = \"" + k_dictData.Value + "\" AND ";
            }
            else
            {
                __query += "\"" + k_dictData.Key + "\" = \"" + k_dictData.Value + "\"";
            }
            __counter++;
        }
        Debug.Log(__query);
        return __query;
    }
    private string BuildQuerySelect(SelectType p_selectType, string p_tableName, string[] p_arraySelectKeys, Dictionary<string, string[]> p_dictWhere)
    {
        string __query = string.Empty;
        int __counter = 0;
        __query = "SELECT ";
        if (p_selectType == SelectType.MAX)
        {
            __query += "MAX(";
        }
        else if (p_selectType == SelectType.MIN)
        {
            __query += "MIN(";
        }
        for (int i = 0;i < p_arraySelectKeys.Length;i++)
        {
            if (i < p_arraySelectKeys.Length - 1)
            {
                __query += p_arraySelectKeys[i] + ", ";
            }
            else
            {
                __query += p_arraySelectKeys[i];
                if (p_selectType == SelectType.NONE)
                {
                    __query += " ";
                }
                else
                {
                    __query += ") ";
                }
            }
        }
        __query += " FROM " + p_tableName + " WHERE ";
        __counter = 0;
        foreach (KeyValuePair<string, string[]> k_dictData in p_dictWhere)
        {
            if (__counter < p_dictWhere.Count - 1)
            {
                for (int i = 0;i < k_dictData.Value.Length;i++)
                {
                    __query += "\"" + k_dictData.Key + "\" = \"" + k_dictData.Value + "\" AND ";
                }
            }
            else
            {
                for (int i = 0;i < k_dictData.Value.Length;i++)
                {
                    if (i < k_dictData.Value.Length - 1)
                    {
                        __query += "\"" + k_dictData.Key + "\" = \"" + k_dictData.Value + "\" AND ";
                    }
                    else
                    {
                        __query += "\"" + k_dictData.Key + "\" = \"" + k_dictData.Value + "\"";
                    }
                }
            }
            __counter++;
        }
        return __query;
    }
    private string BuildQuerySelect(SelectType p_selectType, bool p_selectAll, List<string> p_listTableName, Dictionary<string, string[]> p_dictTableArraySelectKeys, Dictionary<string, Dictionary<string, string>> p_dictTableWhereKeys)
    {
        string __query = string.Empty;
        __query += "SELECT ";
        if (p_selectType == SelectType.MAX)
        {
            __query += "MAX(";
        }
        else if (p_selectType == SelectType.MIN)
        {
            __query += "MIN(";
        }
        if (p_selectAll == true)
        {
            __query += "*";
        }
        else
        {
            for (int i = 0;i < p_listTableName.Count;i++)
            {
                string __tableName = p_listTableName[i];
                for (int j = 0;j < p_dictTableArraySelectKeys[__tableName].Length;j++)
                {
                    __query += p_dictTableArraySelectKeys[__tableName][j];
                    if (j < p_dictTableArraySelectKeys[__tableName].Length - 1 && i < p_listTableName.Count - 1)
                    {
                        __query += ", ";
                    }
                }
            }
        }
        if (p_selectType == SelectType.NONE)
        {
            __query += " ";
        }
        else
        {
            __query += ") ";
        }
        __query += "FROM ";
        for (int i = 0;i < p_listTableName.Count;i++)
        {
            __query += p_listTableName[i];
            if (i < p_listTableName.Count - 1)
            {
                __query += ", ";
            }
            else
            {
                __query += " ";
            }
        }
        __query += "WHERE ";
        for (int i = 0;i < p_listTableName.Count;i++)
        {
            string __tableName = p_listTableName[i];
            int __counter = 0;
            foreach (KeyValuePair<string, string> k_dictData in p_dictTableWhereKeys[__tableName])
            {
                if (__counter < p_dictTableWhereKeys.Count - 1 && i < p_listTableName.Count - 1)
                {
                    __query += k_dictData.Key + " = \"" + k_dictData.Value + "\" AND ";
                }
                else
                {
                    __query += k_dictData.Key + " = \"" + k_dictData.Value + "\"";
                }
                __counter++;
            }
        }
        return __query;
    }
    private string BuildQueryInsertModel(Dictionary<string, string> p_dictData, string p_tableName)
    {
        string __query = string.Empty;
        int __counter = 0;
        __query += "INSERT INTO " + p_tableName + "(";
        foreach (KeyValuePair<string, string> k_key in p_dictData)
        {
            if (__counter < p_dictData.Count - 1)
            {
                __query += k_key.Key + ", ";
            }
            else
            {
                __query += k_key.Key + ") VALUES (\"";
            }
            __counter++;
        }
        __counter = 0;
        foreach (KeyValuePair<string, string> k_key in p_dictData)
        {
            if (__counter < p_dictData.Count - 1)
            {
                __query += p_dictData[k_key.Key] + "\", \"";
            }
            else
            {
                __query += p_dictData[k_key.Key] + "\")";
            }
            __counter++;
        }
#if VOX_DEBUG
Debug.Log("Insert:" + __query);
#endif
        return __query;
    }
    private string BuildQuerryUpdateModel(Dictionary<string, string> p_dictData, string p_tableName, string[] p_arrayWhereKeys)
    {
        string __query = string.Empty;
        int __counter = 0;
        __query = "UPDATE " + p_tableName + " SET ";
        foreach (KeyValuePair<string, string> k_key in p_dictData)
        {
            if (__counter < p_dictData.Count - 1)
            {
                __query += k_key.Key + " = \"" + k_key.Value + "\", ";
            }
            else
            {
                __query += k_key.Key + " = \"" + k_key.Value + "\" WHERE ";
            }
            __counter++;
        }
        for (int i = 0;i < p_arrayWhereKeys.Length;i++)
        {
            if (i < p_arrayWhereKeys.Length - 1)
            {
                __query += p_arrayWhereKeys[i] + " = \"" + p_dictData[p_arrayWhereKeys[i]] + "\" AND ";
            }
            else
            {
                __query += p_arrayWhereKeys[i] + " = \"" + p_dictData[p_arrayWhereKeys[i]] + "\"";
            }
        }
#if VOX_DEBUG
Debug.Log("Update:" + __query);
#endif
        return __query;
    }
    private string BuildClearTableQuery(string p_tableName)
    {
        string __query = string.Empty;
        __query = "DELETE FROM `" + p_tableName + "` WHERE 1 = 1";
        return __query;
    }
    private string BuildDeleteQuery(Dictionary<string, string> p_dictData, string p_tableName, string[] p_arrayWhereKeys)
    {
        string __query = string.Empty;
        __query = "DELETE FROM " + p_tableName + " WHERE ";
        for (int i = 0;i < p_arrayWhereKeys.Length;i++)
        {
            if (i < p_arrayWhereKeys.Length - 1)
            {
                __query += p_arrayWhereKeys[i] + " = \"" + p_dictData[p_arrayWhereKeys[i]] + "\" AND ";
            }
            else
            {
                __query += p_arrayWhereKeys[i] + " = \"" + p_dictData[p_arrayWhereKeys[i]] + "\"";
            }
        }
        return __query;
    }
    protected void SelectDataFromTable(string p_tableName, string[] p_arraySelectKeys, Dictionary<string, string> p_dictWhere, Action<Dictionary<string, string>> p_callbackFinish)
    {
        Dictionary<string, string> __dictRowLoaded = new Dictionary<string, string>();
        OpenDatabaseConnection();
        using (_dbCommand = _dbConnection.CreateCommand())
        {
            _dbCommand.Connection = _dbConnection;
            _dbCommand.CommandText = BuildQuerySelect(SelectType.NONE, false, p_tableName, p_arraySelectKeys, p_dictWhere);
            try
            {
                _dbReader = _dbCommand.ExecuteReader();
                while (_dbReader.Read())
                {
                    for (int i = 0;i < p_arraySelectKeys.Length;i++)
                    {
                        string __stringRow = _dbReader.GetString(i);
                        __dictRowLoaded.Add(p_arraySelectKeys[i], __stringRow);
                    }
                }
                _dbReader.Close();
            }
            catch (SqliteException __sqliteException)
            {
                Debug.LogError("Sqlite Exception: " + __sqliteException);
            }
        }
        _dbConnection.Close();
        if (p_callbackFinish != null)
            p_callbackFinish(__dictRowLoaded);
    }

    protected void SelectDataFromTableAsync(SelectType p_selectType, bool p_selectAll, string p_tableName, string[] p_arraySelectKeys, Dictionary<string, string> p_dictWhere, Action<List<Dictionary<string, string>>> p_callbackFinish)
    {
        Debug.Log("SelectDataFromTableAsync");
        List<Dictionary<string, string>> __listDataLoaded = new List<Dictionary<string, string>>();
        AThread.StartNewThread(delegate
        {
            OpenDatabaseConnection();
            using (_dbTransaction = _dbConnection.BeginTransaction())
            {
                using (_dbCommand = _dbConnection.CreateCommand())
                {
                    _dbCommand.Connection = _dbConnection;
                    _dbCommand.Transaction = _dbTransaction;
                    _dbCommand.CommandText = BuildQuerySelect(p_selectType, p_selectAll, p_tableName, p_arraySelectKeys, p_dictWhere);
                    try
                    {
                        _dbReader = _dbCommand.ExecuteReader();
                        while (_dbReader.Read())
                        {
                            Dictionary<string, string> __dictRowLoaded = new Dictionary<string, string>();
                            for (int i = 0;i < p_arraySelectKeys.Length;i++)
                            {
                                string __stringRow = string.Empty;
                                if (typeof(DateTime) == _dbReader.GetFieldType(i))
                                {
                                    __stringRow = _dbReader.GetString(i);
                                }
                                else
                                {
                                    __stringRow = _dbReader.GetValue(i).ToString();
                                }
                                __dictRowLoaded.Add(p_arraySelectKeys[i], __stringRow);
                            }
                            __listDataLoaded.Add(__dictRowLoaded);
                        }
                        _dbReader.Close();
                    }
                    catch (Exception __sqliteException)
                    {
                        Debug.LogError("Sqlite Exception: " + __sqliteException);
                    }
                }
            }
            _dbConnection.Close();
        },
        delegate
        {
            if (p_callbackFinish != null) p_callbackFinish(__listDataLoaded);
        });
    }
    protected void SelectGroupOfDataFromTableAsync(SelectType p_selectType, string p_tableName, string[] p_arraySelectKeys, Dictionary<string, string[]> p_dictWhere, Action<List<Dictionary<string, string>>> p_callbackFinish)
    {
        List<Dictionary<string, string>> __listDataLoaded = new List<Dictionary<string, string>>();
        AThread.StartNewThread(delegate
        {
            OpenDatabaseConnection();
            using (_dbTransaction = _dbConnection.BeginTransaction())
            {
                using (_dbCommand = _dbConnection.CreateCommand())
                {
                    _dbCommand.Connection = _dbConnection;
                    _dbCommand.Transaction = _dbTransaction;
                    _dbCommand.CommandText = BuildQuerySelect(p_selectType, p_tableName, p_arraySelectKeys, p_dictWhere);
                    try
                    {
                        _dbReader = _dbCommand.ExecuteReader();
                        while (_dbReader.Read())
                        {
                            Dictionary<string, string> __dictRowLoaded = new Dictionary<string, string>();
                            for (int i = 0;i < p_arraySelectKeys.Length;i++)
                            {
                                string __stringRow = string.Empty;
                                if (typeof(DateTime) == _dbReader.GetFieldType(i))
                                {
                                    __stringRow = _dbReader.GetString(i);
                                }
                                else
                                {
                                    __stringRow = _dbReader.GetValue(i).ToString();
                                }
                                __dictRowLoaded.Add(p_arraySelectKeys[i], __stringRow);
                            }
                            __listDataLoaded.Add(__dictRowLoaded);
                        }
                        _dbReader.Close();
                    }
                    catch (Exception __sqliteException)
                    {
                        Debug.LogError("Sqlite Exception: " + __sqliteException);
                    }
                }
            }
            _dbConnection.Close();
        },
        delegate
        {
            if (p_callbackFinish != null)
                p_callbackFinish(__listDataLoaded);
        });
    }
    protected void SelectDataFromTableAsync(List<string> p_listTableNames, Dictionary<string, string[]> p_dictTableArraySelectKeys, Dictionary<string, Dictionary<string, string>> p_dictTableWhereKeys, Action<List<Dictionary<string, string>>> p_finishCallback)
    {
        List<Dictionary<string, string>> __listDataLoaded = new List<Dictionary<string, string>>();
        AThread.StartNewThread(delegate
        {
            OpenDatabaseConnection();
            using (_dbTransaction = _dbConnection.BeginTransaction())
            {
                using (_dbCommand = _dbConnection.CreateCommand())
                {
                    _dbCommand.Connection = _dbConnection;
                    _dbCommand.Transaction = _dbTransaction;
                    _dbCommand.CommandText = BuildQuerySelect(SelectType.NONE, false, p_listTableNames, p_dictTableArraySelectKeys, p_dictTableWhereKeys);
                    try
                    {
                        _dbReader = _dbCommand.ExecuteReader();
                        while (_dbReader.Read())
                        {
                            Dictionary<string, string> __dictRowLoaded = new Dictionary<string, string>();
                            for (int i = 0;i < p_listTableNames.Count;i++)
                            {
                                string __tableName = p_listTableNames[i];
                                for (int j = 0;j < p_dictTableArraySelectKeys[__tableName].Length;j++)
                                {
                                    string __stringCollumnValue = _dbReader.GetString(j);
                                    __dictRowLoaded.Add(p_dictTableArraySelectKeys[__tableName][j], __stringCollumnValue);
                                }
                            }
                            __listDataLoaded.Add(__dictRowLoaded);
                        }
                        _dbReader.Close();
                    }
                    catch (SqliteException __sqliteException)
                    {
                        Debug.Log("Sqlite Exception: " + __sqliteException);
                    }
                }
            }
            _dbConnection.Close();
        },
        delegate
        {
            p_finishCallback(__listDataLoaded);
        });
    }

    #endregion
    #region Database Connection Methods
    protected void OpenDatabaseConnection()
    {
        try
        {
            _dbConnection = new SqliteConnection(_dbPath);
            _dbConnection.Open();
        }
        catch (SqliteException __sqliteException)
        {
            Debug.Log("Sqlite Exception: " + __sqliteException);
        }
    }
    protected List<Dictionary<string, string>> SelectDataFromTable(SelectType p_selectType, bool p_selectAll, string p_tableName, string[] p_arraySelectKeys, Dictionary<string, string> p_dictWhere)
    {
        List<Dictionary<string, string>> __listDataLoaded = new List<Dictionary<string, string>>();
        OpenDatabaseConnection();
        using (_dbCommand = _dbConnection.CreateCommand())
        {
            _dbCommand.Connection = _dbConnection;
            _dbCommand.CommandText = BuildQuerySelect(p_selectType, p_selectAll, p_tableName, p_arraySelectKeys, p_dictWhere);
            try
            {
                _dbReader = _dbCommand.ExecuteReader();
                while (_dbReader.Read())
                {
                    Dictionary<string, string> __dictSelectedData = new Dictionary<string, string>();
                    for (int i = 0;i < p_arraySelectKeys.Length;i++)
                    {
                        string __rowLoaded = _dbReader.GetString(i);
                        __dictSelectedData.Add(p_arraySelectKeys[i], __rowLoaded);
                    }
                    __listDataLoaded.Add(__dictSelectedData);
                }
                _dbReader.Close();
            }
            catch (SqliteException __sqliteException)
            {
                Debug.LogError(__sqliteException);
            }
        }
        _dbConnection.Close();
        return __listDataLoaded;
    }
    protected int SelectDataCountFromTable(string p_tableName)
    {
        int __dataCount = 0;
        OpenDatabaseConnection();
        using (_dbCommand = _dbConnection.CreateCommand())
        {
            _dbCommand.Connection = _dbConnection;
            Dictionary<string, string> __dictWhere = new Dictionary<string, string>();
            __dictWhere.Add("1", "1");
            _dbCommand.CommandText = BuildQuerySelectCount(p_tableName, __dictWhere);
            try
            {
                _dbReader = _dbCommand.ExecuteReader();
                while (_dbReader.Read())
                {
                    __dataCount = _dbReader.GetInt32(0);
                }
                _dbReader.Close();
            }
            catch (SqliteException __sqliteException)
            {
                Debug.LogError(__sqliteException);
            }
        }
        return __dataCount;
    }
    protected void InsertListOfData(List<Dictionary<string, string>> p_listDictData, string p_tableName, Action p_callbackFinish)
    {
        AThread.StartNewThread(delegate
        {
            OpenDatabaseConnection();
            using (_dbTransaction = _dbConnection.BeginTransaction())
            {
                using (_dbCommand = _dbConnection.CreateCommand())
                {
                    _dbCommand.Connection = _dbConnection;
                    _dbCommand.Transaction = _dbTransaction;
                    try
                    {
                        for (int i = 0;i < p_listDictData.Count;i++)
                        {
                            Dictionary<string, string> __dictRowEntries = new Dictionary<string, string>();
                            foreach (var k_key in p_listDictData[i].Keys)
                            {
                                __dictRowEntries.Add(k_key, p_listDictData[i][k_key]);
                            }
                            InsertDataOnTable(__dictRowEntries, p_tableName);
                        }
                        _dbTransaction.Commit();
                    }
                    catch (SqliteException __sqliteException)
                    {
                        Debug.LogError(__sqliteException);
                        _dbTransaction.Rollback();
                    }
                }
            }
            _dbConnection.Close();
        },
        delegate
        {
            if (p_callbackFinish != null)
                p_callbackFinish();
        });
    }
    protected void UpdateListOfData(List<Dictionary<string, string>> p_listDictData, string p_tableName, string[] p_arrayWhereKeys, Action p_callbackFinish)
    {
        OpenDatabaseConnection();
        using (_dbTransaction = _dbConnection.BeginTransaction())
        {
            using (_dbCommand = _dbConnection.CreateCommand())
            {
                _dbCommand.Connection = _dbConnection;
                _dbCommand.Transaction = _dbTransaction;
                try
                {
                    for (int i = 0;i < p_listDictData.Count;i++)
                    {
                        Dictionary<string, string> __dictRowEntries = new Dictionary<string, string>();
                        foreach (var k_key in p_listDictData[i].Keys)
                        {
                            __dictRowEntries.Add(k_key, p_listDictData[i][k_key]);
                        }
                        UpdateDataOnTable(__dictRowEntries, p_tableName, p_arrayWhereKeys);
                    }
                    _dbTransaction.Commit();
                }
                catch (SqliteException __sqliteException)
                {
                    Debug.LogError("Sqlite Exception: " + __sqliteException);
                    _dbTransaction.Rollback();
                }
            }
        }
        _dbConnection.Close();
        if (p_callbackFinish != null)
            p_callbackFinish();
    }
    protected void UpdateListOfDataAsync(List<Dictionary<string, string>> p_listDictData, string p_tableName, string[] p_arrayWhereKeys, Action p_callbackFinish)
    {
        AThreadNodule __threadNodule = null;
        __threadNodule = AThread.StartNewThread(delegate
        {
            OpenDatabaseConnection();
            using (_dbTransaction = _dbConnection.BeginTransaction())
            {
                using (_dbCommand = _dbConnection.CreateCommand())
                {
                    _dbCommand.Connection = _dbConnection;
                    _dbCommand.Transaction = _dbTransaction;
                    try
                    {
                        for (int i = 0;i < p_listDictData.Count;i++)
                        {
                            Dictionary<string, string> __dictRowEntries = new Dictionary<string, string>();
                            foreach (var k_key in p_listDictData[i].Keys)
                            {
                                __dictRowEntries.Add(k_key, p_listDictData[i][k_key]);
                            }
                            UpdateDataOnTable(__dictRowEntries, p_tableName, p_arrayWhereKeys);
                        }
                        _dbTransaction.Commit();
                    }
                    catch (Exception __sqliteException)
                    {
                        Debug.LogError("Sqlite Exception: " + __sqliteException);
                        _dbTransaction.Rollback();
                    }
                }
            }
            _dbConnection.Close();
            if (_cancelDataRequest == true)
            {
                _dbConnection.Close();
                __threadNodule.CancelThread(delegate
                {
                    if (_callbackCancelDataRequest != null)
                        _callbackCancelDataRequest();
                });
            }
        }, p_callbackFinish);
    }
    protected void InsertAndUpdatePageDataAsync(SimpleJSON.JSONNode p_pageDataJSON, string p_tableName, string[] p_arrayWhereKeys, Action p_callbackFinish)
    {
        AThreadNodule __threadNodule = null;
        __threadNodule = AThread.StartNewThread(delegate
        {
            OpenDatabaseConnection();
            using (_dbTransaction = _dbConnection.BeginTransaction())
            {
                using (_dbCommand = _dbConnection.CreateCommand())
                {
                    _dbCommand.Connection = _dbConnection;
                    _dbCommand.Transaction = _dbTransaction;
                    try
                    {
                        for (int i = 0;i < p_pageDataJSON.Count;i++)
                        {
                            Dictionary<string, string> __dictResults = new Dictionary<string, string>();
                            foreach (var k_key in p_pageDataJSON[i].Keys)
                            {
                                __dictResults.Add(k_key, p_pageDataJSON[i][k_key]);
                            }
                            InsertOrUpdateData(__dictResults, p_tableName, p_arrayWhereKeys);
                        }
                        _dbTransaction.Commit();
                    }
                    catch (SqliteException __sqliteException)
                    {
                        Debug.LogError("Sqlite Exception: " + __sqliteException);
                        _dbTransaction.Rollback();
                    }
                }
            }
            _dbConnection.Close();
            if (_cancelDataRequest == true)
            {
                __threadNodule.CancelThread(delegate
                {
                    if (_callbackCancelDataRequest != null)
                        _callbackCancelDataRequest();
                });
            }
        }, p_callbackFinish);
    }
    protected void InsertAndUpdateListOfData(List<Dictionary<string, string>> p_listDataRows, string p_tableName, string[] p_arrayWhereKeys, Action<bool> p_callbackFinish)
    {
        bool __databaseDataChanged = false;
        OpenDatabaseConnection();
        using (_dbTransaction = _dbConnection.BeginTransaction())
        {
            using (_dbCommand = _dbConnection.CreateCommand())
            {
                _dbCommand.Connection = _dbConnection;
                _dbCommand.Transaction = _dbTransaction;
                try
                {
                    for (int i = 0;i < p_listDataRows.Count;i++)
                    {
                        Dictionary<string, string> __dictRowEntries = new Dictionary<string, string>();
                        foreach (var k_key in p_listDataRows[i].Keys)
                        {
                            __dictRowEntries.Add(k_key, p_listDataRows[i][k_key]);
                        }
                        bool __databaseDataChangedTemp = InsertOrUpdateData(__dictRowEntries, p_tableName, p_arrayWhereKeys);
                        if (__databaseDataChanged == false)
                        {
                            __databaseDataChanged = __databaseDataChangedTemp;
                        }
                    }
                    if (__databaseDataChanged == true)
                    {
                        _dbTransaction.Commit();
                    }
                }
                catch (SqliteException __sqliteException)
                {
                    Debug.LogError("Sqlite Exception: " + __sqliteException);
                    _dbTransaction.Rollback();
                }
            }
        }
        _dbConnection.Close();
        if (p_callbackFinish != null)
            p_callbackFinish(__databaseDataChanged);
    }
    protected void InsertAndUpdateListOfDataAsync(List<Dictionary<string, string>> p_listDataRows, string p_tableName, string[] p_arrayWhereKeys, Action<bool> p_callbackFinish)
    {
        AThreadNodule __threadNodule = null;
        bool __databaseDataChanged = false;
        __threadNodule = AThread.StartNewThread(delegate
        {
            OpenDatabaseConnection();
            using (_dbTransaction = _dbConnection.BeginTransaction())
            {
                using (_dbCommand = _dbConnection.CreateCommand())
                {
                    _dbCommand.Connection = _dbConnection;
                    _dbCommand.Transaction = _dbTransaction;
                    try
                    {
                        for (int i = 0;i < p_listDataRows.Count;i++)
                        {
                            Dictionary<string, string> __dictRowEntries = new Dictionary<string, string>();
                            foreach (var k_key in p_listDataRows[i].Keys)
                            {
                                __dictRowEntries.Add(k_key, p_listDataRows[i][k_key]);
                            }
                            bool __databaseDataChangedTemp = InsertOrUpdateData(__dictRowEntries, p_tableName, p_arrayWhereKeys);
                            if (__databaseDataChanged == false)
                            {
                                __databaseDataChanged = __databaseDataChangedTemp;
                            }
                        }
                        if (__databaseDataChanged == true)
                        {
                            _dbTransaction.Commit();
                        }
                    }
                    catch (SqliteException __sqliteException)
                    {
                        Debug.LogError("Sqlite Exception: " + __sqliteException);
                        _dbTransaction.Rollback();
                    }
                }
            }
            _dbConnection.Close();
            if (_cancelDataRequest == true)
            {
                __threadNodule.CancelThread(delegate
                {
                    if (_callbackCancelDataRequest != null)
                        _callbackCancelDataRequest();
                });
            }
        },
        delegate
        {
            if (p_callbackFinish != null)
                p_callbackFinish(__databaseDataChanged);
        });
    }
    protected void ClearDataFromTable(string p_tableName, Action p_callbackSuccess, Action<string> p_callbackFailed)
    {
        OpenDatabaseConnection();
        using (_dbTransaction = _dbConnection.BeginTransaction())
        {
            using (_dbCommand = _dbConnection.CreateCommand())
            {
                _dbCommand.Connection = _dbConnection;
                _dbCommand.Transaction = _dbTransaction;
                try
                {
                    ClearTableData(p_tableName);
                    _dbTransaction.Commit();
                }
                catch (SqliteException __sqliteException)
                {
                    _dbTransaction.Rollback();
                    p_callbackFailed("Sqlite Exception: " + __sqliteException);
                    return;
                }
            }
        }
        _dbConnection.Close();
        p_callbackSuccess();
    }
    protected void DeleteListOfDataAsync(List<Dictionary<string, string>> p_listDataRows, string p_tableName, string[] p_arrayWhereKeys, Action p_callbackFinish)
    {
        AThreadNodule __threadNodule = null;
        __threadNodule = AThread.StartNewThread(delegate
        {
            OpenDatabaseConnection();
            using (_dbTransaction = _dbConnection.BeginTransaction())
            {
                using (_dbCommand = _dbConnection.CreateCommand())
                {
                    _dbCommand.Connection = _dbConnection;
                    _dbCommand.Transaction = _dbTransaction;
                    try
                    {
                        for (int i = 0;i < p_listDataRows.Count;i++)
                        {
                            Dictionary<string, string> __dictRowEntries = new Dictionary<string, string>();
                            foreach (var k_key in p_listDataRows[i].Keys)
                            {
                                __dictRowEntries.Add(k_key, p_listDataRows[i][k_key]);
                            }
                            DeleteData(__dictRowEntries, p_tableName, p_arrayWhereKeys);
                        }
                        _dbTransaction.Commit();
                    }
                    catch (SqliteException __sqliteException)
                    {
                        Debug.LogError("Sqlite Exception: " + __sqliteException);
                        _dbTransaction.Rollback();
                    }
                }
            }
            _dbConnection.Close();
            if (_cancelDataRequest == true)
            {
                __threadNodule.CancelThread(delegate
                {
                    if (_callbackCancelDataRequest != null)
                        _callbackCancelDataRequest();
                });
            }
        }, p_callbackFinish);
    }
    private void ClearTableData(string p_tableName)
    {
        _dbCommand.CommandText = BuildClearTableQuery(p_tableName);
        _dbCommand.ExecuteNonQuery();
    }
    private void DeleteData(Dictionary<string, string> p_dictData, string p_tableName, string[] p_arrayWhereKeys)
    {
        _dbCommand.CommandText = BuildDeleteQuery(p_dictData, p_tableName, p_arrayWhereKeys);
        _dbCommand.ExecuteNonQuery();
    }
    private bool InsertOrUpdateData(Dictionary<string, string> p_dictData, string p_tableName, string[] p_arrayWhereKeys)
    {
        Dictionary<string, string> __dictWhere = new Dictionary<string, string>();
        string[] __arraySelectKeys = new string[p_dictData.Count];
        int __counter = 0;
        foreach (KeyValuePair<string, string> k_dictData in p_dictData)
        {
            __arraySelectKeys[__counter] = k_dictData.Key;
            __counter++;
        }
        for (int i = 0;i < p_arrayWhereKeys.Length;i++)
        {
            __dictWhere.Add(p_arrayWhereKeys[i], p_dictData[p_arrayWhereKeys[i]]);
        }
        _dbCommand.CommandText = BuildQuerySelect(SelectType.NONE, false, p_tableName, __arraySelectKeys, __dictWhere);
        try
        {
            _dbReader = _dbCommand.ExecuteReader();
            bool __doesExistInTable = false;
            bool __doesItNeedToBeUpdated = false;
            while (_dbReader.Read())
            {
                __doesExistInTable = true;
                for (int i = 0;i < __arraySelectKeys.Length;i++)
                {
                    string __stringCollumnEntry = string.Empty;
                    if (typeof(DateTime) == _dbReader.GetFieldType(i))
                    {
                        //__stringRow = _dbReader.GetDateTime(i + 1).ToString(_dateFormat);
                        //Debug.Log("DateTime to String: " + __stringRow);
                        DateTime __dateFromLocal = _dbReader.GetDateTime(i);
                        DateTime __dateFromData = DateTime.Parse(p_dictData[__arraySelectKeys[i]]);
                        if (__dateFromLocal.Ticks != __dateFromData.Ticks)
                        {
                            __doesItNeedToBeUpdated = true;
                        }
                    }
                    else
                    {
                        __stringCollumnEntry = _dbReader.GetValue(i).ToString();
                        if (_dbReader.GetFieldType(i) == typeof(bool))
                        {
                            if (__stringCollumnEntry == "False" || __stringCollumnEntry == "false")
                            {
                                __stringCollumnEntry = "0";
                            }
                            else
                            {
                                __stringCollumnEntry = "1";
                            }
                        }
                        if (__stringCollumnEntry != p_dictData[__arraySelectKeys[i]])
                        {
                            __doesItNeedToBeUpdated = true;
                        }
                    }
                }
            }
            _dbReader.Close();
            if (__doesExistInTable == true)
            {
                if (__doesItNeedToBeUpdated == true)
                {
                    UpdateDataOnTable(p_dictData, p_tableName, p_arrayWhereKeys);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                InsertDataOnTable(p_dictData, p_tableName);
                return true;
            }
        }
        catch (SqliteException __sqliteException)
        {
            Debug.Log("Sqlite Exception: " + __sqliteException);
            return false;
        }
    }
    private void UpdateDataOnTable(Dictionary<string, string> p_dictData, string p_tableName, string[] p_arrayWhereKeys)
    {   
        _dbCommand.CommandText = BuildQuerryUpdateModel(p_dictData, p_tableName, p_arrayWhereKeys);
        _dbCommand.ExecuteNonQuery();
    }
    private void InsertDataOnTable(Dictionary<string, string> p_dictData, string p_tableName)
    {
        _dbCommand.CommandText = BuildQueryInsertModel(p_dictData, p_tableName);
        _dbCommand.ExecuteNonQuery();
    }
    #endregion
    public void CancelDataRequest(Action p_callbackCancelDataRequest)
    {
        _callbackCancelDataRequest = p_callbackCancelDataRequest;
        _cancelDataRequest = true;
    }
    public void UncancelDataRequest()
    {
        _cancelDataRequest = false;
    }
    public void ApplicationQuitHandler()
    {
        if (_dbConnection != null)
        {
            _dbConnection.Close();
        }
    }
    private void GetDatabasePath()
    {
#if UNITY_EDITOR
        {
            _dbPath = "URI=file:" + Application.dataPath + "/StreamingAssets/" + _dbName;
            Debug.Log(_dbPath);
        }
#elif UNITY_IOS
{
_dbPath = "URI=file:" + Application.persistentDataPath + "/" + _dbName;
}
#elif UNITY_ANDROID
{ 
_dbPath = "URI=file:" + Application.persistentDataPath + "/" + _dbName; 
}
#elif UNITY_STANDALONE_WIN
{
_dbPath = "URI=file:" + _windowsDBPath + "/" + _dbName;
}
#elif UNITY_STANDALONE_OSX
{ 
_dbPath = "URI=file:" + _macDBPath + "/" + _dbName;
}
#endif
    }
}