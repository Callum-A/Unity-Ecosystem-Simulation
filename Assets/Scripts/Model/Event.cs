using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event
{
    public abstract void OnEventStart(World world, float severity, int durationInDays);

    public abstract void OnEventDay(World world, float severity, int durationLeftInDays);

    public abstract void OnEventEnd(World world, float severity, int durationInDays); // duration should be 0 here
}

public class TestEvent : Event
{
    public override void OnEventStart(World world, float severity, int duration)
    {
        Debug.Log("TEST EVENT STARTED!");
    }

    public override void OnEventDay(World world, float severity, int durationLeftInDays)
    {
        Debug.Log("Duration left of test event: " + durationLeftInDays);
    }

    public override void OnEventEnd(World world, float severity, int durationInDays)
    {
        Debug.Log("TEST EVENT ENDED!");
    }

    override public string ToString()
    {
        return "Test Event";
    }
}

public class DesertificationEvent : Event
{
    public override void OnEventStart(World world, float severity, int durationInDays)
    {
        foreach (Tile t in world.tiles)
        {
            t.Type = TileType.Sand;
        }
    }

    public override void OnEventDay(World world, float severity, int durationLeftInDays) {}

    public override void OnEventEnd(World world, float severity, int durationInDays) {}

    override public string ToString()
    {
        return "Desertification Event";
    }
}

// simple early implementation of floods
public class FloodEvent : Event
{
    public override void OnEventStart(World world, float severity, int durationInDays)
    {
        world.Data.WaterHeight += (float)(severity * 0.2);
        if (world.Data.SandHeight < world.Data.WaterHeight)
        {
            if (world.Data.SandHeight + (severity * 0.2) > world.Data.SandHeightInitial)
            {
                world.Data.SandHeight = world.Data.SandHeightInitial;
            }
            else
            {
                world.Data.SandHeight += (float)(severity * 0.2);
            }
        }
        world.UpdateTerrain();
    }

    public override void OnEventDay(World world, float severity, int durationLeftInDays) {}

    public override void OnEventEnd(World world, float severity, int durationInDays)
    {
        world.Data.WaterHeight = world.Data.WaterHeightInitial;
        world.Data.SandHeight = world.Data.SandHeightInitial;
        world.UpdateTerrain();
    }

    override public string ToString()
    {
        return "Flood";
    }
}

// simple early implementation of droughts
public class DroughtEvent : Event
{
    public override void OnEventStart(World world, float severity, int durationInDays)
    {
        world.Data.WaterHeight -= (float)(severity * 0.2);
        world.UpdateTerrain();
    }

    public override void OnEventDay(World world, float severity, int durationLeftInDays) {}

    public override void OnEventEnd(World world, float severity, int durationInDays)
    {
        world.Data.WaterHeight = world.Data.WaterHeightInitial;
        world.UpdateTerrain();
    }

    override public string ToString()
    {
        return "Drought Event";
    }
}