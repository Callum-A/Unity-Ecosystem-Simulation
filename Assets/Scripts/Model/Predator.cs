using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : Animal
{

    public Prey CurrentTarget { get; protected set; }
    public Predator Mother { get; protected set; }

    public Predator(Tile tile, AnimalManager animalManager, int id, Gender gender, Predator mother) : base(tile, 3f, 5, AnimalType.Predator, animalManager, id, gender)
    {
        Mother = mother;
    }

    /// <summary>
    /// Returns true if the predator should die.
    /// </summary>
    /// <returns>Boolean if the predator should die.</returns>
    public override bool ShouldDie()
    {
        if (Hunger < 0f && CurrentState != AnimalState.Eating || Thirst < 0f && CurrentState != AnimalState.Drinking)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Method called when should die is True.
    /// </summary>
    public override void Die()
    {
        if (CurrentTarget != null && CurrentTarget.IsBeingChased)
        {
            CurrentTarget.SetIsBeingChased(false);
        }
        AnimalManager.breedingManager.removeFromBreedList(this);
        WorldController.Instance.EventLogController.AddLog($"{ToString()} has died!");
        Debug.Log("Now dying! - " + this.ToString());
        AnimalManager.DespawnAnimal(this);
    }

    /// <summary>
    /// Main update function decrements values and handles the statemachine.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public override void Update(float deltaTime)
    {
        Hunger -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(0.5f));
        Thirst -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(1.5f));
        timeSinceLastBreeded += deltaTime;
        UpdateAge(deltaTime);
        UpdateDoMovement(deltaTime);
        UpdatePregnancy(deltaTime);
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
            case AnimalState.ReadyToBreed:
                UpdateDoIsReadyToBreed(deltaTime);
                break;
            case AnimalState.SearchingForMate:
                UpdateDoSeachingForMate(deltaTime);
                break;
            case AnimalState.MovingToMate:
                UpdateDoMovingToMate(deltaTime);
                break;
            case AnimalState.Breeding:
                UpdateDoBreeding(deltaTime);
                break;
            case AnimalState.FollowingParent:
                UpdateDoFollowingParent(deltaTime);
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

    /// <summary>
    /// State for children, they will simply follow there mother and if she dies they die
    /// </summary>
    /// <param name="deltaTime">Time between frame</param>
    public void UpdateDoFollowingParent(float deltaTime)
    {
        // Can we now fend for ourselves
        if (lifeStage == LifeStage.Adult)
        {
            CurrentState = AnimalState.Idle;
            return;
        }

        // Is our mother dead
        if (Mother.ShouldDie())
        {
            // Death wander
            DestinationTile = CurrentTile.GetRandomNonWaterTileInRadius(SightRange);
            return;
        }

        // Set out hunger and thirst
        if (Mother.CurrentState == AnimalState.Eating)
        {
            Hunger = 1f;
        }
        else if (Mother.CurrentState == AnimalState.Drinking)
        {
            Thirst = 1f;
        }

        // Move with mother
        if (Mother.DestinationTile != DestinationTile)
        {
            DestinationTile = Mother.DestinationTile.GetRandomNonWaterTileInRadius(1);
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
        CurrentTarget.SetIsBeingChased(false);
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
        Prey closestPreyICanSee = AnimalManager.FindClosestPrey(CurrentTile.X, CurrentTile.Y);

        if (closestPreyICanSee != null)
        {
            CurrentState = AnimalState.FoundFood;
            CurrentTarget = closestPreyICanSee;
            closestPreyICanSee.SetIsBeingChased(true);
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
        if (!IsReadyToBreed())
        {
            AnimalManager.breedingManager.removeFromBreedList(this);
        }
        if (IsThirsty())
        {
            CurrentState = AnimalState.Thirsty;
        }
        else if (IsHungry())
        {
            CurrentState = AnimalState.Hungry;
        }

        else if (IsReadyToBreed())
        {
            CurrentState = AnimalState.ReadyToBreed;
        }

        else
        {
            CurrentState = AnimalState.Wandering;
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

    override
   public void setChild()
    {
        this.TimeAlive = 0;
        this.lifeStage = LifeStage.Child;
    }

    override
    public void setAdult()
    {
        this.TimeAlive = 5 * TimeController.Instance.SECONDS_IN_A_DAY;
        this.lifeStage = LifeStage.Adult;
    }

    override
    public void setElder()
    {
        this.TimeAlive = 20 * TimeController.Instance.SECONDS_IN_A_DAY;
        this.lifeStage = LifeStage.Elder;
    }

    public override void GiveBirth()
    {
        int litterSize = UnityEngine.Random.Range(2, 5);

        for (int i = 0; i < litterSize; i++)
        {
            Predator child = AnimalManager.SpawnPredator(CurrentTile, this);
            child.CurrentState = AnimalState.FollowingParent;
        }

        Debug.Log(this + " - Gives Birth");

        pregnacy = null;
        timeSinceLastBreeded = 0f;
    }

    override
    public string ToString()
    {
        return "Predator_" + ID;
    }

}
