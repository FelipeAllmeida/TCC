using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Mono.Data.Sqlite;
using System.Data;

public class LocalConnector
{
    private IDbConnection _dbcon;
    private string _dbName = "punk_hazard.sqdb";
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
#endif
    // Need to load this name from a Config file
    private string _dbURI;

    private IDbCommand _dbCommand;
    private IDataReader _dbReader;
    private IDbTransaction _dbTransacion;
#if UNITY_STANDALONE_OSX
	private string _macDBPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/DBData";
#endif
#if UNITY_STANDALONE_WIN
    private string _windowsDBPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/DownTownStudios/PunkHazard/DBData";
#endif
    // Use this for initialization
    public void AInitialize()
    {
        CreateDatabaseaAtCurrentPlatform();
    }

#if UNITY_ANDROID

    #region AndroidGetDatabasePath

	
	private void GetAndroidDatabasePath()
	{
	_dbURI = "URI=file:" + Application.persistentDataPath + "/" +  _dbName;
	CreateDatabaseLogicForAndroid();
	}

	private void CreateDatabaseLogicForAndroid()
	{
	if(File.Exists(Application.persistentDataPath + "/" + _dbName) == false)
	{
	WWW www = new WWW ("jar:file://" + Application.dataPath + "!/assets/" + _dbName);

	while (!www.isDone) {}

	System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + _dbName, www.bytes);

	InsertBundleVersionDataInDatabase(CurrentBundleVersion.version);
	}
	else
	{
	string __foundVersion = CheckBundleVersion();

	if (__foundVersion != CurrentBundleVersion.version)
	{
	WWW www = new WWW ("jar:file://" + Application.dataPath + "!/assets/" + _dbName);

	while (!www.isDone) {}

	System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + _dbName, www.bytes);

	InsertBundleVersionDataInDatabase(CurrentBundleVersion.version);
	}
	}
	}

	



    #endregion

	
#endif

#if UNITY_IOS
	



    #region IOSGetDatabasePath()

	
	private void GetIOSDatabasePath()
	{
	_dbURI = "URI=file:" + Application.persistentDataPath + "/" +  _dbName;
	CreateDatabaseLogicForIOS();
	}

	private void CreateDatabaseLogicForIOS()
	{
	if(File.Exists(Application.persistentDataPath + "/" + _dbName) == false)
	{
	System.IO.File.Copy(Application.dataPath + "/Raw/" +  _dbName, Application.persistentDataPath + "/" + _dbName, true);

	UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath + "/" + _dbName);

	InsertBundleVersionDataInDatabase(CurrentBundleVersion.version);
	}
	else
	{
	string __foundVersion = CheckBundleVersion();

	if (__foundVersion != CurrentBundleVersion.version)
	{
	System.IO.File.Copy(Application.dataPath + "/Raw/" +  _dbName, Application.persistentDataPath + "/" + _dbName, true);

	InsertBundleVersionDataInDatabase(CurrentBundleVersion.version);

	UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath + "/" + _dbName);
	}
	}
	}
	



    #endregion

	
#endif

#if UNITY_STANDALONE_OSX
	

    #region MacGetDatabasePath()

	
	private void GetMacDatabasePath ()
	{
		_dbURI = "URI=file:" + _macDBPath + "/" + _dbNameDecrypted;
		CreateDatabaseLogicForMac ();
	}

	private void CreateDatabaseLogicForMac ()
	{
		if (Directory.Exists (_macDBPath) == true) {
			if (File.Exists (_macDBPath + "/" + _dbName) == true) {
				if (File.Exists (_macDBPath + "/" + _dbNameDecrypted) == true) {
					if (File.Exists (_macDBPath + "/" + _dbName) == true) {
						File.Delete (_macDBPath + "/" + _dbName);
					}

					SecureFile.EncryptFile (_macDBPath + "/" + _dbNameDecrypted, _macDBPath + "/" + _dbName);
				} else {
					SecureFile.DecryptFile (_macDBPath + "/" + _dbName, _macDBPath + "/" + _dbNameDecrypted);
				}

				string __foundVersion = CheckBundleVersion ();

				if (__foundVersion != CurrentBundleVersion.version) {
					if (File.Exists (_macDBPath + "/" + _dbName) == true) {
						File.Delete (_macDBPath + "/" + _dbName);
					}
					if (File.Exists (_macDBPath + "/" + _dbNameDecrypted) == true) {
						File.Delete (_macDBPath + "/" + _dbNameDecrypted);
					}

					File.Copy (Application.streamingAssetsPath + "/" + _dbName, _macDBPath + "/" + _dbName, true);

					SecureFile.DecryptFile (_macDBPath + "/" + _dbName, _macDBPath + "/" + _dbNameDecrypted);

					InsertBundleVersionDataInDatabase (CurrentBundleVersion.version);
				}
			} else {
				File.Copy (Application.streamingAssetsPath + "/" + _dbName, _macDBPath + "/" + _dbName, true);

				if (File.Exists (_macDBPath + "/" + _dbNameDecrypted) == true) {
					if (File.Exists (_macDBPath + "/" + _dbName) == true) {
						File.Delete (_macDBPath + "/" + _dbName);
					}

					SecureFile.EncryptFile (_macDBPath + "/" + _dbNameDecrypted, _macDBPath + "/" + _dbName);
				} else {
					SecureFile.DecryptFile (_macDBPath + "/" + _dbName, _macDBPath + "/" + _dbNameDecrypted);
				}
			}
		} else {
			Directory.CreateDirectory (_macDBPath);
			File.Copy (Application.streamingAssetsPath + "/" + _dbName, _macDBPath + "/" + _dbName, true);
			SecureFile.DecryptFile (_macDBPath + "/" + _dbName, _macDBPath + "/" + _dbNameDecrypted);

			InsertBundleVersionDataInDatabase (CurrentBundleVersion.version);
		}
	}

	private void UpdateDatabaseOSX ()
	{
		if (File.Exists (_macDBPath + "/" + _dbNameDecrypted) == true) {
			if (File.Exists (_macDBPath + "/" + _dbName) == true) {
				File.Delete (_macDBPath + "/" + _dbName);
			}
			SecureFile.EncryptFile (_macDBPath + "/" + _dbNameDecrypted, _macDBPath + "/" + _dbName);
			File.Delete (_macDBPath + "/" + _dbNameDecrypted);
		}
	}

	

    #endregion

	
