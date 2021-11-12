using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event
{
    public abstract void DoEvent(World world, float severity);
}

public class TestEvent : Event
{
    public override void DoEvent(World world, float severity)
    {
        Debug.Log("TEST EVENT!");
    }

    override public string ToString()
    {
        return "Test Event";
    }
}

public class DesertificationEvent : Event
{
    public override void DoEvent(World world, float severity)
    {
        foreach (Tile t in world.tiles)
        {
            t.Type = TileType.Sand;
        }
    }

    override public string ToString()
    {
        return "Desertification Event";
    }
}