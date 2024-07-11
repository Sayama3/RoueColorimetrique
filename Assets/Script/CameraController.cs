using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10;
    [SerializeField] private float m_ScrollSpeed = 10;

    private Camera _camera;
    private bool wasPress = false;
    private Vector3 _previousPoint;
    private Vector3 _previousPressPoint;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        KeyboardController();
        // bool isPress = Input.GetButton("Fire1");
        //
        // if (isPress || wasPress)
        // {
        //     MouseController();
        // }
        // else
        // {
        //     KeyboardController();
        // }
    }

    void KeyboardController()
    {
        
        float h = Input.GetAxisRaw("Horizontal") * m_Speed * Time.deltaTime;
        float v = Input.GetAxisRaw("Vertical") * m_Speed * Time.deltaTime;
        float z = Input.GetAxisRaw("Mouse ScrollWheel") * m_ScrollSpeed;

        Vector3 mv = h * transform.right + v * transform.up + z * transform.forward;
        transform.position += mv;
    }

    void MouseController()
    {
        bool isPress = Input.GetButton("Fire1");

        if (!isPress)
        {
            wasPress = false;
            return;
        }
        
        // Camera Mouse
        Ray rayScreenToWorld = _camera.ViewportPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        bool result = plane.Raycast(rayScreenToWorld, out float distance);
        Assert.IsTrue(result, "The ray didn't went trough the ground.");
        if (!result) return;

        Vector3 worldMouse = rayScreenToWorld.GetPoint(distance);
        if(!wasPress)
        {
            wasPress = isPress;
            _previousPoint = Input.mousePosition;
            _previousPressPoint = worldMouse;
        }
        else
        {
            if (_previousPoint != Input.mousePosition)
            {
                // var auto = transform.localToWorldMatrix * (worldMouse - _previousPressPoint);
                //auto /= auto.w;
                // transform.position += new Vector3(auto.x, auto.y, auto.z);
                transform.position += (worldMouse - _previousPressPoint);
                _previousPoint = Input.mousePosition;
                _previousPressPoint = worldMouse;
            }
        }
    }
}