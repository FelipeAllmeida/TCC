using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEntity : MonoBehaviour
{
    #region Private Data
    private int _constructorID;
    private string _entityType;
    #endregion

    public void AInitialize(int p_constructorID, string p_type)
    {
        _constructorID = p_constructorID;
        _entityType = p_type;
    }

    public int GetConstructorID()
    {
        return _constructorID;
    }

    public string GetEntityType()
    {
        return _entityType;
    }

    public void AUpdate()
    {
        UpdateFakeEntityPosition();
    }

    private void UpdateFakeEntityPosition()
    {
        Vector3 __position = gameObject.transform.position;
        RaycastHit __raycastHit = new RaycastHit();
        Ray __ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(__ray, out __raycastHit, 100))
        {
            __position = __raycastHit.point;
            __position.y += GetComponent<Renderer>().bounds.extents.y;
        }
        gameObject.transform.position = __position;
    }
}
