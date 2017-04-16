using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GestureType
{
    DRAG,
    PINCH
}

public enum GesturePhaseType
{
    START,
    UPDATE,
    FINISH
}

public class InputInfo
{
    public Unit hit;
    public Vector3 worldClickPoint;
    public Vector3 screenPointClick;
}

public class InputManager 
{
    public event Action<InputInfo> onMouseLeftClick;
    public event Action<InputInfo> onMouseRightClick;

    private GestureType _currenteMouseGesture;

    private Dictionary<int, InputInfo> _dictInputs = new Dictionary<int, InputInfo>();

    private bool _isInputsEnabled = true;

    public void EnableInputs(bool p_value)
    {
        _isInputsEnabled = p_value;
        if (p_value == false)
        {
            _dictInputs.Clear();
        }
    }

    public void CheckInput()
    {
        HandleMouseInputs();
    }

    public Vector3 GetMousePosition()
    {
        return Input.mousePosition;
    }

    private void HandleMouseInputs()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            HandleMouseClick(0, GesturePhaseType.START);
        }
        else if (Input.GetMouseButton(0) == true)
        {
            HandleMouseClick(0, GesturePhaseType.UPDATE);
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            HandleMouseClick(0, GesturePhaseType.FINISH);
        }

        if (Input.GetMouseButtonDown(1) == true)
        {
            HandleMouseClick(1, GesturePhaseType.START);
        }
        else if (Input.GetMouseButton(1) == true)
        {
            HandleMouseClick(1, GesturePhaseType.UPDATE);
        }
        else if (Input.GetMouseButtonUp(1) == true)
        {
            HandleMouseClick(1, GesturePhaseType.FINISH);
        }
    }

    private void HandleMouseClick(int p_inputID, GesturePhaseType p_phase)
    {
        if (p_phase == GesturePhaseType.FINISH)
        {
            if (_dictInputs.ContainsKey(p_inputID))
            {
                _dictInputs.Remove(p_inputID);
            }
        }
        else if (p_phase == GesturePhaseType.UPDATE)
        {
            UpdateInputInfo(p_inputID);
        }
        else if (p_phase == GesturePhaseType.START)
        {
            if (_dictInputs.ContainsKey(p_inputID) == false)
            {
                InputInfo __inputInfo = CreateInputInfo();
                _dictInputs.Add(p_inputID, __inputInfo);
                if (p_inputID == 0)
                {
                    if (onMouseLeftClick != null) onMouseLeftClick(__inputInfo);
                }
                else if (p_inputID == 1)
                {
                    if (onMouseRightClick != null) onMouseRightClick(__inputInfo);
                }
            }
        }
    }

    private InputInfo CreateInputInfo()
    {
        InputInfo __inputInfo = new InputInfo();
        RaycastHit __raycastHit = new RaycastHit();
        Ray __ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(__ray, out __raycastHit, 1000))
        {
            __inputInfo.hit = __raycastHit.collider.gameObject.GetComponent<Unit>();
            __inputInfo.worldClickPoint = __raycastHit.point;
            __inputInfo.screenPointClick = Input.mousePosition;
        }
        return __inputInfo;
    }

    private void UpdateInputInfo(int p_inputID)
    {
        if (_dictInputs.ContainsKey(p_inputID) == true)
        {
            _dictInputs[p_inputID].screenPointClick = Input.mousePosition;
        }
    }
}
