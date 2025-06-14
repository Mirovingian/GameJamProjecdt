using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    private float pulseInterval = 1.0f;
    private float sizeChange = 3.0f;
    private float firstPulseDuration = 0.3f;
    private float secondPulseDuration = 0.5f;

    private AnimationCurve pulseCurveFirst = new AnimationCurve(
        new Keyframe(0f, 0f, 3f, 3f),      // Резкое начало
        new Keyframe(0.7f, 1f),             // Максимальное уменьшение
        new Keyframe(1f, 0.7f)              // Подготовка ко второму удару
    );

    private AnimationCurve pulseCurveSecond = new AnimationCurve(
        new Keyframe(0f, 0.3f),           // Продолжение с предыдущей фазы
        new Keyframe(0.3f, 0.8f),          // Второй удар (меньше первого)
        new Keyframe(1f, 0f, 0.5f, 0.5f)  // Плавный спад
    );

    private AnimationCurve pulseCurveBack = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 0.3f),   // Плавное начало
        new Keyframe(1f, 1f)               // Полное восстановление
    );

    private CinemachineVirtualCamera _virtualCam;
    private float _defaultSize;
    private bool _isEffectActive;
    private Coroutine _pulseCoroutine;

    private void Start()
    {
        _virtualCam = GetComponent<CinemachineVirtualCamera>();
        if (_virtualCam == null)
        {
            Debug.LogError("CinemachineVirtualCamera can't be found");
            return;
        }

        _defaultSize = _virtualCam.m_Lens.OrthographicSize;
    }

    public void SetHeartbeatEffect(bool isActive)
    {
        if(_isEffectActive && isActive)
        {
            return;
        }
        _isEffectActive = isActive;

        if (_isEffectActive)
        {
            if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = StartCoroutine(PulseRoutine());
        }
        else
        {
            if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
            _virtualCam.m_Lens.OrthographicSize = _defaultSize;
        }
    }

    private IEnumerator PulseRoutine()
    {
        while (_isEffectActive)
        {
            yield return Pulse(0.7f * sizeChange, firstPulseDuration, pulseCurveFirst);

            yield return Pulse(0.3f * sizeChange, secondPulseDuration * 0.5f, pulseCurveSecond);

            yield return Pulse(0f, secondPulseDuration * 0.5f, pulseCurveBack);

            yield return new WaitForSeconds(pulseInterval - (firstPulseDuration + secondPulseDuration));
        }
    }

    private IEnumerator Pulse(float targetOffset, float duration, AnimationCurve curve)
    {
        float startSize = _virtualCam.m_Lens.OrthographicSize;
        float targetSize = _defaultSize - targetOffset;
        float timer = 0f;

        while (timer < duration && _isEffectActive)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            float curveValue = curve.Evaluate(progress);
           //Debug.Log(targetOffset + " " + curveValue + " " + Mathf.Lerp(startSize, targetSize, curveValue));
            _virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, curveValue);
            yield return null;
        }
    }
}