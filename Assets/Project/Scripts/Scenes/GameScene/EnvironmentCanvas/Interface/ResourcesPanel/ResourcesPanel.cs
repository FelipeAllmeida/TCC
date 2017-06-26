using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesPanel : UIUnitPanel
{
    [SerializeField] private Text _resourceText;

    public override void Initialize()
    {
        base.Initialize();
        CalculateAndSetInputBlockerScaleAndPosition();
    }

    private void CalculateAndSetInputBlockerScaleAndPosition()
    {
        _inputBloker.size = new Vector3(_background.rectTransform.rect.width, _background.rectTransform.rect.height, 0f);
    }

    public void UpdateResources(int p_crystalAmount)
    {
        _resourceText.text = "Crystal: " + p_crystalAmount.ToString();
    }


}
