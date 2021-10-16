using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AnimalType
{
    Prey,
    Predator
}

public enum AnimalState
{
    Idle, // we have nothing to do we are standing still, entry state
    Wandering, // we have nothing we need to do, we are walking to a random point
    Hungry, // we are hungry, we need to decide if we can see food or we need to search for it
    Thirsty, // we are thirsty, we need to decide if we can see water or we need to search for it
    FoundFood, // we are hungry, we can see food, we need to path to it
    SeekFood, // we are hungry, we cannot see food in our sightline
    Eating,
    FoundWater,
    SeekWater,
    Breeding,
    Drinking
}

public abstract class Animal
{
    public float X => Mathf.Lerp(CurrentTile.X, NextTile.X, movePercentage);
    public float Y => Mathf.Lerp(CurrentTile.Y, NextTile.Y, movePercentage);
    public Tile CurrentTile { get; protected set; }
    public Tile DestinationTile { get; protected set; }
    public Tile NextTile { get; protected set; }

    public AnimalManager AnimalManager { get; protected set; }
    public AnimalState CurrentState { get; protected set; }

    public float Hunger { get; protected set; }
    public float Thirst { get; protected set; }
    public AnimalType AnimalType { get; protected set; }

    public float Speed { get; protected set; }
    public int SightRange { get; protected set; }
    private PathAStar pathAStar;
    private float movePercentage;

    protected Action<Animal> OnAnimalChangedCallback;

    public Animal(Tile tile, float speed, int sightRange, AnimalType animalType, AnimalManager animalManager)
    {
        CurrentTile = tile;
        NextTile = tile;
        DestinationTile = tile;
        Hunger = 1f;
        Thirst = 1f;
        Speed = speed;
        movePercentage = 0f;
        SightRange = sightRange;
        AnimalType = animalType;
        AnimalManager = animalManager;
        CurrentState = AnimalState.Idle;
    }

    public void SetDestination(Tile t)
    {
        DestinationTile = t;
    }

    public void StopMovement()
    {
        DestinationTile = NextTile;
    }

    public bool NeedsMet()
    {
        return Hunger >= 0.5f && Thirst >= 0.5f;
    }

    public bool IsHungry()
    {
        return Hunger <= 0.3f;
    }

    public bool IsThirsty()
    {
        return Thirst <= 0.3f;
    }

    public void CheckDeath()
    {
        if (ShouldDie())
        {
            Die();
        }
    }

    public abstract bool ShouldDie();

    public abstract void Die();

    public abstract void Update(float deltaTime);

    public void UpdateDoMovement(float deltaTime)
    {
        if (CurrentTile == DestinationTile)
        {
            pathAStar = null;
            return;
        }

        if (NextTile == null || NextTile == CurrentTile)
        {
            // Get next tile from path finder
            if (pathAStar == null || pathAStar.Length() == 0)
            {
                // Generate path
                pathAStar = new PathAStar(CurrentTile.World, CurrentTile, DestinationTile);
                if (pathAStar.Length() == 0)
                {
                    Debug.LogError("Could not find path to destination tile " + DestinationTile.X + ", " + DestinationTile.Y);
                    pathAStar = null;
                    return;
                }

                NextTile = pathAStar.Dequeue(); // skip first as it is our curr tile
            }

            // Grab next tile from path
            NextTile = pathAStar.Dequeue();
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(CurrentTile.X - NextTile.X, 2) + Mathf.Pow(CurrentTile.Y - NextTile.Y, 2));
        float distThisFrame = deltaTime * Speed;
        float percThisFrame = distThisFrame / distToTravel;
        movePercentage += percThisFrame;
        if (movePercentage >= 1)
        {
            // We have reached our dest
            // TODO: Get next tile from path finding
            //       If no more then we have truly reached our dest
            CurrentTile = NextTile;
            movePercentage = 0;
            // Retain overshot movement?
        }
    }

    public void UpdateDoSeekWater(float deltaTime)
    {
        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            CurrentState = AnimalState.Thirsty;
        }
    }

    // TODO: rework this timer I don't like how it looks
    private float timeSinceStartedDrinking;
    private float drinkingTimeInSeconds = 0.5f;

    public void UpdateDoDrinking(float deltaTime)
    {
        timeSinceStartedDrinking += deltaTime;
        if (timeSinceStartedDrinking >= drinkingTimeInSeconds)
        {
            Debug.Log("Done drinking!");
            Thirst = 1f;
            CurrentState = AnimalState.Idle;
        }
    }

    public void UpdateDoFoundWater(float deltaTime)
    {
        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            timeSinceStartedDrinking = 0f;
            CurrentState = AnimalState.Drinking;
        }
    }

    public void UpdateDoThirsty(float deltaTime)
    {
        // Stop movement
        StopMovement();
        // Check for food tiles in our sight radius
        List<Tile> tilesICanSee = CurrentTile.GetTilesInRadius(SightRange);
        Tile waterTile = null;
        foreach (Tile t in tilesICanSee)
        {
            if (t != null)
            {
                if (t.Type == TileType.Water)
                {
                    waterTile = t;
                    break;
                }
            }
        }

        if (waterTile != null)
        {
            CurrentState = AnimalState.FoundWater;
            DestinationTile = waterTile;
        }
        else
        {
            // We need to pick a direction to walk in to seek, metagame to find water?
            // TODO: Make it go near water
            CurrentState = AnimalState.SeekFood;
            Tile dest = WorldController.Instance.World.GetRandomNonWaterTileInRadius(CurrentTile, SightRange);
            DestinationTile = dest;
        }
    }

    public void RegisterOnAnimalChangedCallback(Action<Animal> cb)
    {
        OnAnimalChangedCallback += cb;
    }

    public void UnregisterOnAnimalChangedCallback(Action<Animal> cb)
    {
        OnAnimalChangedCallback -= cb;
    }
}
