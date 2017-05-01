using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicActionButton : MonoBehaviour
{
    #region Private Serialized-Data
    [Header("Images")]
    [SerializeField] private Image _actionImage;
    [SerializeField] private Image _backgroundImage;

    [Header("Button")]
    [SerializeField] private Button _button;

    [Header("Transform")]
    [SerializeField] private RectTransform _rectTransform;
    #endregion



    #region Event Data
    public event Action<Enum> onClick;
    #endregion

    #region Private Data
    private Enum _actionType;
    #endregion

    public void ChangeButtonCommandType(Enum p_actionType)
    {
        Debug.Log("ChangeButtonCommandType: " + p_actionType);
        _actionType = p_actionType;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(delegate
        {
            Debug.Log("wtf");
            if (onClick != null) onClick(_actionType);
        });
    }

    public void Enable(bool p_isEnabled)
    {
        gameObject.SetActive(p_isEnabled);
        SetInteractable(p_isEnabled);
    }

    public void SetInteractable(bool p_isInteractable)
    {
        _button.interactable = p_isInteractable;
    }

    public void ResetButton()
    {
        _button.onClick.RemoveAllListeners();
    }

    public RectTransform GetRectTransform()
    {
        return _rectTransform;
    }
}
