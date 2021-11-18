using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Animal
{
    public bool IsBeingChased { get; protected set; }
    public Prey(Tile tile, AnimalManager animalManager, int id, Gender gender) : base(tile, 2f, 5, AnimalType.Prey, animalManager, id, gender)
    {
        IsBeingChased = false;
    }

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
        if (DestinationTile != null && DestinationTile.HasFood() && DestinationTile.isFoodOccupied())
        {
            DestinationTile.setFoodUnoccupied();
        }
        WorldController.Instance.EventLogController.AddLog($"{ToString()} has died!");
        Debug.Log("Now dying! - " + this.ToString() + " State: " + this.CurrentState);
        AnimalManager.DespawnAnimal(this);
    }

    public void SetIsBeingChased(bool b)
    {
        IsBeingChased = b;
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

        // If we see food tiles before reaching our destination
        List<Tile> foodTilesInSightRange = CurrentTile.GetFoodTilesInRadius(SightRange);
        if (foodTilesInSightRange.Count > 0)
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
            //Debug.Log("Eating done!" + this.ToString());

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

    

    /// <summary>
    /// Idle state, either moves to hungry, thirsty or wanders.
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
            DestinationTile = CurrentTile.GetRandomNonWaterTileInRadius(SightRange);

            if (DestinationTile == null) { Drown(); }
        }
    }

    public void UpdateDoIsReadyToBreed(float deltaTime) 
    {
        AnimalManager.breedingManager.addToBreedList(this);
        CurrentState = AnimalState.SearchingForMate;
    }

    public void UpdateDoSeachingForMate(float deltaTime)
    {
            Prey FoundPartner = AnimalManager.breedingManager.FindPartner(this);

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
                this.setPartner(FoundPartner);
                FoundPartner.setPartner(this);

                this.StopMovement();
                FoundPartner.StopMovement();

                int x1 = this.CurrentTile.X;
                int y1 = this.CurrentTile.Y;

                int x2 = FoundPartner.CurrentTile.X;
                int y2 = FoundPartner.CurrentTile.Y;

                int midX = (x1 + x2) / 2;
                int midY = (y1 + y2) / 2;

                DestinationTile = WorldController.Instance.World.GetTileAt(midX, midY);
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
        }
        
    }

    public void UpdateDoBreeding(float deltaTime)
    {
        Prey currentParter = getPartner() as Prey;
        AnimalManager.breedingManager.Breed(this, currentParter);

        if (AnimalSex == Gender.Male) 
        {
            timeSinceLastBreeded = 0;
        }

        //Setting them to
        //idle after breeding
        currentParter.CurrentState = AnimalState.Idle;
        CurrentState = AnimalState.Idle;

        //clearing partners
        currentParter.clearPartner();
        clearPartner();
    }

    private bool IsReadyToBreed()
    {
        //NeedsMet() && timeSinceLastBreeded >= breedingCooldown && world.AnimalManager.Prey.Count >= 2 && !isPregnant()
        return (NeedsMet() && timeSinceLastBreeded >= breedingCooldown && !isPregnant() && lifeStage == LifeStage.Adult);
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
    public void AgeUp()
    {
        int age = Age;

        //Aging up
        if (lifeStage != LifeStage.Elder)
        {
            if (age == 10 && lifeStage != LifeStage.Adult)
            {
                lifeStage = LifeStage.Adult;
                Debug.Log(this.ToString() + "is now an Adult");
            }

            else if (age == 40)
            {
                lifeStage = LifeStage.Elder;
                Debug.Log(this.ToString() + "is now an Elder");
            }
        }

        //Chance to die
        else
        {
            int randomNum = UnityEngine.Random.Range(0, 100);
            int changeOfDeath = age - 35;

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
        this.TimeAlive = 10 * TimeController.Instance.SECONDS_IN_A_DAY;
        this.lifeStage = LifeStage.Adult;
    }

    override
    public void setElder()
    {
        this.TimeAlive = 40 * TimeController.Instance.SECONDS_IN_A_DAY;
        this.lifeStage = LifeStage.Elder;
    }

    public override void Impregnate()
    {
        pregnacy = new Pregnancy(this);
        timeSinceLastBreeded = 0f;
    }

    public override void UpdatePregnancy(float deltatime)
    {
        if (isPregnant())
        {
            pregnacy.UpdatePregnancy(deltatime);
        }
    }

    public override bool isPregnant()
    {
        return pregnacy != null;
    }

    public override void GiveBirth()
    {
        int litterSize = UnityEngine.Random.Range(1, 9);

        for (int i = 0; i < litterSize; i++)
        {
            AnimalManager.SpawnPrey(CurrentTile);
        }

        Debug.Log(this + " - Gives Birth");

        pregnacy = null;
        timeSinceLastBreeded = 0f;
    }

    public override void MisCarry()
    {
        pregnacy = null;
    }

    override
    public string ToString()
    {
        return "Prey_" + ID;
    }
}

