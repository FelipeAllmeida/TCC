using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLayoutGroup : MonoBehaviour 
{
    #region Events
    public event Action<string> onClickJoinRoom;
    #endregion

    private List<RoomListing> _listRooms = new List<RoomListing>();

    public void AInitialize()
    {
        ListenEvents();
    }

    public List<RoomListing> GetRooms()
    {
        return _listRooms;
    }

    private void ListenEvents()
    {
        PhotonUtility.onReceivedRoomListUpdate -= HandleOnReceivedRoomListUpdate;
        PhotonUtility.onReceivedRoomListUpdate += HandleOnReceivedRoomListUpdate;
    }

    private void HandleOnReceivedRoomListUpdate()
    {

        RoomInfo[] _arrayRoomInfo = PhotonUtility.GetRoomList();
        foreach (RoomInfo __roomInfo in _arrayRoomInfo)
        {
            RoomReceived(__roomInfo);
        }

        RemoveOldRooms();
    }

    private void RoomReceived(RoomInfo p_roomInfo)
    {
        int __index = _listRooms.FindIndex(x => x.GetRoomName() == p_roomInfo.Name);

        if (__index == -1)
        {
            if (p_roomInfo.IsVisible && p_roomInfo.PlayerCount < p_roomInfo.MaxPlayers)
            {
                RoomListing __newRoomListing = PoolManager.instance.Spawn(PoolType.ROOM_LISTING, transform).GetComponent<RoomListing>();
                __newRoomListing.AInitialize();
                ListenRoomListingEvents(__newRoomListing);
                _listRooms.Add(__newRoomListing);
                __index = (_listRooms.Count - 1);
            }
        }

        if (__index != -1)
        {
            RoomListing __roomListing = _listRooms[__index];
            __roomListing.SetRoomNameText(p_roomInfo.Name);
            __roomListing.updated = true;
        }
    }

    private void ListenRoomListingEvents(RoomListing p_roomListing)
    {
        p_roomListing.onClickJoinRoom += onClickJoinRoom;
    }

    private void RemoveOldRooms()
    {
        List<RoomListing> _listToRemoveRooms = new List<RoomListing>();

        foreach (RoomListing __roomListing in _listRooms)
        {
            if (__roomListing.updated == false)
            {
                _listToRemoveRooms.Add(__roomListing);
            }
            else
            {
                __roomListing.updated = false;
            }
        }

        foreach (RoomListing __roomListing in _listToRemoveRooms)
        {
            PoolManager.instance.Despawn(PoolType.ROOM_LISTING, __roomListing.gameObject);
        }
    }
}
