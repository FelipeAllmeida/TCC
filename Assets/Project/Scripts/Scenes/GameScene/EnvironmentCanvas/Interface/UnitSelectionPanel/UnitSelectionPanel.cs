using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitSelectionPanel : UIUnitPanel 
{
    #region Events
    public event Action<int, CommandType, object[]> onClickCommand; 
    #endregion

    #region Private Serialized-Data
    [SerializeField] private Image _background;
    #endregion

    #region Private Data
    private List<DynamicActionButton> _listDynamicActionButtons;

    private Vector2 _dimensions;
    private Vector2 _actionButtonDimensions = new Vector2(35f, 35f);
    #endregion

    public override void Initialize()
    {
        base.Initialize();
        _dimensions = GetPanelDimensions();
        CalculateAndSetDynamicActionButtonsPositions();
    }

    public override void Enable()
    {
        base.Enable();
    }

    public override void Disable()
    {
        base.Disable();
    }

    public override void SetSelectedUnit(Entity p_unit)
    {
        if (_selectedUnit == p_unit) return;
        if (p_unit != null)
        {
            base.SetSelectedUnit(p_unit);
            InitializeDynamicActionButtons();
        }
        else
        {
            DeselectUnit();
        }
    }

    public override void DeselectUnit()
    {
        DisableAllDynamicActionButtons();
        base.DeselectUnit();
    }

    private Vector2 GetPanelDimensions()
    {
        return new Vector2(_background.GetComponent<RectTransform>().rect.width, _background.GetComponent<RectTransform>().rect.height);
    }

    private void CalculateAndSetDynamicActionButtonsPositions()
    {
        _listDynamicActionButtons = new List<DynamicActionButton>();
        float __width = _dimensions.x;
        float __height = _dimensions.y;

        float __widthOffset = __width * 0.1f;
        float __heightOffset = __height * 0.1f;

       

        int __buttonsAxisX = Mathf.FloorToInt(__width / (_actionButtonDimensions.x + __widthOffset));
        int __buttonsAxisY = Mathf.FloorToInt(__height / (_actionButtonDimensions.y + __heightOffset));
        
        for (int i = 0; i < __buttonsAxisX + 1; i++)
        {
            for (int j = 0; j < __buttonsAxisY + 1; j++)
            {
                DynamicActionButton __actionButton = PoolManager.instance.Spawn(PoolType.DYNAMIC_ACTION_BUTTON, transform).GetComponent<DynamicActionButton>();
                __actionButton.Enable(false);
                __actionButton.onClick += HandleOnActionButtonClick;

                float __buttonPosX = _background.GetComponent<RectTransform>().rect.min.x + (__widthOffset + _actionButtonDimensions.x / 2f) * (i + 1f);
                float __buttonPosY = _background.GetComponent<RectTransform>().rect.max.y - (__heightOffset + _actionButtonDimensions.y / 2f) * (j + 1f);
                Vector2 __buttonPos = new Vector2(__buttonPosX, __buttonPosY);

                 
                __actionButton.GetRectTransform().anchoredPosition = __buttonPos;
                _listDynamicActionButtons.Add(__actionButton);
            }
        }
    }

    private void InitializeDynamicActionButtons()
    {
        List<CommandType> __listCommands = _selectedUnit.GetListAvaiableCommands();
        for (int i = 0; i < _listDynamicActionButtons.Count; i++)
        {
            if (i < __listCommands.Count)
            {
                _listDynamicActionButtons[i].ChangeButtonCommandType(__listCommands[i]);
                _listDynamicActionButtons[i].onClick += HandleOnActionButtonClick;
                _listDynamicActionButtons[i].Enable(true);
            }
            else
            {
                _listDynamicActionButtons[i].Enable(false);
            }
        }
    }

    private void DisableAllDynamicActionButtons()
    {
        for (int i = 0;i < _listDynamicActionButtons.Count;i++)
        {
            _listDynamicActionButtons[i].Enable(false);
        }
    }

    private void HandleOnActionButtonClick(Enum p_enumType)
    {     
        if (p_enumType is CommandType)
        {
            CommandType __commandType = (CommandType)p_enumType;
            switch (__commandType)
            {
                case CommandType.BUILD:
                    break;
                case CommandType.MOVE:
                case CommandType.NONE:
                    if (onClickCommand != null) onClickCommand(_selectedUnit.GetEntityID(), __commandType, null);
                    break;
            }
        }

        if (p_enumType is EntityUnitType)
        {
            if (onClickCommand != null) onClickCommand(_selectedUnit.GetEntityID(), CommandType.BUILD, null);
        }

        if (p_enumType is EntityBuildingType)
        {
            if (onClickCommand != null) onClickCommand(_selectedUnit.GetEntityID(), CommandType.BUILD, null);
        }       
    }
}
