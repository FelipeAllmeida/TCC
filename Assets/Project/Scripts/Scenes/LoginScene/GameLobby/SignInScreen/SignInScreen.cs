using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignInScreen : MonoBehaviour 
{
    #region Private Serialized-Data
    [SerializeField] private InputField _nickInputField;
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _createServerButton;
    #endregion

    public event Action onSuccessfullyConnect;

    private bool _canConnect = false;

    public void Initialize()
    {
        ListenEvents();
        EnableInputs(false);
    }

    public void EnableScreen(bool p_value)
    {
        gameObject.SetActive(p_value);
    }

    private void EnableInputs(bool p_value)
    {
        _connectButton.interactable = p_value;
        if (_canConnect == true)
        {
            _createServerButton.interactable = p_value;
        }
        else
        {
            _createServerButton.interactable = false;
        }
    }

    private void ListenEvents()
    {
        _connectButton.onClick.RemoveAllListeners();
        _createServerButton.onClick.RemoveAllListeners();

        _connectButton.onClick.AddListener(delegate
        {
            EnableInputs(false);
            DataManager.instance.TryToConnectToSocket("127.0.0.1", 1300, delegate
            {
                StreamToServer.ConnectNewUser(_nickInputField.text);
                if (onSuccessfullyConnect != null) onSuccessfullyConnect();
            }, delegate
            {
                EnableInputs(true);
            });
        });

        _createServerButton.onClick.AddListener(delegate
        {
            SocketInitializationData __socketData = new SocketInitializationData();
            __socketData.ipAddress = "127.0.0.1";
            __socketData.maxClients = 2;
            __socketData.port = 1300;
            Server.instance.InitializeServer(__socketData, delegate
            {
                StreamToServer.ConnectNewUser(_nickInputField.text);  
                if (onSuccessfullyConnect != null) onSuccessfullyConnect();
            });
        });

        _nickInputField.onValueChanged.AddListener(delegate (string p_value)
        {
            if (p_value != string.Empty)
            {
                _canConnect = true;                
                EnableInputs(true);
            }
            else
            {
                _canConnect = false;
                EnableInputs(false);
            }
        });
    }


}
