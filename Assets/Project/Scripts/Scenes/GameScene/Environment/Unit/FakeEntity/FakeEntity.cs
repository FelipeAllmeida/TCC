using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEntity : MonoBehaviour 
{
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
