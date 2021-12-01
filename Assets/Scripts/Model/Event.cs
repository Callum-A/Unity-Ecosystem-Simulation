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

public class FamineEvent : Event
{
    private int foodToRemovePerDay;
    public override void OnEventStart(World world, float severity, int durationInDays)
    {
        int totalFoodToRemove = Mathf.FloorToInt(0.5f * world.FoodManager.FoodTiles.Count * severity); // Max half of food can be killed by famine
        // Can be 0 due to low food amount e.g. called at start of sim or after loads eaten or low sev
        if (totalFoodToRemove == 0)
        {
            // One per day
            foodToRemovePerDay = 1;
        } else
        {
            foodToRemovePerDay = totalFoodToRemove / durationInDays;
        }
        
        Debug.Log("Killing food per day: " + foodToRemovePerDay);
    }

    public override void OnEventDay(World world, float severity, int durationLeftInDays)
    {
        int i = 0;
        while (i < foodToRemovePerDay)
        {
            Tile foodTile = world.FoodManager.FoodTiles[UnityEngine.Random.Range(0, world.FoodManager.FoodTiles.Count)];
            foodTile.DrownTile();
            i++;
        }
    }

    public override void OnEventEnd(World world, float severity, int durationInDays) {}

    override public string ToString()
    {
        return "Famine Event";
    }
}

public class SproutEvent : Event
{
    private int foodToAddPerDay;
    public override void OnEventStart(World world, float severity, int durationInDays)
    {
        int totalFoodToAdd = Mathf.FloorToInt(0.5f * world.FoodManager.FoodTiles.Count * severity); // Max 50% more food can be aded
        // Can be 0 due to low food amount e.g. called at start of sim or after loads eaten or low sev
        if (totalFoodToAdd == 0)
        {
            // One per day
            foodToAddPerDay = 1;
        }
        else
        {
            foodToAddPerDay = totalFoodToAdd / durationInDays;
        }

        Debug.Log("Adding food per day: " + foodToAddPerDay);
    }

    public override void OnEventDay(World world, float severity, int durationLeftInDays)
    {
        int i = 0;
        while (i < foodToAddPerDay)
        {
            Tile tile = world.Data.GrassTiles[UnityEngine.Random.Range(0, world.Data.GrassTiles.Count)];
            world.FoodManager.AddFoodToTile(tile);
            i++;
        }
    }

    public override void OnEventEnd(World world, float severity, int durationInDays) { }

    override public string ToString()
    {
        return "Sprout Event";
    }
}

public class MigrationEvent : Event
{
    private readonly int maxPreds = 4;
    private readonly int maxPrey = 12;

    public override void OnEventStart(World world, float severity, int durationInDays)
    {
        int[] spawnCoords = world.TerrainGenerator.GetHighestPoint(world.Data.TerrainData); // TODO: determine safe spawn tile on edge of map
        Tile spawnTile = world.GetTileAt(spawnCoords[0], spawnCoords[1]);
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            // Predator migration
            int amount = UnityEngine.Random.Range(1, Mathf.CeilToInt(severity * (maxPreds + 1))); // max 4 preds
            while (amount > 0)
            {
                Predator newPrey = world.AnimalManager.SpawnPredator(spawnTile, null);
                int ageRand = UnityEngine.Random.Range(0, 4);
                if (ageRand == 0)
                {
                    newPrey.setElder();
                }
                else
                {
                    newPrey.setAdult();
                }
                amount--;
            }
        }
        else
        {
            // Prey migration
            int amount = UnityEngine.Random.Range(1, Mathf.CeilToInt(severity * (maxPrey + 1))); // max 12 rabbits
            while (amount > 0)
            {
                Prey newPrey = world.AnimalManager.SpawnPrey(spawnTile, null);
                int ageRand = UnityEngine.Random.Range(0, 4);
                if (ageRand == 0)
                {
                    newPrey.setElder();
                }
                else
                {
                    newPrey.setAdult();
                }
                amount--;
            }
        }
    }

    public override void OnEventDay(World world, float severity, int durationLeftInDays) { }

    public override void OnEventEnd(World world, float severity, int durationInDays) { }

    override public string ToString()
    {
        return "Migration Event";
    }
}

public class DiseaseEvent : Event
{
    private int animalsToKillADay;
    public override void OnEventStart(World world, float severity, int durationInDays)
    {
        int totalAnimalsToKill = Mathf.FloorToInt(0.5f * world.AnimalManager.AllAnimals.Count * severity);

        if (totalAnimalsToKill == 0)
        {
            animalsToKillADay = 1;
        }
        else
        {
            animalsToKillADay = totalAnimalsToKill / durationInDays;
        }

        Debug.Log("Killing animals per day " + animalsToKillADay);
    }

    public override void OnEventDay(World world, float severity, int durationLeftInDays)
    {
        int i = 0;
        while (i < animalsToKillADay)
        {
            Animal toKill = world.AnimalManager.AllAnimals[UnityEngine.Random.Range(0, world.AnimalManager.AllAnimals.Count)];
            toKill.Die();
            i++;
        }
    }

    public override void OnEventEnd(World world, float severity, int durationInDays) {}

    override public string ToString()
    {
        return "Disease Event";
    }
}