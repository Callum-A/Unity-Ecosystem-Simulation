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

    public bool IsEventHappening()
    {
        return currentActiveEvent != null;
    }

    private void AddEvents()
    {
        //RegisterEvent(new TestEvent());
        //RegisterEvent(new FamineEvent());
        //RegisterEvent(new DesertificationEvent());
        RegisterEvent(new FloodEvent());
        RegisterEvent(new DroughtEvent());
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

    /// <summary>
    /// Overload taking an event override to force an event
    /// </summary>
    /// <param name="world">The world</param>
    /// <param name="overide">Force an event from debug menu, usually null when called in OnNewDay</param>
    public void OnNewDay(World world, Event overide)
    {
        if (currentActiveEvent == null)
        {
            Event e = overide != null ? overide : ChooseEvent();

            if (e != null)
            {
                // Do event
                currentActiveEvent = e;
                currentDurationLeft = UnityEngine.Random.Range(3, 6);
                currentSeverity = UnityEngine.Random.Range(0.1f, 1f);
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

    /// <summary>
    /// On new day which is called on a new day by time contrtoller to choose an event
    /// </summary>
    /// <param name="world">The world</param>
    public void OnNewDay(World world)
    {
        OnNewDay(world, null);
    }
}
