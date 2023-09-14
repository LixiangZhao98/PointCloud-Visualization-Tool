using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 相机围绕着目标点旋转、缩放。
/// 其中目标点为CameraRoot 
/// </summary>
[Serializable]
public struct CameraParameter
{
    public bool limitXAngle;
    public float minXAngle;
    public float maxXAngle;

    public bool limitYAngle;
    public float minYAngle;
    public float maxYAngle;

    public float orbitSensitive;
    public float mouseMoveRatio;

    public CameraParameter(bool limitXAngle = true
        , float minXAngle = 0f
        , float maxXAngle = 80f
        , bool limitYAngle = false
        , float minYAngle = 0f
        , float maxYAngle = 0f
        , float orbitSensitive = 10f
        , float mouseMoveRatio = 0.3f)
    {
        this.limitXAngle = limitXAngle;
        this.minXAngle = minXAngle;
        this.maxXAngle = maxXAngle;
        this.limitYAngle = limitYAngle;
        this.minYAngle = minYAngle;
        this.maxYAngle = maxYAngle;
        this.orbitSensitive = orbitSensitive;
        this.mouseMoveRatio = mouseMoveRatio;
    }

}

public class CameraController : MonoBehaviour
{

    private Vector3 lastMousePos;
    private Vector3 targetEulerAngle;
    private Vector3 eulerAngle;

    public CameraParameter freeOrbitParameter;
    private CameraParameter cureentCameraParameter;

    public Transform cameraRootTf;
    public Transform cameraTf;

    private float cameraDistance;
    private float targetCameraDistance;

    private float lastTouchDistance;

    public float minDistance = 5f;
    public float maxDistance = 30f;
    public float mouseScroollRatio = 1f;
    public float zoomSensitive = 1f;

    private Quaternion originalRotate;

    public float[] yMinAngles;
    public float[] yMaxAngles;
    public bool[] isAlreadyFire;

    void Start()
    {
        originalRotate = cameraRootTf.localRotation;
        cameraDistance = cameraTf.localPosition.z;
        targetCameraDistance = cameraDistance;
        cureentCameraParameter = freeOrbitParameter;
        isAlreadyFire = new bool[yMinAngles.Length];
    }

    void Update()
    {
        Oribit();
        Zoom();
    }

    private void Oribit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            targetEulerAngle.x += (-Input.mousePosition.y + lastMousePos.y) * cureentCameraParameter.mouseMoveRatio;
            targetEulerAngle.y += (Input.mousePosition.x - lastMousePos.x) * cureentCameraParameter.mouseMoveRatio;
            if (cureentCameraParameter.limitXAngle)
            {
                targetEulerAngle.x = Mathf.Clamp(targetEulerAngle.x, cureentCameraParameter.minXAngle,
                    cureentCameraParameter.maxXAngle);
            }
            if (cureentCameraParameter.limitYAngle)
            {
                targetEulerAngle.y = Mathf.Clamp(targetEulerAngle.y, cureentCameraParameter.minYAngle,
                    cureentCameraParameter.maxXAngle);
            }
            lastMousePos = Input.mousePosition;
        }
        if (Input.touchCount < 2)
        {
            eulerAngle = Vector3.Lerp(eulerAngle, targetEulerAngle,
                Time.fixedDeltaTime * cureentCameraParameter.orbitSensitive);
            cameraRootTf.rotation = originalRotate * Quaternion.Euler(eulerAngle);
        }
    }

    private void Zoom()
    {
        if (Input.touchCount < 2)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                cameraDistance = -cameraTf.localPosition.z;
                targetCameraDistance = cameraDistance -
                                       Input.GetAxis("Mouse ScrollWheel") * cameraDistance * mouseScroollRatio;
                targetCameraDistance = Mathf.Clamp(targetCameraDistance, minDistance, maxDistance);
            }
        }
        else
        {
            if (Input.GetTouch(1).phase == TouchPhase.Began)
            {
                lastTouchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            }
            if (Input.GetTouch(1).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                cameraDistance = -cameraTf.localPosition.z;
                targetCameraDistance = cameraDistance -
                                       (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) -
                                        lastTouchDistance) * mouseScroollRatio;
                lastMousePos = Input.mousePosition;
            }
        }
        if (Mathf.Abs(targetCameraDistance - cameraDistance) > 0.1f)
        {
            cameraDistance = Mathf.Lerp(cameraDistance, targetCameraDistance, Time.fixedDeltaTime * zoomSensitive);
            cameraTf.localPosition = new Vector3(0f, 0f, -cameraDistance);
        }
    }
}

