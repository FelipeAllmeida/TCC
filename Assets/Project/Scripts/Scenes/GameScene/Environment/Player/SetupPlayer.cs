using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupPlayer : NetworkBehaviour
 {
	void Start () 
    {
        if (isLocalPlayer == true)
        {
            GetComponent<Player>().enabled = true;
        }
        else
        {
            GetComponent<Player>().enabled = false;
        }
    }
}
