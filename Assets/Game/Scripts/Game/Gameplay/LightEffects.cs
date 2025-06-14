using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightEffects : MonoBehaviour
{
    [SerializeField] private Light2D light;
    public float defaultInner = 14f;
    public float defaultOuter = 16f;
    public float defaultfalloffIntensity = 0.5f;

    public float lightChange = 2f;
    void Start()
    {
        //StartCoroutine(Test());
    }

    void Update()
    {

    }
    public IEnumerator Test()
    {
        while (true)
        {
            TickLightDecrease();
            yield return new WaitForSeconds(2f);
        }
    }
    public bool TickLightDecrease()
    {
        float falloffKoaf = 10f;
        if(light.pointLightInnerRadius / falloffKoaf <= 1)
        {
            light.falloffIntensity *= light.pointLightInnerRadius / falloffKoaf;
        }
        Debug.Log(light.pointLightInnerRadius);
        if (light.pointLightOuterRadius < lightChange * 2)
        {
            return false;
        }
        light.pointLightInnerRadius -= lightChange;
        light.pointLightOuterRadius -= lightChange;
        return true;
    }

    public void ResetLight()
    {
        light.pointLightInnerRadius = defaultInner;
        light.pointLightOuterRadius = defaultOuter;
        light.falloffIntensity = defaultfalloffIntensity;
    }
}
