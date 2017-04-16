using UnityEngine;
using System.Collections;
namespace Framework
{
    public class Utilities
    {
        public static GameObject SpawnAt(GameObject p_gameObject, Vector3 p_spawnPosition, Transform p_parent, Quaternion p_quaternion)
        {
            GameObject __spawnedGameObject = GameObject.Instantiate(p_gameObject, p_spawnPosition, p_quaternion) as GameObject;
            __spawnedGameObject.transform.SetParent(p_parent);
            return __spawnedGameObject;
        }
    }
}