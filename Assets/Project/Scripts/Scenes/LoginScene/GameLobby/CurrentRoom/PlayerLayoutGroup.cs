using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviour
{
    private List<PlayerListing> _listPlayerListing = new List<PlayerListing>();

    public void Enable()
    {
        InitializeRoom();
        ListenEvents();
    }

    public void Disable()
    {
        PhotonUtility.onPhotonPlayerDisconnected -= HandleOnPhotonPlayerDisconnected;
        PhotonUtility.onPhotonPlayerConnected -= HandleOnPhotonPlayerConnected;
        PhotonUtility.onMasterClientSwitched -= HandleOnMasterClientSwitched;
    }

    private void ListenEvents()
    {
        PhotonUtility.onPhotonPlayerDisconnected -= HandleOnPhotonPlayerDisconnected;
        PhotonUtility.onPhotonPlayerDisconnected += HandleOnPhotonPlayerDisconnected;

        PhotonUtility.onPhotonPlayerConnected -= HandleOnPhotonPlayerConnected;
        PhotonUtility.onPhotonPlayerConnected += HandleOnPhotonPlayerConnected;

        PhotonUtility.onMasterClientSwitched -= HandleOnMasterClientSwitched;
        PhotonUtility.onMasterClientSwitched += HandleOnMasterClientSwitched;
    }

    private void InitializeRoom()
    {
        for (int i = 0;i < _listPlayerListing.Count;i++)
        {
            PoolManager.instance.Despawn(PoolType.PLAYER_LISTING, _listPlayerListing[i].gameObject);
        }
        _listPlayerListing.Clear();

        PhotonPlayer[] __photonPlayerArray = PhotonNetwork.playerList;
        for (int i = 0; i < __photonPlayerArray.Length;i++)
        {
            PlayerJoinedRoom(__photonPlayerArray[i]);
        }
    }

    private void HandleOnPhotonPlayerConnected(PhotonPlayer p_photonPlayer)
    {
        PlayerJoinedRoom(p_photonPlayer);
    }

    private void HandleOnPhotonPlayerDisconnected(PhotonPlayer p_photonPlayer)
    {
        PlayerLeftRoom(p_photonPlayer);
    }

    private void HandleOnMasterClientSwitched(PhotonPlayer p_photonPlayer)
    {
        PhotonUtility.LeaveRoom();
    }

    private void PlayerLeftRoom(PhotonPlayer p_photonPlayer)
    {
        int __index = _listPlayerListing.FindIndex(x => x.photonPlayer == p_photonPlayer);

        if (__index != -1)
        {
            PoolManager.instance.Despawn(PoolType.PLAYER_LISTING, _listPlayerListing[__index].gameObject);
            _listPlayerListing.RemoveAt(__index);
        }
    }

    private void PlayerJoinedRoom(PhotonPlayer p_photonPlayer)
    {
        if (p_photonPlayer == null)
            return;

        PlayerLeftRoom(p_photonPlayer);

        PlayerListing __playerListing = PoolManager.instance.Spawn(PoolType.PLAYER_LISTING, transform).GetComponent<PlayerListing>();
        __playerListing.SetPhotonPlayer(p_photonPlayer);

        _listPlayerListing.Add(__playerListing);
    }
}
