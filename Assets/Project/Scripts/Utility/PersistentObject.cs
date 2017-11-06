using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour 
{
    public static PersistentObject instance;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void AddGameObject(GameObject p_gameObject)
    {    
        p_gameObject.transform.SetParent(transform);
    }
}
