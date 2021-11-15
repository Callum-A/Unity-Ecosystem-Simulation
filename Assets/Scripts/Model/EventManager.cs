using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager
{
    public static readonly float EVENT_CHANCE_PER_DAY = 0.02f; // 2% chance
    private List<Event> events;

    public EventManager()
    {
        events = new List<Event>();
        AddEvents();
    }

    private void AddEvents()
    {
        RegisterEvent(new TestEvent());
        //RegisterEvent(new DesertificationEvent());
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
        Event e = ChooseEvent();
        
        if (e != null)
        {
            // Do event
            float sev = UnityEngine.Random.Range(0f, 1f);
            WorldController.Instance.EventLogController.AddLog($"Choosing event: {e.ToString()} with severity: {sev}", Color.red);
            e.DoEvent(world, sev);
        }
        else
        {
            WorldController.Instance.EventLogController.AddLog("No event on this day!");
        }
    }
}
