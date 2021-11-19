using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager
{
    public static readonly float EVENT_CHANCE_PER_DAY = 0.02f; // 2% chance
    private List<Event> events;
    private Event currentActiveEvent;
    private int currentDurationLeft;
    private float currentSeverity;

    public EventManager()
    {
        events = new List<Event>();
        currentActiveEvent = null;
        currentDurationLeft = 0;
        currentSeverity = 0f;
        AddEvents();
    }

    private void AddEvents()
    {
        RegisterEvent(new TestEvent());
        //RegisterEvent(new DesertificationEvent());
        //RegisterEvent(new FloodEvent());
        //RegisterEvent(new DroughtEvent());
    }

    public Event ChooseEvent()
    {
        float chance = UnityEngine.Random.Range(0f, 1f);
        if (chance < EVENT_CHANCE_PER_DAY && events.Count > 0)
        {
            return events[UnityEngine.Random.Range(0, events.Count)];
        }

        return null;
    }

    public void RegisterEvent(Event e)
    {
        events.Add(e);
    }

    public void OnNewDay(World world)
    {
        if (currentActiveEvent == null)
        {
            Event e = ChooseEvent();

            if (e != null)
            {
                // Do event
                currentActiveEvent = e;
                currentDurationLeft = UnityEngine.Random.Range(1, 6); ;
                currentSeverity = UnityEngine.Random.Range(0f, 1f);
                currentActiveEvent.OnEventStart(world, currentSeverity, currentDurationLeft);
                WorldController.Instance.EventLogController.AddLog($"Choosing event: {e.ToString()} with severity: {currentSeverity}", Color.red);
            }
            else
            {
                WorldController.Instance.EventLogController.AddLog("No event on this day!");
            }
        }
        else
        {
            currentDurationLeft--;
            if (currentDurationLeft == 0)
            {
                WorldController.Instance.EventLogController.AddLog($"Event {currentActiveEvent.ToString()} is now over!", Color.red);
                currentActiveEvent.OnEventEnd(world, currentSeverity, currentDurationLeft);
                // Reset severity variable to be regenerated when we choose
                currentSeverity = 0f;
                currentActiveEvent = null;
                currentDurationLeft = 0;
            }
            else
            {
                WorldController.Instance.EventLogController.AddLog($"Event {currentActiveEvent.ToString()} is still on going!", Color.red);
                currentActiveEvent.OnEventDay(world, currentSeverity, currentDurationLeft);
            }
        }
    }
}
