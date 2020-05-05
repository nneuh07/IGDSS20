using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MouseManager : MonoBehaviour
{
    public Boolean primaryDown = false;
    public Boolean secondaryDown = false;
    public float speed = 500.0f;
    private void Update()
    {
        MouseInput(); //checks for Mouse clicks
        MouseMovement(); //checks the Mouse movement
        CamZoom();
        
    }

    void MouseMovement()
    {
        if (Input.GetAxis("Mouse X") > 0 & secondaryDown == true)
        {
            transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime*speed, 
                0.0f, Input.GetAxisRaw("Mouse Y")*Time.deltaTime*speed);
        }
        else if (Input.GetAxis("Mouse X") < 0 & secondaryDown == true)
        {
            transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime*speed, 
                0.0f, Input.GetAxisRaw("Mouse Y")*Time.deltaTime*speed);
        }
    }
    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            primaryDown = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            secondaryDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            primaryDown = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            secondaryDown = false;
        }
    }

    void CamZoom()
    {
        float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
        if (ScrollWheelChange != 0)
        {
            float R = ScrollWheelChange * 15; 
            float PosX = Camera.main.transform.eulerAngles.x + 90;
            float PosY = -1 * (Camera.main.transform.eulerAngles.y - 90);
            PosX = PosX / 180 * Mathf.PI; 
            PosY = PosY / 180 * Mathf.PI;
            float X = R * Mathf.Sin(PosX) * Mathf.Cos(PosY);
            float Z = R * Mathf.Sin(PosX) * Mathf.Sin(PosY);
            float Y = R * Mathf.Cos(PosX);
            float CamX = Camera.main.transform.position.x;
            float CamY = Camera.main.transform.position.y;
            float CamZ = Camera.main.transform.position.z;
            Camera.main.transform.position = new Vector3(CamX + X, CamY + Y, CamZ + Z);
        }

    }

}