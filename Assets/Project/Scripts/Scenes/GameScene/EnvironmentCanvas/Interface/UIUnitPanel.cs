using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIUnitPanelStateType
{
    NO_UNIT_SELECTED,
    UNIT_SELECTED,
}

public abstract class UIUnitPanel : UIPanel 
{
    #region Protected Serialized-Data
    [SerializeField] protected Image _background;
    [SerializeField] protected BoxCollider _inputBloker;
    #endregion

    #region Protected Data
    protected UIUnitPanelStateType _currentUnitPanelState;
    protected Entity _selectedEntity;
    protected Vector2 _dimensions;
    #endregion

    public override void Initialize()
    {
        base.Initialize();
        _currentUnitPanelState = UIUnitPanelStateType.NO_UNIT_SELECTED;
    }

    public virtual void AUpdate()
    {
    
    }

    protected Vector2 GetPanelDimensions()
    {
        return new Vector2(_background.GetComponent<RectTransform>().rect.width, _background.GetComponent<RectTransform>().rect.height);
    }

    public virtual void SetSelectedUnit(Entity p_unit)
    {
        if (_selectedEntity == p_unit) return;
        _selectedEntity = p_unit;
        _currentUnitPanelState = UIUnitPanelStateType.UNIT_SELECTED;
    }

    public virtual void DeselectUnit()
    {
        _selectedEntity = null;
        _currentUnitPanelState = UIUnitPanelStateType.NO_UNIT_SELECTED;
    }

    public virtual UIUnitPanelStateType GetUIPanelStateType()
    {
        return _currentUnitPanelState;
    }
}
