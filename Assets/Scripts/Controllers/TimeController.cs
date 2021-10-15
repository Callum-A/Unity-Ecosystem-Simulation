using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; protected set; }
    public float SECONDS_IN_A_DAY;
    private float currentSeconds;
    public int NumberOfDays { get; protected set; }

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
    }

    public float GetTimesADayMultiplier(float days)
    {
        if (days == 0f)
        {
            return 0f;
        }

        return (days / SECONDS_IN_A_DAY);
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
        currentSeconds += Time.deltaTime;
        if (currentSeconds >= SECONDS_IN_A_DAY)
        {

            currentSeconds = 0;
            NumberOfDays++;
            if (OnNewDayCallback != null)
            {
                OnNewDayCallback(WorldController.Instance.World);
            }
            Debug.Log("new Day " + NumberOfDays);
        }
    }
}
