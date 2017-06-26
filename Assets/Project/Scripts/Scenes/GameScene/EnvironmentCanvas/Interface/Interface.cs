using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour 
{
    #region Events
    public event Action<string, CommandType, object[]> onClickCommand;
    #endregion

    [Header("Interface Panels")]
    [SerializeField] private UnitSelectionPanel _unitSelectionPanel;
    [SerializeField] private EntityDisplayPanel _entityDisplayPanel;
    [SerializeField] private ResourcesPanel _resourcesPanel;

    public void AInitialize()
    {
        InitializeSelectionPanel();
        InitializeEntityDisplayPanel();
    }

    private void InitializeSelectionPanel()
    {
        _unitSelectionPanel.Initialize();
        _unitSelectionPanel.onClickCommand += delegate (string p_entityID, CommandType p_commandType, object[] p_args)
        {
            if (onClickCommand != null) onClickCommand(p_entityID, p_commandType, p_args);
        };

    }

    private void InitializeEntityDisplayPanel()
    {
        _entityDisplayPanel.Initialize();
    }

    private void InitializeResourcesPanel()
    {
        _resourcesPanel.Initialize();
    }

    public void AUpdate()
    {
        _unitSelectionPanel.AUpdate();
        _entityDisplayPanel.AUpdate();
    }

    #region Panels
    public void SetSelectedUnit(bool p_isPlayer, Entity p_unit)
    {
        if (p_isPlayer == true)
        {
            _unitSelectionPanel.SetSelectedUnit(p_unit);        
        }
        _entityDisplayPanel.SetSelectedUnit(p_unit);
    }

    public void ShowDisplayBuildingUnitUI(bool p_enabled)
    {
        _entityDisplayPanel.ActivateEntityBuildedDisplay(p_enabled);
    }

    public void DeselectUnit(Entity p_unit)
    {
        _unitSelectionPanel.DeselectUnit();
        _entityDisplayPanel.DeselectUnit();
    }

    public void UpdateResourcesPanel(int p_crystalAmount)
    {
        _resourcesPanel.UpdateResources(p_crystalAmount);
    }
    #endregion
}
