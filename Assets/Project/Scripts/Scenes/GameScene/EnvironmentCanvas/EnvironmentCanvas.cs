using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework;

public class EnvironmentCanvas : MonoBehaviour 
{
    #region Events
    public event Action<string, CommandType, object[]> onClickInterfaceCommand;
    #endregion

    #region Private Serialized-Data
    [SerializeField] private Interface _interface;
    [SerializeField] private Image _background;
    #endregion

    #region Private Data
    private Vector2 _canvasDimensions;
    #endregion

    public void AInitialize()
    {
        InitializeInterface();
       //Debug.Log(GetComponent<RectTransform>().sizeDelta);
       // Debug.Log(GetComponent<RectTransform>().rect.width + " | " + GetComponent<RectTransform>().rect.height);
    }

    private void InitializeInterface()
    {
        _interface.AInitialize();
        _interface.onClickCommand += delegate (string p_entityID, CommandType p_commandType, object[] p_args)
        {
            if (onClickInterfaceCommand != null) onClickInterfaceCommand(p_entityID, p_commandType, p_args);
        };
    }

    public void RemoveInitializationBackground()
    {
        TweenNodule __nodule = ATween.FloatTo(1f, 0f, 1.5f, TweenEase.LINEAR, delegate (float p_value)
        {
            Color __col = _background.color;
            __col.a = p_value;
            _background.color = __col;
        });
        __nodule.onFinished += delegate
        {
            _background.gameObject.SetActive(false);
        };
    }

    public void AUpdate()
    {
        _interface.AUpdate();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    #region Interface
    public void SetInterfaceSelectedUnit(bool p_isPlayer, Entity p_unit)
    {
        _interface.SetSelectedUnit(p_isPlayer, p_unit);
    }

    public void UpdateResourcesPanel(int p_crystalAmount)
    {
        _interface.UpdateResourcesPanel(p_crystalAmount);
    }
    #endregion
}
