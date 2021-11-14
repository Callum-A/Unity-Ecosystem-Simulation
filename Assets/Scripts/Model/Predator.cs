using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : Animal
{
    public Prey CurrentTarget { get; protected set; }

    public Predator(Tile tile, AnimalManager animalManager, int id) : base(tile, 2f, 5, AnimalType.Predator, animalManager, id) { }

    // TODO: Implement death here and start testing population levels etc.
    /// <summary>
    /// Returns true if the predator should die.
    /// </summary>
    /// <returns>Boolean if the predator should die.</returns>
    public override bool ShouldDie()
    {
        return false;
    }

    /// <summary>
    /// Method called when should die is True.
    /// </summary>
    public override void Die()
    {
        WorldController.Instance.EventLogController.AddLog($"{ToString()} has died!");
        AnimalManager.DespawnAnimal(this);
    }

    /// <summary>
    /// Main update function decrements values and handles the statemachine.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public override void Update(float deltaTime)
    {
        Hunger -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(1.5f));
        Thirst -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(2f));
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

    /// <summary>
    /// I cannot see a prey to eat, I am heading to a tile to then look for one.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public void UpdateDoSeekFood(float deltaTime)
    {
        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            CurrentState = AnimalState.Hungry;
        }
    }

    /// <summary>
    /// I am eating a prey.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public void UpdateDoEating(float deltaTime)
    {
        // TODO: add some kind of timer? chance to fail?
        CurrentTarget.Die();
        CurrentTarget = null;
        CurrentState = AnimalState.Idle;
        Hunger = 1f;
        Debug.Log("Done eating - Predator");
    }

    /// <summary>
    /// I can see prey and I am heading towards them.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
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

    /// <summary>
    /// I am hungry I need to check if I can see prey or I need to look for one.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
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
            //Tile dest = WorldController.Instance.World.GetRandomNonWaterTileInRadius(CurrentTile, 5);
            Tile dest = CurrentTile.GetRandomNonWaterTileInRadius(5);
            DestinationTile = dest;
        }
    }

    /// <summary>
    /// Idle function, checks if hungry or thirsty. Also wanders to a random tile.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
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
            //DestinationTile = world.GetRandomNonWaterTileInRadius(CurrentTile, 5);
            DestinationTile = CurrentTile.GetRandomNonWaterTileInRadius(5);
        }
    }

    /// <summary>
    /// I am walking to a random tile.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
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

    override
    public void AgeUp()
    {
        int age = Age;

        //Aging up
        if (lifeStage != LifeStage.Elder)
        {
            if (age == 5 && lifeStage != LifeStage.Adult)
            {
                lifeStage = LifeStage.Adult;
                Debug.Log(this.ToString() + "is now an Adult");
            }

            else if (age == 20)
            {
                lifeStage = LifeStage.Elder;
                Debug.Log(this.ToString() + "is now an Elder");
            }
        }

        //Chance to die
        else
        {
            int randomNum = UnityEngine.Random.Range(0, 100);
            int changeOfDeath = age - 15;

            if (randomNum < (changeOfDeath))
            {
                Debug.Log("Died at " + age + " days old.");
                Die();
            }
        }
    }
}
