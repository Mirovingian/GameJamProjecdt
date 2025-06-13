using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Light2D _light;

    [SerializeField] private float _radius;


    public void SetLightRange(float radius)
    {
        _light.pointLightOuterRadius = radius;
        _light.pointLightInnerRadius = radius - 2.4f;
    }

}
