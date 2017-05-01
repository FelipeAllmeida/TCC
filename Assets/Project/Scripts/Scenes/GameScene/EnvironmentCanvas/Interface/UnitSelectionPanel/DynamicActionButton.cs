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
    private Enum _enumType;
    #endregion

    public void ChangeButtonCommandType(Enum p_enumType)
    {
        _enumType = p_enumType;
        _button.onClick.AddListener(delegate
        {
            if (onClick != null)
                onClick(_enumType);
        });
    }

    public void Enable(bool p_isEnabled)
    {
        gameObject.SetActive(p_isEnabled);
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
