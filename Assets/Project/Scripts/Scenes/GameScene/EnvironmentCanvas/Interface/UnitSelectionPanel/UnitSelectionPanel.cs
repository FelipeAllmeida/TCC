using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitSelectionPanel : UIUnitPanel 
{
    #region Private Data
    private List<DynamicActionButton> _listDynamicActionButtons;
    #endregion

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Enable()
    {
        base.Enable();
    }

    public override void Disable()
    {
        base.Disable();
    }

    public override void SetSelectedUnit(Unit p_unit)
    {
        base.SetSelectedUnit(p_unit);
        InitializeDynamicActionButtons();
    }

    public override void DeselectUnit()
    {
        ClearDynamicActionButtonsList();
        base.DeselectUnit();
    }

    private void InitializeDynamicActionButtons()
    {
        _listDynamicActionButtons = new List<DynamicActionButton>();
        List<CommandType> _listUnitCommands = _selectedUnit.GetListAvaiableCommands();

        for (int i = 0;i < _listUnitCommands.Count;i++)
        {
            DynamicActionButton __actionButton = PoolManager.instance.Spawn(PoolType.DYNAMIC_ACTION_BUTTON).GetComponent<DynamicActionButton>();
            __actionButton.Initialize(_listUnitCommands[i]);
            __actionButton.onClick += HandleOnActionButtonClick;
            _listDynamicActionButtons.Add(__actionButton);
        }
    }

    private void ClearDynamicActionButtonsList()
    {
        for (int i = 0;i < _listDynamicActionButtons.Count;i++)
        {
            _listDynamicActionButtons[i].ResetButton();
            PoolManager.instance.Despawn(PoolType.DYNAMIC_ACTION_BUTTON, _listDynamicActionButtons[i].gameObject);
        }
    }

    private void HandleOnActionButtonClick(CommandType p_commandType)
    {
        switch (p_commandType)
        {
            case CommandType.BUILD:
                break;
            case CommandType.MOVE:
                break;
        }
    }
}
