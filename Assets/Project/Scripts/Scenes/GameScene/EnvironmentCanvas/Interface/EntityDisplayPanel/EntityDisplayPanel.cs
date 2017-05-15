﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityDisplayPanel : UIUnitPanel
{
    #region Private Serialized-Data
    [Header("Entity Display Info")]
    [SerializeField] private GameObject _entityDisplay;
    [SerializeField] private Text _entityName;
    [SerializeField] private Image _entityImage;

    [Header("Entity Building Info")]
    [SerializeField] private GameObject _entityBuildedDisplay;
    [SerializeField] private Slider _entityBuildedSlider;
    [SerializeField] private Text _entityBuildedPercentage;
    [SerializeField] private Image _entityBuildedImage;
    #endregion

    #region Private Data
    #endregion

    public override void Initialize()
    {
        base.Initialize();
        CalculateAndSetInputBlockerScaleAndPosition();
        _entityDisplay.SetActive(false);
        _entityBuildedDisplay.SetActive(false);
    }

    public override void AUpdate()
    {
        if (_selectedEntity != null)
        {
            if (_selectedEntity.GetEntityType() == EntityType.BUILDING)
            {
                if ((_selectedEntity as EntityBuilding).GetIsBuilding() == true)
                {
                    ActivateEntityBuildedDisplay(true);
                    UpdateEntityBuildedDisplay();
                }
                else
                {
                    ActivateEntityBuildedDisplay(false);
                }               
            }
        }
    }

    private void CalculateAndSetInputBlockerScaleAndPosition()
    {
        _inputBloker.size = new Vector3(_background.rectTransform.rect.width, _background.rectTransform.rect.height, 0f);
    }

    public override void SetSelectedUnit(Entity p_entity)
    {
        if (_selectedEntity == p_entity) return;
        if (p_entity != null)
        {
            base.SetSelectedUnit(p_entity);
            _entityDisplay.SetActive(true);
            _entityName.text = p_entity.GetEntityName();

            if (p_entity.GetEntityType() == EntityType.BUILDING)
            {
                ActivateEntityBuildedDisplay((p_entity as EntityBuilding).GetIsBuilding());
            }
            else
            {
                ActivateEntityBuildedDisplay(false);
            }
        }
        else
        {
            DeselectUnit();
        }
    }

    public void ActivateEntityBuildedDisplay(bool p_active)
    {
        _entityBuildedDisplay.SetActive(p_active);
    }

    private void UpdateEntityBuildedDisplay()
    {
        float __percentage = (_selectedEntity as EntityBuilding).GetUnitBuildPercentage();
        _entityBuildedSlider.value = __percentage;
        _entityBuildedPercentage.text = (__percentage * 100f).ToString("0.00") + "%";
    }

    public override void DeselectUnit()
    {
        _entityDisplay.SetActive(false);
        _entityBuildedDisplay.SetActive(false);
        base.DeselectUnit();
    }
}
