using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    [SerializeField] private float x_max_limit;
    [SerializeField] private float x_min_limit;
    [SerializeField] private float y_max_limit;
    [SerializeField] private float y_min_limit;
    [SerializeField] private float z_min_limit;
    [SerializeField] private float z_max_limit;

    private void Update()
    {
        transform.position = new Vector3 //Vector for Clamping the Camera
        (
        Mathf.Clamp(transform.position.x , x_min_limit, x_max_limit),
        Mathf.Clamp(transform.position.y, y_min_limit, y_max_limit),
        Mathf.Clamp(transform.position.z, z_min_limit,z_max_limit)
            );
    }
    
}
