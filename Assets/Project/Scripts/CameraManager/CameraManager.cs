﻿using System.Collections;
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
        _uiCamera.transform.eulerAngles = new Vector3(45f, 0f, 0f);
        _worldDimensions = p_worldDimensions;
    }

    public void SetCameraPosToObject(Vector3 p_target)
    {
        Vector3 __newCameraPosition;
        __newCameraPosition.x = Mathf.Clamp(p_target.x, -_worldDimensions.x / 2f, _worldDimensions.x / 2f);
        __newCameraPosition.z = Mathf.Clamp(p_target.z - 10f, -_worldDimensions.z / 2f, _worldDimensions.z / 2f);
        __newCameraPosition.y = 10f;
        Vector3 __startCameraPosition = _mainCamera.transform.position;
        _cameraPositionNodule = ATween.Vector3To(__startCameraPosition, __newCameraPosition, 0.1f, TweenEase.LINEAR, delegate (Vector3 p_value)
        {
            _mainCamera.transform.position = p_value;
            _uiCamera.transform.position = p_value;
        });
    }

    public void UpdateMainCameraPosition()
    {
        Vector2 __mousePosition = Input.mousePosition;
        float __screenWidth = Screen.width;
        float __screenHeight = Screen.height;

        bool __hasChange = false;

        Vector3 __newCameraPosition = _mainCamera.transform.position;
        if (__mousePosition.x < __screenWidth * 0.05f && __mousePosition.x > 0f)
        {
            __newCameraPosition += new Vector3(- _constCameraSpeedSensitivity, 0f, 0f);
            __hasChange = true;
        }
        else if (__mousePosition.x > __screenWidth * 0.95f && __mousePosition.x < __screenWidth)
        {
            __newCameraPosition += new Vector3(_constCameraSpeedSensitivity, 0f, 0f);
            __hasChange = true;
        }

        if (__mousePosition.y < __screenHeight * 0.05f && __mousePosition.y > 0f)
        {
            __newCameraPosition += new Vector3(0f, 0f, - _constCameraSpeedSensitivity);
            __hasChange = true;
        }
        else if (__mousePosition.y > __screenHeight * 0.95f && __mousePosition.y < __screenHeight)
        {
            __newCameraPosition += new Vector3(0f, 0f, _constCameraSpeedSensitivity);
            __hasChange = true;
        }

        if (__hasChange == true)
        {
            __newCameraPosition.x = Mathf.Clamp(__newCameraPosition.x, -_worldDimensions.x / 2f, _worldDimensions.x/2f);
            __newCameraPosition.z = Mathf.Clamp(__newCameraPosition.z, - _worldDimensions.z / 2f, _worldDimensions.z/2f);
            __newCameraPosition.y = 10f;
            Vector3 __startCameraPosition = _mainCamera.transform.position;
            _cameraPositionNodule = ATween.Vector3To(__startCameraPosition, __newCameraPosition, 0.1f, TweenEase.LINEAR, delegate (Vector3 p_value)
            {
                _mainCamera.transform.position = p_value;
                _uiCamera.transform.position = p_value;
            });
        }
        //__cameraPos.y = 7.5f;
    }
}
