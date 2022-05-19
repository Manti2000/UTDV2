using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //How fast the camera moves in tiles/second
    public float cameraHeight;
    public float cameraMoveSpeed = 10;
    public float ZoomAmount = 2;
    public int detectionRange;
    public  float minZoomHeight;
    public float maxZoomHeight;

    public bool mouseMovement = true;
    public bool arrowMovement = true;

    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = cameraMoveSpeed * Time.deltaTime;

        //Camera movement
        MoveWithArrows(speed);
        MoveWithMouse(speed);
        Zoom(ZoomAmount);

        //transform.position = new Vector3(transform.position.x, cameraHeight, transform.position.z);

    }


    void MoveWithArrows(float speed)
    {
        if (arrowMovement)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.localPosition += Vector3.left * speed;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.localPosition += Vector3.right * speed;
            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                transform.localPosition += Vector3.forward * speed;
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                transform.localPosition += Vector3.back * speed;
            }
        }
    }

    void MoveWithMouse(float speed)
    {
        if (mouseMovement)
        {
            Vector2 mousePos = Input.mousePosition;
            float width = Screen.width;
            float height = Screen.height;

            if (mousePos.x <= detectionRange)
            {
                transform.localPosition += Vector3.left * speed;
            }

            if (mousePos.x >= width - detectionRange)
            {
                transform.localPosition += Vector3.right * speed;
            }

            if (mousePos.y >= height - detectionRange)
            {
                transform.localPosition += Vector3.forward * speed;
            }

            if (mousePos.y <= detectionRange)
            {
                transform.localPosition += Vector3.back * speed;
            }
        }
    }

    void Zoom(float zoomAmount)
    {
        float scroll = Input.mouseScrollDelta.y;

        if(scroll != 0)
        {

            if (scroll < 0)
            {
                if (Vector3.Distance(transform.position, new Vector3(transform.position.x, minZoomHeight, transform.position.y)) < zoomAmount)
                {
                    transform.position = new Vector3(transform.position.x, minZoomHeight, transform.position.z);
                    return;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, new Vector3(transform.position.x, maxZoomHeight, transform.position.y)) < zoomAmount)
                {
                    transform.position = new Vector3(transform.position.x, maxZoomHeight, transform.position.z);
                    return;
                }
            }

            Vector3 directionToZoom = (cam.ScreenToWorldPoint(new Vector3(Screen.width / 2F, Screen.height / 2F, 1)) - cam.transform.position);

            transform.position += scroll * ZoomAmount * directionToZoom;

        }
    }
}
