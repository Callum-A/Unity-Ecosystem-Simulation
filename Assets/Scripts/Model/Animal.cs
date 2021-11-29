using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Model;

public enum AnimalType
{
    Prey,
    Predator
}

public enum LifeStage
{ 
    Child,
    Adult,
    Elder
}

public enum Gender 
{ 
    Male,
    Female
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
    Drinking,
    ReadyToBreed,
    SearchingForMate,
    MovingToMate,
    Breeding
}

public abstract class Animal
{
    private Animal partner;
    public float X => Mathf.Lerp(CurrentTile.X, NextTile.X, movePercentage);
    public float Y => Mathf.Lerp(CurrentTile.Y, NextTile.Y, movePercentage);
    public Tile CurrentTile { get; protected set; }
    public Tile DestinationTile { get; protected set; }
    public Tile NextTile { get; protected set; }
    public Tile LastTile { get; protected set; }
    public AnimalManager AnimalManager { get; protected set; }
    public AnimalState CurrentState { get; protected set; }
    public bool readyToBreed { get; protected set; }
    public Pregnancy pregnacy { get; protected set; }
    public int ID { get; protected set; }
    public float TimeAlive { get; protected set; }
    public int Age
    {
        get { return (int)Math.Floor(TimeAlive/TimeController.Instance.SECONDS_IN_A_DAY); }
    }
    public LifeStage lifeStage { get; protected set; }
    public float Hunger;
    public float Thirst;
    protected float timeSinceLastBreeded;
    protected float breedingCooldown = 3 * TimeController.Instance.SECONDS_IN_A_DAY; // can breed every 3 days
    public AnimalType AnimalType { get; protected set; }
    public Gender AnimalSex { get; protected set;}
    
    public float Speed { get; protected set; }
    public int SightRange { get; protected set; }
    private float movePercentage;
    private int swimDistance; // used to keep track far animal has swam

    private Queue<Tile> path;

    protected Action<Animal> OnAnimalChangedCallback;

    public Animal(Tile tile, float speed, int sightRange, AnimalType animalType, AnimalManager animalManager, int id, Gender gender)
    {
        CurrentTile = tile;
        NextTile = tile;
        DestinationTile = tile;
        LastTile = tile;
        Hunger = 1f;
        Thirst = 1f;
        Speed = speed;
        movePercentage = 0f;
        swimDistance = 0;
        SightRange = sightRange;
        AnimalType = animalType;
        AnimalManager = animalManager;
        CurrentState = AnimalState.Idle;
        ID = id;
        lifeStage = LifeStage.Child;
        AnimalSex = gender;
        timeSinceLastBreeded = 0;
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

    public void Drown() 
    {
        //Debug.Log("DROWNING " + this.ToString());
        this.Die();
    }

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
        if (CurrentTile.Type == TileType.Water && CurrentTile.GetWalkableNeighboursIncludingDiagonal().Count == 0)
        {
            if (LastTile == CurrentTile) // if this happens, they are stuck in water.
            {
               Debug.Log("Tried to swim " + swimDistance + " tiles and drowned.");
               Drown();
            }

            if (swimDistance > 4)
            {
                int randomNum = UnityEngine.Random.Range(0, 10);
                int drownChance = swimDistance - 4;

                if (randomNum < (drownChance))
                {
                    Drown();
                    Debug.Log("Tried to swim " + swimDistance + " tiles and drowned.");
                    return;
                }
            }
        }
        else
        {
            swimDistance = 0;
        }

        if (CurrentState != AnimalState.Wandering)
        {
            if (CurrentTile == DestinationTile)
            {
                path = null;
                return;
            }

            if (NextTile == null || NextTile == CurrentTile)
            {
                
                    //Get next tile from path finder
                    if (path == null || path.Count == 0)
                    {
                        // Generate path
                        path = AnimalManager.PathManager.SolvePath(CurrentTile.World, CurrentTile, DestinationTile);

                        if (path.Count == 0)
                        {
                            Debug.LogError("Could not find path to destination tile " + DestinationTile.X + ", " + DestinationTile.Y);
                            path = null;
                            return;
                        }

                        NextTile = path.Dequeue(); // skip first as it is our current tile
                    }

                    // Grab next tile from path
                    NextTile = path.Dequeue();
            }
        }
        else
        {

            if (NextTile == null || NextTile == CurrentTile)
            {
                NextTile = AnimalManager.PathManager.GetWanderTile(CurrentTile, LastTile);
            }
        }

        if (NextTile != null) { NextTile.HeatCounter += 1.0f; }  // update current tile for heatmap

        float distToTravel = Mathf.Sqrt(Mathf.Pow(CurrentTile.X - NextTile.X, 2) + Mathf.Pow(CurrentTile.Y - NextTile.Y, 2));
        float distThisFrame = deltaTime * Speed;
        float percThisFrame = distThisFrame / distToTravel;
        movePercentage += percThisFrame;
        if (movePercentage >= 1)
        {
            // We have reached our dest
            // TODO: Get next tile from path finding
            //       If no more then we have truly reached our dest

            if (LastTile.Type == TileType.Water)
            {
                swimDistance++;
            }

            LastTile = CurrentTile;
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

        // If we see water tiles before we reach our destination
        List<Tile> waterTilesInSightRange = CurrentTile.GetWaterTilesInRadius(SightRange);
        if (waterTilesInSightRange.Count > 0)
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
            //Debug.Log("Done drinking! " + this.ToString());
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
        Tile waterTile = CurrentTile.GetClosestTile(tilesICanSee, TileType.Water);
 

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
            //Tile dest = WorldController.Instance.World.FindClosestDrikableTile(CurrentTile);
            Tile dest = CurrentTile.GetClosestWaterTile();
            DestinationTile = dest;
        }
    }

