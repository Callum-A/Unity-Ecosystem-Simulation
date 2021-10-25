using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Animal
{
    public Prey(Tile tile, AnimalManager animalManager, int id) : base(tile, 1f, 5, AnimalType.Prey, animalManager, id) { }

    /// <summary>
    /// Prey should die function, returns true when it should die. Should die if hunger
    /// or thirst drops below 0 and it is not eating or drinking.
    /// </summary>
    /// <returns>Boolean if it should die.</returns>
    public override bool ShouldDie()
    {
        if (Hunger < 0f && CurrentState != AnimalState.Eating || Thirst < 0f && CurrentState != AnimalState.Drinking)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Method called when the prey dies.
    /// </summary>
    public override void Die()
    {
        if (DestinationTile.HasFood() && DestinationTile.isFoodOccupied())
        {
            DestinationTile.setFoodUnoccupied();
        }

        Debug.Log("Now dying! - "  + this.ToString());
        AnimalManager.DespawnAnimal(this);
    }

    /// <summary>
    /// Main update function and statemachine decrements hunger and thirst.
    /// </summary>
    /// <param name="deltaTime">Time between last frame</param>
    public override void Update(float deltaTime)
    {
        Hunger -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(1.5f));
        Thirst -= (deltaTime * TimeController.Instance.GetTimesADayMultiplier(2f));
        timeSinceLastBreeded += deltaTime;
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

    /// <summary>
    /// I am hungry however I do not see food, I am moving to a tile to seek food,
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

    private float timeSinceStartedEating;
    private float eatingTimeInSeconds = 0.5f;

    // TODO: In all food states check if food has ran out. Use tile.HasFood()

    /// <summary>
    /// I am on a food tile and I am eating it.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public void UpdateDoEating(float deltaTime)
    {
        timeSinceStartedEating += deltaTime;
        if (timeSinceStartedEating >= eatingTimeInSeconds)
        {
            bool hasEaten = CurrentTile.ConsumeFood();
            // TODO: Check this to see if we've actually eaten
            Hunger = 1f;
            Debug.Log("Eating done!" + this.ToString());

            if (DestinationTile.HasFood())
            {
                DestinationTile.setFoodUnoccupied();
            }

            CurrentState = AnimalState.Idle;
        }
    }

    /// <summary>
    /// I see food and I am walking to it.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public void UpdateDoFoundFood(float deltaTime)
    {
        if (CurrentTile == DestinationTile)
        {
            StopMovement();
            timeSinceStartedEating = 0f;
            CurrentState = AnimalState.Eating;
        }
    }

    /// <summary>
    /// I am now hungry I need to see if I can see a food tile,
    /// or I need to go looking for food.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public void UpdateDoHungry(float deltaTime)
    {
        // Stop movement
        StopMovement();
        // Check for food tiles in our sight radius
        List<Tile> tilesICanSee = CurrentTile.GetRadius(SightRange);
        Tile foodTile = null;
        
        foreach (Tile t in tilesICanSee)
        {
            if (t != null)
            {
                if (t.HasFood() && !t.isFoodOccupied())
                {
                    foodTile = t;
                    t.setFoodOccupied();
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
            foodTile = CurrentTile.GetClosestFoodTile();
            Tile dest = foodTile.GetRandomNonWaterTileInRadius(SightRange);
            DestinationTile = dest;
        }
    }

    // TODO: Randomise this on start
    private float timeSinceLastBreeded = 0;
    private float breedingCooldown = 2 * TimeController.Instance.SECONDS_IN_A_DAY; // can breed every 2 days

    /// <summary>
    /// Idle state, either moves to hungry, thirsty or wanders.
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
            DestinationTile = CurrentTile.GetRandomNonWaterTileInRadius(SightRange);
        }
    }

    /// <summary>
    /// I am wandering to a random tile.
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
    public string ToString() 
    {
        return "Prey_" + ID;
    }
}
