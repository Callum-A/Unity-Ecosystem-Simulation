using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : Animal
{
    public Prey CurrentTarget { get; protected set; }

    public Predator(Tile tile, AnimalManager animalManager) : base(tile, 2f, 5, AnimalType.Predator, animalManager) { }

    // TODO: Implement death here and start testing population levels etc.
    public override bool ShouldDie()
    {
        return false;
    }

    public override void Die()
    {
        Debug.Log("Now dying!");
        AnimalManager.DespawnPredator(this);
    }

    public override void Update(float deltaTime)
    {
        Hunger -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(1.5f));
        Thirst -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(2f));
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

    // TODO: All of this seek food stuff needs to add functionality for:
    //       - Another predator kills the prey (idk how to do this, might need some flag to be set on prey a bool BeingChased?)
    //       - They prey dies of natural causes (hunger thirst etc.)

    public void UpdateDoSeekFood(float deltaTime)
    {
        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            CurrentState = AnimalState.Hungry;
        }
    }

    public void UpdateDoEating(float deltaTime)
    {
        // TODO: add some kind of timer? chance to fail?
        CurrentTarget.Die();
        CurrentTarget = null;
        CurrentState = AnimalState.Idle;
        Hunger = 1f;
        Debug.Log("Done eating - Predator");
    }

    public void UpdateDoFoundFood(float deltaTime)
    {
        if (DestinationTile != CurrentTarget.CurrentTile)
        {
            DestinationTile = CurrentTarget.CurrentTile;
        }

        // TODO: Change this to within a range?
        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            CurrentState = AnimalState.Eating;
        }
    }

    public void UpdateDoHungry(float deltaTime)
    {
        // Stop movement
        StopMovement();
        // Check for food tiles in our sight radius
        // Check for prey in our sight radius
        Prey closestPreyICanSee = AnimalManager.FindClosestPreyInRadius(CurrentTile.X, CurrentTile.Y, SightRange);

        if (closestPreyICanSee != null)
        {
            CurrentState = AnimalState.FoundFood;
            CurrentTarget = closestPreyICanSee;
            DestinationTile = closestPreyICanSee.CurrentTile;
        }
        else
        {
            // We need to pick a direction to walk in to seek
            CurrentState = AnimalState.SeekFood;
            // TODO: meta game this so they walk near prey
            Tile dest = WorldController.Instance.World.GetRandomNonWaterTileInRadius(CurrentTile, 5);
            DestinationTile = dest;
        }
    }

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
        else
        {
            CurrentState = AnimalState.Wandering;
            DestinationTile = world.GetRandomNonWaterTileInRadius(CurrentTile, 5);
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
