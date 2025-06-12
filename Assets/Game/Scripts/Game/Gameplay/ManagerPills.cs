using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerPills: MonoBehaviour
{
    private int AMOUNT_OF_BULLETS_TO_ADD = 10;

    public TMP_Text test_Text_1; //TEST
    public TMP_Text test_Text_2; //TEST

    private int pillsForOverdose = 10;
    private float overdoseTimeWindow = 10f;
    private float overdoseEscalate = 5f;

    private int pillsInOverdose = 5;
    private int pillsInOverdoseNow = 0;
    private float overdoseEndTime = 0;

    private List<float> pillsTakenTimes = new List<float>();
    private bool overdoseTriggered;

    public void Update()
    {
        if(overdoseTriggered)
        {
            test_Text_2.text = (overdoseEndTime - Time.time).ToString();
        }
        if (overdoseTriggered && Time.time >= overdoseEndTime)
        {
            EndOverdose();
            test_Text_2.text = "0";
        }
        CleanupOldPills();

        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            UseAmmoPill();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            UseLightPill();
        }
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
        test_Text_1.text = pillsTakenTimes.Count.ToString();
    }

    private void EndOverdose()
    {
        Debug.Log("End overdosing");
        overdoseTriggered = false;
    }


    public void UseAmmoPill()
    {
        Debug.Log("Take ammo pill");
        //Player.GetComponent<PlayerGunController>().AddEnergy(AMOUNT_OF_BULLETS_TO_ADD);
        pillsTakenTimes.Add(Time.time);
        CheckOverdose();
    }
    public void UseLightPill()
    {
        Debug.Log("Take light pill");
        //GameEntryPoint > light > do smt 
        pillsTakenTimes.Add(Time.time);
        CheckOverdose();
    }

}
