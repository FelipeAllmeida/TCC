using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ServerData 
{
    public class UserData
    {
        public UserData(string p_id)
        {
            Debug.Log("New user of id: " + p_id);
            _id = p_id;
        }
        private string _id;
        public string id { get { return _id; } }

        public string userName;
    }

    public Dictionary<string, UserData> dictUserData = new Dictionary<string, UserData>();
}
