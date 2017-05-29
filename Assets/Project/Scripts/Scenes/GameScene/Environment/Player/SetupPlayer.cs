using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupPlayer : NetworkBehaviour
 {
	void Start () 
    {
        if (isLocalPlayer)
        {
            GetComponent<Player>().enabled = false;
        }
	}
}
