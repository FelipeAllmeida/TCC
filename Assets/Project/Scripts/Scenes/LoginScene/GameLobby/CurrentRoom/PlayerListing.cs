using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
    [SerializeField] private Text _playerNameText;

    public PhotonPlayer photonPlayer {get; private set;}

    public void SetPhotonPlayer(PhotonPlayer p_photonPlayer)
    {
        photonPlayer = p_photonPlayer;
        _playerNameText.text = photonPlayer.NickName;
    }

    public string GetPlayerName()
    {
        return _playerNameText.text;
    }
}
