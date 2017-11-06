using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour 
{
    #region Events
    public event Action<string> onClickJoinRoom;
    #endregion

    [SerializeField] private Text _roomNameText;

    public bool updated = false;

    private Button _button;

    public void AInitialize()
    {
        InitializeButton();
        updated = false;
    }

    private void InitializeButton()
    {
        _button = GetComponent<Button>();
        ListenButtonEvents();
    }

    private void ListenButtonEvents()
    {
        _button.onClick.AddListener(delegate
        {
            if (onClickJoinRoom != null) onClickJoinRoom(_roomNameText.text);
        });
    }

    public void SetRoomNameText(string p_roomName)
    {
        _roomNameText.text = p_roomName;
    }

    public string GetRoomName()
    {
        return _roomNameText.text; 
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();   
    }
}
