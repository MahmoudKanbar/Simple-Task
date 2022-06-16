using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform earth;
    [SerializeField] private Transform player;
    [SerializeField] private float sensitivity;
    private Vector3 lastMousePosition;

    private void Start()
    {
        lastMousePosition = Input.mousePosition;
    }

    private void Update()
    {
        transform.position = player.position;
        
        FixCameraUp();
        RotateCamera();
    }

    private void RotateCamera()
    {
        var delta = Input.mousePosition - lastMousePosition;
        transform.rotation = Quaternion.AngleAxis(delta.x * sensitivity, transform.up) * transform.rotation;
        lastMousePosition = Input.mousePosition;
    }
    
    private void FixCameraUp()
    {
        var correctUp = (transform.position - earth.position).normalized;
        var currentUp = transform.up;
        var rotation = Quaternion.FromToRotation(currentUp, correctUp) * transform.rotation;
        transform.rotation = rotation;
    }
}
