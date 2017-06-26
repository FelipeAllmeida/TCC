using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityDisplayPanel : UIUnitPanel
{
    #region Private Serialized-Data
    [Header("Entity Display Info")]
    [SerializeField] private GameObject _entityDisplay;
    [SerializeField] private Text _entityName;
    [SerializeField] private Text _entityHealth;
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
                ActivateEntityBuildedDisplay(_selectedEntity.GetIsBuilding());
                UpdateEntityBuildedDisplay();
            }
            else
            {
                ActivateEntityBuildedDisplay(false);
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
            _entityHealth.text = string.Format("{0} / {1}", p_entity.GetCurrentHealth(), p_entity.GetMaxHealth());
            if (p_entity.GetEntityType() == EntityType.BUILDING)
            {                
                ActivateEntityBuildedDisplay(p_entity.GetIsBuilding());
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
        float __percentage = _selectedEntity.GetUnitBuildPercentage();
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
