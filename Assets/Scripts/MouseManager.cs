using System;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private Camera _cam;
    private const float PanSpeed = 50f;
    private const float ZoomSpeedMouse = 2f;

    private static readonly float[] BoundsX = new float[]{0, 1850f};
    private static readonly float[] BoundsZ = new float[]{0, 150f};
    private static readonly float[] ZoomBounds = new float[]{10f, 80f};
    
    private Vector3 _lastPanPosition;

    private void Awake() {
        _cam = GetComponent<Camera>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        HandleMouse();
    }

    private void HandleMouse() {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(1)) {
            _lastPanPosition = Input.mousePosition;
        } else if (Input.GetMouseButton(1)) {
            PanCamera(Input.mousePosition);
        }else if (Input.GetMouseButtonDown(0))
        {
            var ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.name);
            }
        }

        // Check for scrolling to zoom the camera
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);
    }

    private void PanCamera(Vector3 newPanPosition) {
        // Determine how much to move the camera
        var offset = _cam.ScreenToViewportPoint(_lastPanPosition - newPanPosition);
        var move = new Vector3(offset.y * PanSpeed, 0, -offset.x * PanSpeed);

        // Perform the movement
        transform.Translate(move, Space.World);  
    
        // Ensure the camera remains within bounds.
        var pos = _cam.transform.position;
        pos.x = Mathf.Clamp(pos.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(pos.z, BoundsZ[0], BoundsZ[1]);
        transform.position = pos;

        // Cache the position
        _lastPanPosition = newPanPosition;
    }

    private void ZoomCamera(float offset, float speed) {
        if (Math.Abs(offset) < 0.0001) {
            return;
        }

        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }
}