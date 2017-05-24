using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerField : MonoBehaviour
{
    #region Private Serialized-Data
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _background;
    [SerializeField] private Image _divisor;

    [SerializeField] private Text _userNameText;
    #endregion

    #region Private Data
    private const float _constDivisorWidth = 5.557f;
    #endregion

    #region Initialization Methods
    public void Intialize()
    {
        SetScreenColors();
        _rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void SetScreenColors()
    {
    
    }
    #endregion

    #region Set Methods
    public void SetPosition(Vector2 p_screenPosition)
    {
        _rectTransform.anchoredPosition = p_screenPosition;
    }

    public Vector2 GetPosition()
    {
        return _rectTransform.anchoredPosition;
    }

    public void SetSize(Vector2 p_size)
    {
        _rectTransform.sizeDelta = p_size;

        float __offsetX = p_size.x * 0.01f;

        Vector2 __divisorSize = new Vector2(_constDivisorWidth, p_size.y);
        Vector2 __userNameTextSize = new Vector2(p_size.x * 0.4f, p_size.y);

        _divisor.rectTransform.sizeDelta = __divisorSize;
        _userNameText.rectTransform.sizeDelta = __userNameTextSize;

        Vector2 __userNameTextPosition = new Vector2(-p_size.x / 2 + __userNameTextSize.x / 2 + __offsetX, 0f);
        Vector2 __divisorPosition = new Vector2(-p_size.x / 2 + __userNameTextSize.x + __offsetX + _constDivisorWidth / 2, 0f);

        _userNameText.rectTransform.anchoredPosition = __userNameTextPosition;
        _divisor.rectTransform.anchoredPosition = __divisorPosition;
    }

    public RectTransform GetRectTransform()
    {
        return _rectTransform;
    }

    public void SetData(string p_user)
    {
        _userNameText.text = p_user;
    }
    #endregion
}
