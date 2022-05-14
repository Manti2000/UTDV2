using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask rayMask;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) 
        {

            GameObject obj = RayCastFromScreen();

            if (obj != null) {

                Tile tile = obj.GetComponent<Tile>();

                if (tile != null && tile.type == TileType.Plot)
                {
                    if (tile.tower == null)
                    {
                        //References.map.BuildTower(TowerType.Base, tile.tileX, tile.tileY);
                    }
                    else
                    {
                        //References.map.DestroyTower(tile.tileX, tile.tileY);
                    }
                }
            }
        }
       

    }



    GameObject RayCastFromScreen()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.1F));

        RaycastHit rayHit;
        Physics.Raycast(cam.transform.position, worldPos - transform.position, out rayHit, Mathf.Infinity, rayMask);

        if (rayHit.collider != null)
        {
            return rayHit.collider.gameObject;
        }

        return null;
    }
}
