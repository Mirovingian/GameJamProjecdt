using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerPills: MonoBehaviour
{
    private int AMOUNT_OF_BULLETS_TO_ADD = 10;

    private int pillsForOverdose = 10;
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
/*        if(overdoseTriggered)
        {
            test_Text_2.text = (overdoseEndTime - Time.time).ToString();
        }
        if (overdoseTriggered && Time.time >= overdoseEndTime)
        {
            EndOverdose();
            test_Text_2.text = "0";
        }*/
        CleanupOldPills();
    }

    private void Overdosing()
    {
        pillsInOverdoseNow++;
        overdoseEndTime = Time.time + overdoseEscalate;
        Debug.Log("You're eat " + pillsInOverdoseNow + "overdosed");
        if (pillsInOverdoseNow >= pillsInOverdose)
        {
            Debug.Log("Death");
            EndOverdose();
            //Death();
        }
    }

    private void StartOverdosing()
    {
        pillsTakenTimes.Clear();
        Debug.Log("Start overdosing");
        overdoseTriggered = true;
        pillsInOverdoseNow = 0;
        overdoseEndTime = Time.time + overdoseEscalate;
        //Do smt
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
        Debug.Log("End overdosing");
        overdoseTriggered = false;
    }


    public void UseAmmoPill()
    {
        Debug.Log("Take ammo pill");
        if (_currentPillsCount > 0)
        {
            --_currentPillsCount;
            GameEntryPoint._instance._uiRoot.ChangePillsAmountView(_currentPillsCount);
            GameEntryPoint._instance._playerController._gunController.AddEnergy(AMOUNT_OF_BULLETS_TO_ADD);
            pillsTakenTimes.Add(Time.time);
            CheckOverdose();
        }
            
    }
    public void UseLightPill()
    {
        Debug.Log("Take light pill");
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
