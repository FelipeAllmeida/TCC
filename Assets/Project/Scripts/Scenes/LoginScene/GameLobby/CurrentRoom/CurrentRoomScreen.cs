using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRoomScreen : MonoBehaviour 
{
    public Action onClickStartMatchButton;
    public Action onLeaveRoom;

    [SerializeField] private Button _startMatchButton;
    [SerializeField] private Button _roomStateButton;
    [SerializeField] private Button _leaveRoomButton;

    [SerializeField] private Text _roomStateText;

    [SerializeField] PlayerLayoutGroup _playerLayoutGroup;

    public void AInitialize()
    {
    }

    public void Activate()
    {
        _roomStateText.text = ((PhotonUtility.IsRoomOpen) ? "Public" : "Private") + " Room";
        _playerLayoutGroup.Enable();

        _startMatchButton.interactable = PhotonUtility.IsMasterClient;

        ListenEvents();

        gameObject.SetActive(true);
    }

    private void ListenEvents()
    {
        _startMatchButton.onClick.RemoveAllListeners();
        _roomStateButton.onClick.RemoveAllListeners();
        _leaveRoomButton.onClick.RemoveAllListeners();

        _startMatchButton.onClick.AddListener(() =>
        {
            HandleOnClickStartSync();
        });

        _roomStateButton.onClick.AddListener(() =>
        {
            HandleOnClickRoomState();
        });

        _leaveRoomButton.onClick.AddListener(() =>
        {
            HandleOnClickLeaveRoom();
        });
    }

    private void HandleOnClickStartSync()
    {
        _playerLayoutGroup.Disable();
        if (onClickStartMatchButton != null) onClickStartMatchButton();
    }

    private void HandleOnClickRoomState()
    {
        if (PhotonUtility.IsMasterClient == false)
            return;

        PhotonUtility.IsRoomOpen = !PhotonUtility.IsRoomOpen;
        PhotonUtility.IsRoomVisible = PhotonUtility.IsRoomOpen;

        _roomStateText.text = ((PhotonUtility.IsRoomOpen) ? "Public" : "Private") + " Room";
    }

    private void HandleOnClickLeaveRoom()
    {
        PhotonUtility.LeaveRoom();
        _playerLayoutGroup.Disable();
        if (onLeaveRoom != null) onLeaveRoom();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
