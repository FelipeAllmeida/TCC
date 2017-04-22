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
    #endregion

    #region Event Data
    public event Action<CommandType> onClick;
    #endregion

    #region Private Data
    private CommandType _commandType;
    #endregion

    public void Initialize(CommandType p_commandType)
    {
        _commandType = p_commandType;
        _button.onClick.AddListener(delegate
        {
            if (onClick != null)
                onClick(_commandType);
        });
    }

    public void ResetButton()
    {
        _button.onClick.RemoveAllListeners();
    }
}
