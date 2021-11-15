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

// simple early implementation of floods
public class FloodEvent : Event
{
    public override void DoEvent(World world, float severity)
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

    override public string ToString()
    {
        return "Flood";
    }
}

// simple early implementation of droughts
public class DroughtEvent : Event
{
    public override void DoEvent(World world, float severity)
    {
        world.Data.WaterHeight -= (float)(severity * 0.2); 
        world.UpdateTerrain();
    }

    override public string ToString()
    {
        return "Drought Event";
    }
}