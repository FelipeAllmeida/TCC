using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class LobbyScreen : MonoBehaviour
{
    #region Private Serialized-Data
    [SerializeField] private Image _background;
    [SerializeField] private Text _headerText;
    #endregion

    #region Private Data
    private Dictionary<string, LobbyPlayerField> _dictLobbyPlayerField = new Dictionary<string, LobbyPlayerField>();
    private float _lastLobbyFieldPosY = 0f;
    private Vector2 _dimensions;
    #endregion

    public void Initialize()
    {
        _headerText.text = "Lobby";
        _dimensions = GetScreenDimensions();

    }

    public void EnableScreen(bool p_value)
    {
        gameObject.SetActive(p_value);
        ListenEvents(p_value);
    }

    public void EnableInputs(bool p_value)
    {
        
    }

    private Vector2 GetScreenDimensions()
    {
        return _background.rectTransform.rect.size;
    }

    private void ListenEvents(bool p_value)
    {
        Server.instance.onSocketResponse -= HandleOnSocketResponse;
        if (p_value == true)
        {
            Server.instance.onSocketResponse += HandleOnSocketResponse;
        }
    }

    private void HandleOnSocketResponse(string p_response)
    {
        JSONNode __output = JSON.Parse(p_response);
        foreach (var key in __output.Keys)
        {
            switch (key)
            {
                case "LobbyMembers":
                    HandleOnNewPlayerConnect(__output[key]);
                    break;
            }
        }
    }

    private void HandleOnNewPlayerConnect(JSONNode p_node)
    {
        for (int i = 0;i < p_node.Count;i++)
        {            
            string __userID = p_node[i]["id"];
            if (_dictLobbyPlayerField.ContainsKey(__userID) == false)
            {
                LobbyPlayerField __lobbyPlayerField = PoolManager.instance.Spawn(PoolType.LOBBY_PLAYER_FIELD).GetComponent<LobbyPlayerField>();
                __lobbyPlayerField.transform.SetParent(transform);
                __lobbyPlayerField.Intialize();
                Vector2 __size = new Vector2(_dimensions.x, _dimensions.y * 0.1f);
                Vector2 __position = new Vector2(0f, (_dimensions.y / 2f) - (__size.y * 0.5f) + (-__size.y  - (__size.y * 0.1f)) * (_dictLobbyPlayerField.Count + 1));
                __lobbyPlayerField.SetSize(__size);
                __lobbyPlayerField.SetPosition(__position);
                __lobbyPlayerField.SetData(p_node[i]["user"]);
                __lobbyPlayerField.gameObject.SetActive(true);
                _lastLobbyFieldPosY = __lobbyPlayerField.GetPosition().y;
                _dictLobbyPlayerField.Add(__userID, __lobbyPlayerField);
            }        
        }
    }
}
