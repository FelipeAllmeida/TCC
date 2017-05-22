using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class LoginScene : State<TCC.StateType>
{
    #region Private Serialized-Data
    [SerializeField] private GameLobby _gameLobby;
    #endregion

    public override void AInitialize()
    {
        _gameLobby.AInitialize();
    }

    public override void AUpdate()
    {
    
    }

    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }
}
