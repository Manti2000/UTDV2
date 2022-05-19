using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask rayMask;
    Camera cam;

    bool towerPlacingMode = false;
    Vector3 towerPos;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) 
        {
            towerPlacingMode = !towerPlacingMode;
        }

        if (towerPlacingMode)
        {
            if (RayCastFromScreen(out Vector3 hitPoint))
            {

                towerPos = References.map.GetWorldPosition(hitPoint);
                
            }
        }

        //Debug.DrawLine(hitPoint, hitPoint + Vector3.up / 2, Color.red);

        //Debug.DrawLine(towerPos + Vector3.right / 2, towerPos - Vector3.right / 2, Color.yellow);
        //Debug.DrawLine(towerPos + Vector3.forward / 2, towerPos - Vector3.forward / 2, Color.yellow);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(towerPos.x, towerPos.y, towerPos.z), new Vector3(1, 0, 1));
    }


    bool RayCastFromScreen(out Vector3 hitPoint)
    {
        Vector2 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.1F));

        RaycastHit rayHit;
        
        if (Physics.Raycast(cam.transform.position, worldPos - transform.position, out rayHit, Mathf.Infinity, rayMask))
        {
            hitPoint = rayHit.point;
            return true;
        }

        hitPoint = Vector3.zero;
        return false;
    }
}
