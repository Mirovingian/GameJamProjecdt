using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerPills: MonoBehaviour
{
    private int bulletsToAdd = 10;

    private int pillsForOverdose = 3;
    private float overdoseTimeWindow = 10f;
    private float overdoseEscalate = 5f;

    private int pillsInOverdose = 5;
    private int pillsInOverdoseNow = 0;
    private float overdoseEndTime = 0;

    private List<float> pillsTakenTimes = new List<float>();
    private bool overdoseTriggered;

    private int _currentPillsCount = 10;

    private const float _lightMagnificationValue = 2f;


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
    }


    public void UseAmmoPill()
    {
        if (_currentPillsCount > 0)
        {
            --_currentPillsCount;
            GameEntryPoint._instance._uiRoot.ChangePillsAmountView(_currentPillsCount);
            GameEntryPoint._instance._playerController._gunController.AddEnergy(bulletsToAdd);
            pillsTakenTimes.Add(Time.time);
            CheckOverdose();
        }
            
    }
    public void UseLightPill()
    {
        if (_currentPillsCount > 0)
        {
            --_currentPillsCount;
            GameEntryPoint._instance._uiRoot.ChangePillsAmountView(_currentPillsCount);
            GameEntryPoint._instance._playerController.IncreaseLightRange(_lightMagnificationValue);
            pillsTakenTimes.Add(Time.time);
            CheckOverdose();
        }
       
    }

    public void IncreasePillsAmount()
    {
        ++_currentPillsCount;
        GameEntryPoint._instance._uiRoot.ChangePillsAmountView(_currentPillsCount);
    }

}
