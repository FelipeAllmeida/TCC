using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class CameraManager : MonoBehaviour 
{

    #region Const Data
    private float _constCameraSpeedSensitivity = 1f;
    #endregion
    #region Private-Serialized Data
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _uiCamera;
    [SerializeField] private Camera _hintsCamera;
    [SerializeField] private Camera _3DOverlayCamera;
    [SerializeField] private Camera _backgroundCamera;
    #endregion

    #region Private Data
    private Vector3 _worldDimensions;
    private Vector3 _maxDragRestriction;
    private Vector3 _minDragRestriction;

    private TweenNodule _cameraPositionNodule;
    #endregion

    public void Initialize(Vector3 p_worldDimensions)
    {
        _mainCamera.transform.eulerAngles = new Vector3(45f, 0f, 0f);
        _worldDimensions = p_worldDimensions;
    }

    public void UpdateMainCameraPosition(Vector2 p_mousePosition)
    {
        float __screenWidth = Screen.width;
        float __screenHeight = Screen.height;

        bool __hasChange = false;

        Vector3 __newCameraPosition = _mainCamera.transform.position;
        if (p_mousePosition.x < __screenWidth * 0.05f)
        {
            __newCameraPosition += new Vector3(- _constCameraSpeedSensitivity, 0f, 0f);
            __hasChange = true;
        }
        else if (p_mousePosition.x > __screenWidth * 0.95f)
        {
            __newCameraPosition += new Vector3(_constCameraSpeedSensitivity, 0f, 0f);
            __hasChange = true;
        }

        if (p_mousePosition.y < __screenHeight * 0.05f)
        {
            __newCameraPosition += new Vector3(0f, 0f, - _constCameraSpeedSensitivity);
            __hasChange = true;
        }
        else if (p_mousePosition.y > __screenHeight * 0.95f)
        {
            __newCameraPosition += new Vector3(0f, 0f, _constCameraSpeedSensitivity);
            __hasChange = true;
        }

        if (__hasChange == true)
        {
            __newCameraPosition.x = Mathf.Clamp(__newCameraPosition.x, 0f, _worldDimensions.x);
            __newCameraPosition.z = Mathf.Clamp(__newCameraPosition.z, 0f, _worldDimensions.z);
            __newCameraPosition.y = 10f;
            Vector3 __startCameraPosition = _mainCamera.transform.position;
            _cameraPositionNodule = ATween.Vector3To(__startCameraPosition, __newCameraPosition, 0.1f, TweenEase.LINEAR, delegate (Vector3 p_value)
            {
                _mainCamera.transform.position = p_value;
            });
        }
        //__cameraPos.y = 7.5f;
    }
}