#endif

#if UNITY_STANDALONE_WIN
    
    #region WindowsGetDatabasePath

    private void CreateDatabaseLogicForWindows()
    {
        Debug.Log("CreateDatabaseLogicForWindows");
        if (Directory.Exists(_windowsDBPath) == true)
        {
            Debug.Log("Directory Exists: " + _windowsDBPath);
            if (File.Exists(_windowsDBPath + "/" + _dbName) == true)
            {
                File.Delete(_windowsDBPath + "/" + _dbName);
                File.Copy(Application.streamingAssetsPath + "/" + _dbName, _windowsDBPath + "/" + _dbName, true);
            }
            /*//if (File.Exists(_windowsDBPath + "/" +_dbName) == true)
            {
                if (File.Exists(_windowsDBPath + "/" +_dbNameDecrypted) == true)
                {
                    if (File.Exists(_windowsDBPath + "/" +_dbName) == true)
                    {
                        File.Delete(_windowsDBPath + "/" +_dbName);
                    }
                }
                else
                {
                }

                string __foundVersion = CheckBundleVersion();

                if (__foundVersion != CurrentBundleVersion.version)
                {
                    if (File.Exists(_windowsDBPath + "/" +_dbName) == true)
                    {
                        File.Delete(_windowsDBPath + "/" +_dbName);
                    }
                    if (File.Exists(_windowsDBPath + "/" +_dbNameDecrypted) == true)
                    {
                        File.Delete(_windowsDBPath + "/" +_dbNameDecrypted);
                    }

                    File.Copy(Application.streamingAssetsPath + "/" +_dbName, _windowsDBPath + "/" +_dbName, true);

                    SecureFile.DecryptFile(_windowsDBPath + "/" +_dbName, _windowsDBPath + "/" +_dbNameDecrypted);

                    InsertBundleVersionDataInDatabase(CurrentBundleVersion.version);
                }
            }
            else
            {
                File.Copy(Application.streamingAssetsPath + "/" +_dbName, _windowsDBPath + "/" +_dbName, true);

                if (File.Exists(_windowsDBPath + "/" +_dbNameDecrypted) == true)
                {
                    if (File.Exists(_windowsDBPath + "/" +_dbName) == true)
                    {
                        File.Delete(_windowsDBPath + "/" +_dbName);
                    }
                    SecureFile.EncryptFile(_windowsDBPath + "/" +_dbNameDecrypted, _windowsDBPath + "/" +_dbName);
                }
                else
                {
                    SecureFile.DecryptFile(_windowsDBPath + "/" +_dbName, _windowsDBPath + "/" +_dbNameDecrypted);
                }
            }*/
        }
        else
        {
            Debug.Log("Create Directory: " + _windowsDBPath);
            Directory.CreateDirectory(_windowsDBPath);
            File.Copy(Application.streamingAssetsPath + "/" +_dbName, _windowsDBPath + "/" +_dbName, true);
        }
    }


    #endregion

#endif

#if UNITY_EDITOR
    #region EditorGetDatabasePath

    private void GetEditorDatabasePath()
    {
        _dbURI = "URI = file:" +Application.dataPath + "/ StreamingAssets /" +_dbName;
    }

    #endregion

#endif

    private void CreateDatabaseaAtCurrentPlatform()
    {
        //Debug.Log("CreateDatabaseaAtCurrentPlatform");
//#if UNITY_EDITOR
//        {
//            GetEditorDatabasePath();
//        }
#if UNITY_IOS
		{
		GetIOSDatabasePath();
		}
#elif UNITY_ANDROID
		{
		GetAndroidDatabasePath();
		}
#elif UNITY_STANDALONE_WIN
		{
		CreateDatabaseLogicForWindows();		
		}
#elif UNITY_STANDALONE_OSX
		{
		GetMacDatabasePath();
		}
#endif
    }
}