    public void UpdateAge(float deltaTime) 
    {
        this.TimeAlive += deltaTime;
    }

    public void UpdateDoIsReadyToBreed(float deltaTime)
    {
        AnimalManager.breedingManager.addToBreedList(this);

        if (IsHungry() || IsThirsty())
        {
            CurrentState = AnimalState.Idle;
        }

        else if (AnimalSex == Gender.Male)
        {
            CurrentState = AnimalState.SearchingForMate;
        }

        else // if in water, move out
        {
            CurrentState = AnimalState.Wandering;
        }

    }

    public void UpdateDoSeachingForMate(float deltaTime)
    {
        //tries to find partner
        Animal FoundPartner = AnimalManager.breedingManager.FindPartner(this);

        //Cant find a partner
        if (FoundPartner == null)
        {
            DestinationTile = CurrentTile.GetRandomNonWaterTileInRadius(3);
            CurrentState = AnimalState.Wandering;
            return;
        }

        //Found a partner and does not already have one
        else if (getPartner() == null)
        {
           // Debug.Log(this.ToString() + "Partners " + FoundPartner.ToString());

            this.setPartner(FoundPartner);
            FoundPartner.setPartner(this);

            this.StopMovement();
            FoundPartner.StopMovement();

            DestinationTile = CurrentTile.GetMidPointTile(FoundPartner.CurrentTile);
            FoundPartner.DestinationTile = DestinationTile;

            CurrentState = AnimalState.MovingToMate;
            FoundPartner.CurrentState = AnimalState.MovingToMate;

            AnimalManager.breedingManager.removeFromBreedList(this);
            AnimalManager.breedingManager.removeFromBreedList(FoundPartner);
        }

    }

    public void UpdateDoMovingToMate(float deltatime)
    {
        Tile partnerTile = getPartner().CurrentTile;

        if (CurrentTile == DestinationTile)
        {
            StopMovement();
        }

        if (getPartner() == null || IsThirsty() || IsHungry() || (getPartner().CurrentState != AnimalState.MovingToMate && getPartner().CurrentState != AnimalState.Breeding))
        {
            CurrentState = AnimalState.Idle;
            return;
        }


        if (partnerTile == DestinationTile && CurrentTile == DestinationTile)
        {
            CurrentState = AnimalState.Breeding;
            
            timeSinceLastBreeded = 0;
            getPartner().timeSinceLastBreeded = 0;
        }

    }

    public void UpdateDoBreeding(float deltaTime)
    {
        AnimalManager.breedingManager.Breed(this, getPartner());

        //Setting them to
        //idle after breeding
        getPartner().CurrentState = AnimalState.Idle;
        CurrentState = AnimalState.Idle;

        //clearing partners
        getPartner().clearPartner();
        clearPartner();
    }

    public abstract void AgeUp();

    public abstract void setChild();

    public abstract void setAdult();

    public abstract void setElder();

    public void setPartner(Animal animal) 
    {
        partner = animal;
    }

    public void clearPartner() 
    {
        partner = null;
    }

    public Animal getPartner() 
    {
        return this.partner;
    }

    public abstract void GiveBirth();

    public  void Impregnate()
    {
        pregnacy = new Pregnancy(this);
        timeSinceLastBreeded = 0f;
    }

    public  void UpdatePregnancy(float deltatime)
    {
        if (isPregnant())
        {
            pregnacy.UpdatePregnancy(deltatime);
        }
    }

    public  bool isPregnant()
    {
        return pregnacy != null;
    }

    protected bool IsReadyToBreed()
    {
        return (NeedsMet() && timeSinceLastBreeded >= breedingCooldown && !isPregnant() && lifeStage == LifeStage.Adult && getPartner() == null);
    }


    public void MisCarry()
    {
        pregnacy = null;
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
