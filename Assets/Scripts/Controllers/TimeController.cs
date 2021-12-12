using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; protected set; }
    public float SECONDS_IN_A_DAY;
    public Text CurrentDay;
    private float currentSeconds;
    public int NumberOfDays { get; protected set; }
    public static int TimeMultiplier { get; protected set; } = 1;
    private AnimalSpriteController asc;

    private Action<World> OnNewDayCallback;

    private void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("Unreachable instance of time controller already exists");
        }
        Instance = this;
    }

    private void Start()
    {
        NumberOfDays = 0;
        OnNewDayCallback += CSVData.CollectData;
        CSVData.CollectData(WorldController.Instance.World); // Workaround to collect data on app start
        asc = FindObjectOfType<AnimalSpriteController>();
    }

    public void SetTimeMultiplier(int multiplier)
    {
        if (multiplier != TimeMultiplier)
        {
            if (multiplier < 0)
            {
                TimeMultiplier = 1;
                return;
            }

            if (multiplier == 32)
            {
                asc.TurnOffGameObjects();
            }
            else
            {
                asc.TurnOnGameObjects();
            }

            TimeMultiplier = multiplier;
            WorldController.Instance.EventLogController.AddLog($"Setting time multiplier to {multiplier}x");
        }
    }

    public float GetTimesADayMultiplier(float times)
    {
        if (times == 0f)
        {
            return 0f;
        }

        return (times / SECONDS_IN_A_DAY);
    }

    public void RegisterOnNewDayCallback(Action<World> cb)
    {
        OnNewDayCallback += cb;
    }

    public void UnregisterOnNewDayCallback(Action<World> cb)
    {
        OnNewDayCallback -= cb;
    }


    // Update is called once per frame
    void Update()
    {
        currentSeconds += (TimeMultiplier * Time.deltaTime);
        if (currentSeconds >= SECONDS_IN_A_DAY)
        {

            currentSeconds = 0;
            NumberOfDays++;
            if (OnNewDayCallback != null)
            {
                OnNewDayCallback(WorldController.Instance.World);
            }

            CurrentDay.text = "Current  Day:  " + NumberOfDays;
        }
    }
}
