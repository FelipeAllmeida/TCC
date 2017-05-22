using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignInScreen : MonoBehaviour 
{
    [SerializeField] private InputField _nickInputField;
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _createServerButton;

    public event Action onSuccessfullyConnect;

    public void Initialize()
    {
        ListenEvents();
    }

    public void EnableSignInScreen(bool p_value)
    {
        gameObject.SetActive(p_value);
    }

    public void EnableInputs(bool p_value)
    {
        _connectButton.interactable = p_value;
        _createServerButton.interactable = p_value;
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
            __socketData.maxClients = 4;
            __socketData.port = 1300;
            Server.instance.InitializeServer(__socketData, delegate
            {
                if (onSuccessfullyConnect != null) onSuccessfullyConnect();
            });
        });
    }


}
