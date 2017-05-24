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
   
    #endregion

    #region Private Data
    private List<DynamicActionButton> _listDynamicActionButtons;

    private Vector2 _actionButtonDimensions = new Vector2(35f, 35f);
    #endregion

    public override void Initialize()
    {
        base.Initialize();
        _dimensions = GetPanelDimensions();
        CalculateAndSetDynamicActionButtonsPositions();
        CalculateAndSetInputBlockerScaleAndPosition();
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
        if (_selectedEntity == p_unit) return;
        if (p_unit != null)
        {
            base.SetSelectedUnit(p_unit);
            List<CommandType> __listCommands = _selectedEntity.GetListAvaiableCommands();

            List<string> __listActions = new List<string>();
            
            foreach (var k in __listCommands)
            {
                __listActions.Add(k.ToString());
            }
            InitializeDynamicActionButtons(__listActions);
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



    private void CalculateAndSetDynamicActionButtonsPositions()
    {
        _listDynamicActionButtons = new List<DynamicActionButton>();
        float __width = _dimensions.x;
        float __height = _dimensions.y;

        float __widthOffset = __width * 0.1f;
        float __heightOffset = __height * 0.1f;       

        int __buttonsAxisX = Mathf.FloorToInt(__width / (_actionButtonDimensions.x + __widthOffset));
        int __buttonsAxisY = Mathf.FloorToInt(__height / (_actionButtonDimensions.y + __heightOffset));
        
        for (int i = 0; i < __buttonsAxisY + 1; i++)
        {
            for (int j = 0; j < __buttonsAxisX + 1; j++)
            {
                DynamicActionButton __actionButton = PoolManager.instance.Spawn(PoolType.DYNAMIC_ACTION_BUTTON, transform).GetComponent<DynamicActionButton>();
                __actionButton.Enable(false);
                __actionButton.onClick += HandleOnActionButtonClick;

                float __buttonPosX = _background.GetComponent<RectTransform>().rect.min.x + (__widthOffset + _actionButtonDimensions.x / 2f) * (j + 1f);
                float __buttonPosY = _background.GetComponent<RectTransform>().rect.max.y - (__heightOffset + _actionButtonDimensions.y / 2f) * (i + 1f);
                Vector2 __buttonPos = new Vector2(__buttonPosX, __buttonPosY);

                 
                __actionButton.GetRectTransform().anchoredPosition = __buttonPos;
                _listDynamicActionButtons.Add(__actionButton);
            }
        }
    }

    private void CalculateAndSetInputBlockerScaleAndPosition()
    {
        _inputBloker.size = new Vector3(_background.rectTransform.rect.width, _background.rectTransform.rect.height, 0f);
    }

    private void InitializeDynamicActionButtons(List<string> p_listActionType)
    {
        for (int i = 0; i < _listDynamicActionButtons.Count; i++)
        {
            if (i < p_listActionType.Count)
            {
                _listDynamicActionButtons[i].ChangeButtonCommandType(p_listActionType[i]);
                //_listDynamicActionButtons[i].onClick += HandleOnActionButtonClick;
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
        for (int i = 0; i < _listDynamicActionButtons.Count; i++)
        {
            _listDynamicActionButtons[i].Enable(false);
        }
    }

    private void HandleOnActionButtonClick(string p_actionType)
    {
        Debug.Log("HandleOnActionButtonClick: " + p_actionType);
        Debug.Log("is command: " + Enum.IsDefined(typeof(CommandType), p_actionType));
        if (Enum.IsDefined(typeof(CommandType), p_actionType) == true)
        {
            CommandType __commandType = (CommandType)Enum.Parse(typeof(CommandType), p_actionType);
            Debug.Log("CommandType: " + __commandType);
            switch (__commandType)
            {
                case CommandType.BUILD: 
                        InitializeDynamicActionButtons(_selectedEntity.GetListAvaiableEntities()); 
                    break;
                case CommandType.MOVE:
                case CommandType.NONE:
                    if (onClickCommand != null)
                        onClickCommand(_selectedEntity.GetEntityID(), __commandType, null);
                    break;
            }
        }
        else
        {            
            object[] __args = new object[] { p_actionType };
            if (onClickCommand != null) onClickCommand(_selectedEntity.GetEntityID(), CommandType.BUILD, __args);           
        }
    }
}
