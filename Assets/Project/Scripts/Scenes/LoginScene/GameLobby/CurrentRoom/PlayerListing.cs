using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//PUN   
public class PlayerListing : MonoBehaviour
{
    [SerializeField] private Text _playerNameText;
    [SerializeField] private Image _playerColor;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Button _button;

    public PhotonPlayer photonPlayer {get; private set;}

    private int _lastEnumValue = 0;


    public void SetPhotonPlayer(PhotonPlayer p_photonPlayer)
    {
        _button.onClick.RemoveAllListeners();


        photonPlayer = p_photonPlayer;
        _playerNameText.text = photonPlayer.NickName;

        if (photonPlayer.IsLocal == true)
        {
            _button.interactable = true;
            _button.onClick.AddListener(SetNextColorToPlayer);
            SetNextColorToPlayer();
        }
        else
        {
            _button.interactable = false;
        }
    }

    public string GetPlayerName()
    {
        return _playerNameText.text;
    }

    private void SetNextColorToPlayer()
    {
        for (int i = _lastEnumValue; i < Enum.GetValues(typeof(PlayerNetwork.Colors)).Length; i++)
        {
            _lastEnumValue = i;
            bool __alreadyUsed = false;

            foreach (PhotonPlayer __photonPlayer in PhotonUtility.playerList)
            {
                if (__photonPlayer.CustomProperties.ContainsKey("Color"))
                {
                    if ((PlayerNetwork.Colors)__photonPlayer.CustomProperties["Color"] == (PlayerNetwork.Colors)i)
                    {
                        __alreadyUsed = true;
                        break;
                    }
                }
            }

            if (__alreadyUsed == false)
            {
                Color __nextColor = ((PlayerNetwork.Colors)i).ToColor();
                _playerColor.color = __nextColor;
                PhotonUtility.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Color", (PlayerNetwork.Colors)i } });
                if (_lastEnumValue == Enum.GetValues(typeof(PlayerNetwork.Colors)).Length - 1)
                    _lastEnumValue = 0;
                break;
            }

            if (_lastEnumValue == Enum.GetValues(typeof(PlayerNetwork.Colors)).Length - 1)
            {
                _lastEnumValue = 0;
            }
        }
    }

    private void Update()
    {
        if (photonPlayer.IsLocal == false)
        {
            foreach (var photonPlayer in PhotonUtility.playerList)
            {
                if (photonPlayer.ID == photonPlayer.ID)
                {
                    _playerColor.color = ((PlayerNetwork.Colors)photonPlayer.CustomProperties["Color"]).ToColor();
                    break;
                }
            }
        }
    }
}
