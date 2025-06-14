using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    private float pulseInterval = 1.0f;
    private float sizeChange = 2.0f;
    private float firstPulseDuration = 0.3f;
    private float secondPulseDuration = 0.5f;
    private float zoomReturnDuration = 1.0f;

    private AnimationCurve pulseCurveFirst = new AnimationCurve(
        new Keyframe(0f, 0f, 3f, 3f),
        new Keyframe(0.7f, 1f),
        new Keyframe(1f, 0.7f)
    );

    private AnimationCurve pulseCurveSecond = new AnimationCurve(
        new Keyframe(0f, 0.3f),
        new Keyframe(0.3f, 0.8f),
        new Keyframe(1f, 0f, 0.5f, 0.5f)
    );

    private AnimationCurve pulseCurveBack = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 0.3f),
        new Keyframe(1f, 1f)
    );

    private CinemachineVirtualCamera _virtualCam;
    private float _defaultSize;
    private float _currentBaseSize;
    private bool _isEffectActive;
    private Coroutine _pulseCoroutine;
    private Coroutine _zoomReturnCoroutine;

    private void Start()
    {
        _virtualCam = GetComponent<CinemachineVirtualCamera>();
        if (_virtualCam == null)
        {
            Debug.LogError("CinemachineVirtualCamera can't be found");
            return;
        }

        _defaultSize = _virtualCam.m_Lens.OrthographicSize;
        _currentBaseSize = _defaultSize;
    }

    public void SetHeartbeatEffect(bool isActive)
    {
        if (_isEffectActive == isActive) return;
        _isEffectActive = isActive;

        if (_zoomReturnCoroutine != null)
        {
            StopCoroutine(_zoomReturnCoroutine);
            _zoomReturnCoroutine = null;
        }

        if (_isEffectActive)
        {
            if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = StartCoroutine(PulseRoutine());
        }
        else
        {
            if (_pulseCoroutine != null)
            {
                StopCoroutine(_pulseCoroutine);
                _pulseCoroutine = null;
            }
            _zoomReturnCoroutine = StartCoroutine(ReturnZoom());
        }
    }

    public void DescreseZoom()
    {
        _currentBaseSize -= 2f;
        _virtualCam.m_Lens.OrthographicSize = _currentBaseSize;
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
        float targetSize = _currentBaseSize - targetOffset;
        float timer = 0f;

        while (timer < duration && _isEffectActive)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            _virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, curve.Evaluate(progress));
            yield return null;
        }
    }

    private IEnumerator ReturnZoom()
    {
        float startSize = _virtualCam.m_Lens.OrthographicSize;
        float timer = 0f;

        while (timer < zoomReturnDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / zoomReturnDuration;
            _virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, _defaultSize, progress);
            _currentBaseSize = _virtualCam.m_Lens.OrthographicSize;
            yield return null;
        }

        _virtualCam.m_Lens.OrthographicSize = _defaultSize;
        _currentBaseSize = _defaultSize;
    }
}