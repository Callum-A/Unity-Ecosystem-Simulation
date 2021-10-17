using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Animal
{
    public Prey(Tile tile, AnimalManager animalManager) : base(tile, 1f, 5, AnimalType.Prey, animalManager) { }

    public override bool ShouldDie()
    {
        if (Hunger < 0f && CurrentState != AnimalState.Eating || Thirst < 0f && CurrentState != AnimalState.Drinking)
        {
            return true;
        }
        return false;
    }

    public override void Die()
    {
        Debug.Log("Now dying!");
        AnimalManager.DespawnPrey(this);
    }

    public override void Update(float deltaTime)
    {
        Hunger -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(1.5f));
        Thirst -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(2f));
        timeSinceLastBreeded += deltaTime;
        CheckDeath();
        UpdateDoMovement(deltaTime);
        switch (CurrentState)
        {
            case AnimalState.Idle:
                UpdateDoIdle(deltaTime);
                break;
            case AnimalState.Wandering:
                UpdateDoWandering(deltaTime);
                break;
            case AnimalState.Hungry:
                UpdateDoHungry(deltaTime);
                break;
            case AnimalState.Eating:
                UpdateDoEating(deltaTime);
                break;
            case AnimalState.FoundFood:
                UpdateDoFoundFood(deltaTime);
                break;
            case AnimalState.SeekFood:
                UpdateDoSeekFood(deltaTime);
                break;
            case AnimalState.Thirsty:
                UpdateDoThirsty(deltaTime);
                break;
            case AnimalState.Drinking:
                UpdateDoDrinking(deltaTime);
                break;
            case AnimalState.FoundWater:
                UpdateDoFoundWater(deltaTime);
                break;
            case AnimalState.SeekWater:
                UpdateDoSeekWater(deltaTime);
                break;
            default:
                Debug.LogError("Unrecognised state " + CurrentState);
                break;
        }

        if (OnAnimalChangedCallback != null)
        {
            OnAnimalChangedCallback(this);
        }
    }

    public void UpdateDoSeekFood(float deltaTime)
    {
        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            CurrentState = AnimalState.Hungry;
        }
    }

    private float timeSinceStartedEating;
    private float eatingTimeInSeconds = 0.5f;

    // TODO: In all food states check if food has ran out.

    public void UpdateDoEating(float deltaTime)
    {
        timeSinceStartedEating += deltaTime;
        if (timeSinceStartedEating >= eatingTimeInSeconds)
        {
            CurrentTile.food.Consume();
            Hunger = 1f;
            Debug.Log("Eating done!");
            CurrentState = AnimalState.Idle;
        }
    }

    public void UpdateDoFoundFood(float deltaTime)
    {
        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            timeSinceStartedEating = 0f;
            CurrentState = AnimalState.Eating;
        }
    }

    public void UpdateDoHungry(float deltaTime)
    {
        // Stop movement
        StopMovement();
        // Check for food tiles in our sight radius
        List<Tile> tilesICanSee = CurrentTile.GetTilesInRadius(SightRange);
        Tile foodTile = null;
        foreach (Tile t in tilesICanSee)
        {
            if (t != null)
            {
                if (t.food != null)
                {
                    foodTile = t;
                    break;
                }
            }
        }

        if (foodTile != null)
        {
            CurrentState = AnimalState.FoundFood;
            DestinationTile = foodTile;
        }
        else
        {
            // We need to pick a direction to walk in to seek
            // TODO: Seek in a cardinal direction
            CurrentState = AnimalState.SeekFood;
            foodTile = WorldController.Instance.World.FindClosestFoodTile(CurrentTile);
            Tile dest = WorldController.Instance.World.GetRandomNonWaterTileInRadius(foodTile, SightRange);
            DestinationTile = dest;
        }
    }

    private float timeSinceLastBreeded = 0f;
    private float breedingCooldown = 2 * TimeController.Instance.SECONDS_IN_A_DAY; // can breed every 2 days

    public void UpdateDoIdle(float deltaTime)
    {
        World world = WorldController.Instance.World;
        StopMovement();
        if (IsThirsty())
        {
            CurrentState = AnimalState.Thirsty;
        }
        else if (IsHungry())
        {
            CurrentState = AnimalState.Hungry;
        }
        else if (NeedsMet() && timeSinceLastBreeded >= breedingCooldown && world.AnimalManager.Prey.Count >= 2)
        {
            // TODO: Randomise the cooldown on start so they dont all breed after a bit
            // TODO: Improve breeding, add gender? Add a meet up point instead of just checking for anyone in a radius
            // TODO: Move to breed state
            List<Prey> animals = world.AnimalManager.Prey;
            Prey partner = null;
            foreach (Prey a in animals)
            {
                if (a == this)
                {
                    continue; // can't breed with self
                }

                if (World.ManhattanDistance(CurrentTile.X, CurrentTile.Y, a.CurrentTile.X, a.CurrentTile.Y) < 3f) // TODO: Change to breed radius variable
                {
                    partner = a;
                    break;
                }
            }

            if (partner != null)
            {
                timeSinceLastBreeded = 0f;
                Debug.Log("Found partner so spawning new animal");
                AnimalManager.SpawnPrey(CurrentTile);
            }
        }
        else
        {
            CurrentState = AnimalState.Wandering;
            DestinationTile = WorldController.Instance.World.GetRandomNonWaterTileInRadius(CurrentTile, SightRange);
        }
    }

    public void UpdateDoWandering(float deltaTime)
    {
        if (IsThirsty())
        {
            StopMovement();
            CurrentState = AnimalState.Thirsty;
        }
        else if (IsHungry())
        {
            StopMovement();
            CurrentState = AnimalState.Hungry;
        }

        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            CurrentState = AnimalState.Idle;
        }
    }
}
