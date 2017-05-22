using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLobby : MonoBehaviour
{
    [SerializeField] private SignInScreen _signInScreen;
    public void AInitialize()
    {
        _signInScreen.Initialize();
    }

    public void AUpdate()
    {
    
    }

}
