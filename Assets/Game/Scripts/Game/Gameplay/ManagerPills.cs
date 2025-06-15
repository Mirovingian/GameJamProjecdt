using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerPills: MonoBehaviour
{
    public const int StartCountPills = 10;
    private int bulletsToAdd = 10;

    private int pillsForOverdose = 3;
    private float overdoseTimeWindow = 10f;
    private float overdoseEscalate = 5f;

    private int pillsInOverdose = 5;
    private int pillsInOverdoseNow = 0;
    private float overdoseEndTime = 0;

    private List<float> pillsTakenTimes = new List<float>();
    private bool overdoseTriggered;

    public int _currentPillsCount = StartCountPills;

    private const float _lightMagnificationValue = 6f;

    [SerializeField] private AudioSource _audioSource;


    private void Start()
    {
        GameEntryPoint._instance._uiRoot.ChangePillsAmountView(_currentPillsCount);
    }

    public void Update()
    {
        if (overdoseTriggered && Time.time >= overdoseEndTime)
        {
            EndOverdose();
        }
        CleanupOldPills();
    }

    private void Overdosing()
    {
        pillsInOverdoseNow++;
        //Trigger camera 
        GameEntryPoint._instance.TickOverdose();
        overdoseEndTime = Time.time + overdoseEscalate;
        if (pillsInOverdoseNow >= pillsInOverdose)
        {
            EndOverdose();
            GameEntryPoint._instance._playerController.Death();
        }
    }

    private void StartOverdosing()
    {
        pillsTakenTimes.Clear();
        overdoseTriggered = true;
        pillsInOverdoseNow = 0;
        overdoseEndTime = Time.time + overdoseEscalate;
        GameEntryPoint._instance.StartOverdose();
        _audioSource.Play();
    }

    private void CheckOverdose()
    {
        if (overdoseTriggered)
        {
            Overdosing();
            return;
        }

        var recentPills = GetPillsInTimeWindow(overdoseTimeWindow);

        if (recentPills >= pillsForOverdose)
        {
            StartOverdosing();
        }
    }

    private int GetPillsInTimeWindow(float window)
    {
        float cutoffTime = Time.time - window;
        int count = 0;

        foreach (float time in pillsTakenTimes)
        {
            if (time > cutoffTime) count++;
        }

        return count;
    }

    private void CleanupOldPills()
    {
        if (!overdoseTriggered)
        {
            float oldestAllowedTime = Time.time - Mathf.Max(overdoseTimeWindow, overdoseEscalate * 2);
            pillsTakenTimes.RemoveAll(time => time < oldestAllowedTime);
        }
    }

    private void EndOverdose()
    {
        overdoseTriggered = false;
        GameEntryPoint._instance.EndOverdose();
        _audioSource.Stop();
    }


    public bool UseAmmoPill()
    {
        if (_currentPillsCount > 0)
        {
            --_currentPillsCount;
            GameEntryPoint._instance._uiRoot.ChangePillsAmountView(_currentPillsCount);
            GameEntryPoint._instance._playerController._gunController.AddEnergy(bulletsToAdd);
            pillsTakenTimes.Add(Time.time);
            CheckOverdose();
            return true;
        }
        return false;    
    }
    public bool UseLightPill()
    {
        if (_currentPillsCount > 0)
        {
            --_currentPillsCount;
            GameEntryPoint._instance._uiRoot.ChangePillsAmountView(_currentPillsCount);
            GameEntryPoint._instance._playerController.IncreaseLightRange(_lightMagnificationValue);
            pillsTakenTimes.Add(Time.time);
            CheckOverdose();
            return true;
        }
       return false;
    }

    public float _timePick = -0.2f;
    public void IncreasePillsAmount()
    {
        var currentTime = Time.realtimeSinceStartup;
        if (currentTime > _timePick + 0.2f)
        {
            ++_currentPillsCount;
            GameEntryPoint._instance._uiRoot.ChangePillsAmountView(_currentPillsCount);
        }
        _timePick = currentTime;
    }

}
