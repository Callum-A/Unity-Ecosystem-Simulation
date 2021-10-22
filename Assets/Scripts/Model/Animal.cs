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

    public int ID { get; protected set; }
    public float Hunger { get; protected set; }
    public float Thirst { get; protected set; }
    public AnimalType AnimalType { get; protected set; }

    public float Speed { get; protected set; }
    public int SightRange { get; protected set; }
    private PathAStar pathAStar;
    private float movePercentage;

    protected Action<Animal> OnAnimalChangedCallback;

    public Animal(Tile tile, float speed, int sightRange, AnimalType animalType, AnimalManager animalManager, int id)
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
        ID = id;
    }

    /// <summary>
    /// Set the destination tile.
    /// </summary>
    /// <param name="t">Tile to set the destination to.</param>
    public void SetDestination(Tile t)
    {
        DestinationTile = t;
    }

    /// <summary>
    /// Stops the movement.
    /// </summary>
    public void StopMovement()
    {
        DestinationTile = NextTile;
    }

    /// <summary>
    /// Checks if both needs are above 50%.
    /// </summary>
    /// <returns>True if both needs are above 50%, false otherwise.</returns>
    public bool NeedsMet()
    {
        return Hunger >= 0.5f && Thirst >= 0.5f;
    }

    /// <summary>
    /// Checks if the animal is hungry. Hungry is defined when hunger is less than 0.3.
    /// </summary>
    /// <returns>True if hunger is less than 0.3.</returns>
    public bool IsHungry()
    {
        return Hunger <= 0.3f;
    }

    /// <summary>
    /// Checks if the animal is thirsty. Thirsty is defined when thirst is less than 0.3.
    /// </summary>
    /// <returns>True if thirst is less than 0.3.</returns>
    public bool IsThirsty()
    {
        return Thirst <= 0.3f;
    }

    /// <summary>
    /// Abstract method to check if an animal should die.
    /// </summary>
    /// <returns>True if it should die, false otherwise.</returns>
    public abstract bool ShouldDie();

    /// <summary>
    /// Abstract method called when the animal dies.
    /// </summary>
    public abstract void Die();

    /// <summary>
    /// Abstract main update function.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public abstract void Update(float deltaTime);

    /// <summary>
    /// Movement function, handles pathfinding and move percentage.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
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

    /// <summary>
    /// I am thirsty but I do not see water, I am heading to try and find some.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
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

    /// <summary>
    /// I am drinking.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public void UpdateDoDrinking(float deltaTime)
    {
        timeSinceStartedDrinking += deltaTime;
        if (timeSinceStartedDrinking >= drinkingTimeInSeconds)
        {
            Debug.Log("Done drinking! " + this.ToString());
            Thirst = 1f;
            CurrentState = AnimalState.Idle;
        }
    }

    /// <summary>
    /// I see water and I am heading to it.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public void UpdateDoFoundWater(float deltaTime)
    {
        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            timeSinceStartedDrinking = 0f;
            CurrentState = AnimalState.Drinking;
        }
    }

    /// <summary>
    /// I am thirsty and I need to check if I can see water, or I need to look for it.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public void UpdateDoThirsty(float deltaTime)
    {
        // Stop movement
        StopMovement();
        // Check for food tiles in our sight radius
        List<Tile> tilesICanSee = CurrentTile.GetRadius(SightRange);
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
            CurrentState = AnimalState.SeekWater;
            //Tile dest = WorldController.Instance.World.GetRandomNonWaterTileInRadius(CurrentTile, SightRange);
            Tile dest = WorldController.Instance.World.FindClosestDrikableTile(CurrentTile);
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
