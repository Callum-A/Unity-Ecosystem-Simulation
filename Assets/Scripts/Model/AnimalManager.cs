using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimalManager
{
    public List<Prey> Prey { get; protected set; }
    public List<Predator> Predators { get; protected set; }
    public List<Animal> AllAnimals { get; protected set; }
    private World world;
    private int currentPreyID;
    private int currentPredatorID;

    private Action<Animal> OnAnimalCreatedCallback;
    private Action<Animal> OnAnimalDestroyedCallback;

    public AnimalManager(World w)
    {
        world = w;
        Prey = new List<Prey>();
        Predators = new List<Predator>();
        AllAnimals = new List<Animal>();
        currentPreyID = 0;
        currentPredatorID = 0;
    }

    /// <summary>
    /// Called on the start of the world to spawn initial animals.
    /// </summary>
    /// <param name="preyAmount">Number of prey.</param>
    /// <param name="predatorAmount">Number of predators.</param>
    public void SpawnAnimals(int preyAmount, int predatorAmount)
    {
        // TODO: choose spawn locations
        for (int i = 0; i < preyAmount; i++)
        {
            SpawnPrey(world.GetTileAt(50, 50));
        }

        for (int i = 0; i < predatorAmount; i++)
        {
            SpawnPredator(world.GetTileAt(50, 50));
        }
    }

    /// <summary>
    /// Main update function, calls update on every animal. Handles despawning
    /// and respawning via stacks.
    /// </summary>
    /// <param name="deltaTime">Time between last frame.</param>
    public void Update(float deltaTime)
    {
        int i = 0;
        while (i < AllAnimals.Count)
        {
            Animal a = AllAnimals[i];

            if (a.ShouldDie())
            {
                // Calls AnimalManager.DespawnAnimal from die func
                a.Die();
            }
            else
            {
                i++;
            }

            a.Update(deltaTime);
        }
    }

    /// <summary>
    /// Helper method to find closest prey within a radius.
    /// </summary>
    /// <param name="x">X position.</param>
    /// <param name="y">Y position.</param>
    /// <param name="r">Radius.</param>
    /// <returns>The closest prey in radius or null if none found.</returns>
    public Prey FindClosestPreyInRadius(int x, int y, int r)
    {
        Prey closest = null;
        List<Prey> inRadius = new List<Prey>();
        int startX = x - r;
        int startY = y - r;
        int endX = x + r;
        int endY = y + r;
        foreach (Prey p in Prey)
        {
            if (p.CurrentTile.X >= startX && p.CurrentTile.X <= endX && p.CurrentTile.Y >= startY &&
                p.CurrentTile.Y <= endY && !p.IsBeingChased)
            {
                inRadius.Add(p);
            }
        }
        int closestDistance = Int32.MaxValue;
        foreach (Prey p in inRadius)
        {
            int currentDist = World.ManhattanDistance(x, y, p.CurrentTile.X, p.CurrentTile.Y);
            if (currentDist < closestDistance)
            {
                closestDistance = currentDist;
                closest = p;
            }
        }

        return closest;
    }

    /// <summary>
    /// Helper method to find the closest prey to a given position.
    /// </summary>
    /// <param name="x">X position.</param>
    /// <param name="y">Y position.</param>
    /// <returns>Closest prey or null if none found.</returns>
    public Prey FindClosestPrey(int x, int y)
    {
        Prey closest = null;
        int closestDistance = Int32.MaxValue;
        foreach (Prey a in Prey)
        {
            int currentDist = World.ManhattanDistance(x, y, a.CurrentTile.X, a.CurrentTile.Y);
            if (currentDist < closestDistance)
            {
                closestDistance = currentDist;
                closest = a;
            }
        }
        return closest;
    }

    /// <summary>
    /// Spawn an animal and add to the spawn stack.
    /// </summary>
    /// <param name="a">Animal to spawn.</param>
    /// <returns>The spawned animal.</returns>
    private Animal Spawn(Animal a)
    {
        AllAnimals.Add(a);
        if (OnAnimalCreatedCallback != null)
        {
            OnAnimalCreatedCallback(a);
        }
        return a;
    }

    /// <summary>
    /// Spawn a predator on a given tile. Adds to predator list.
    /// </summary>
    /// <param name="tile">Tile to spawn it on.</param>
    /// <returns>The predator spawned.</returns>
    public Predator SpawnPredator(Tile tile)
    {
        Gender gender = (UnityEngine.Random.Range(0, 2) == 0) ? Gender.Male : Gender.Female;

        Predator p = new Predator(tile, this, currentPredatorID, gender);
        Predators.Add(p);
        Spawn(p);
        currentPredatorID++;
        return p;
    }

    /// <summary>
    /// Spawn a prey on a given tile. Adds to prey list.
    /// </summary>
    /// <param name="tile">Tile to spawn it on.</param>
    /// <returns>The prey spawned.</returns>
    public Prey SpawnPrey(Tile tile)
    {
        Gender gender = (UnityEngine.Random.Range(0,2) == 0) ? Gender.Male : Gender.Female;

        Prey p = new Prey(tile, this, currentPreyID, gender);
        Prey.Add(p);
        Spawn(p);
        currentPreyID++;

        Debug.Log(p.ToString() + " - " + p.AnimalSex);

        return p;
    }

    /// <summary>
    /// Despawn an animal, calling the callback and adding to the stack.
    /// </summary>
    /// <param name="a">Animal to despawn.</param>
    private void Despawn(Animal a)
    {
        if (OnAnimalDestroyedCallback != null)
        {
            OnAnimalDestroyedCallback(a);
        }

        AllAnimals.Remove(a);
    }

    public void DespawnAnimal(Animal a) 
    {
        if (a.AnimalType == AnimalType.Prey)
        {
            Prey.Remove(a as Prey);
        }

        else 
        {
            Predators.Remove(a as Predator);
        }

        Despawn(a);
    }

    public void AgeUpAnimals() 
    {
        for (int i = 0; i < AllAnimals.Count; i++) 
        {
            Animal a = AllAnimals[i];
            a.AgeUp();
        }
    }

    public void RegisterOnAnimalCreatedCallback(Action<Animal> cb)
    {
        OnAnimalCreatedCallback += cb;
    }

    public void UnregisterOnAnimalCreatedCallback(Action<Animal> cb)
    {
        OnAnimalCreatedCallback -= cb;
    }

    public void RegisterOnAnimalDestroyedCallback(Action<Animal> cb)
    {
        OnAnimalDestroyedCallback += cb;
    }

    public void UnregisterOnAnimalDestroyedCallback(Action<Animal> cb)
    {
        OnAnimalDestroyedCallback -= cb;
    }
}
