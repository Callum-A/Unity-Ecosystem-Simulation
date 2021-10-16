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
    private Stack<Animal> animalsToSpawn;
    private Stack<Animal> animalsToDespawn;

    private Action<Animal> OnAnimalCreatedCallback;
    private Action<Animal> OnAnimalDestroyedCallback;

    public AnimalManager(World w)
    {
        world = w;
        Prey = new List<Prey>();
        Predators = new List<Predator>();
        AllAnimals = new List<Animal>();
        animalsToSpawn = new Stack<Animal>();
        animalsToDespawn = new Stack<Animal>();
    }

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

    public void Update(float deltaTime)
    {
        foreach (Animal a in AllAnimals)
        {
            a.Update(deltaTime);
        }

        while (animalsToDespawn.Count != 0)
        {
            Animal toDespawn = animalsToDespawn.Pop();
            AllAnimals.Remove(toDespawn);
        }

        while (animalsToSpawn.Count != 0)
        {
            Animal toSpawn = animalsToSpawn.Pop();
            AllAnimals.Add(toSpawn);
            if (OnAnimalCreatedCallback != null)
            {
                OnAnimalCreatedCallback(toSpawn);
            }
        }
    }

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
                p.CurrentTile.Y <= endY)
            {
                inRadius.Add(p);
            }
        }
        int closestDistance = Int32.MaxValue;
        foreach (Prey p in inRadius)
        {
            int currentDist = World.ManhattenDistance(x, y, p.CurrentTile.X, p.CurrentTile.Y);
            if (currentDist < closestDistance)
            {
                closestDistance = currentDist;
                closest = p;
            }
        }

        return closest;
    }

    public Prey FindClosestPrey(int x, int y)
    {
        Prey closest = null;
        int closestDistance = Int32.MaxValue;
        foreach (Prey a in Prey)
        {
            int currentDist = World.ManhattenDistance(x, y, a.CurrentTile.X, a.CurrentTile.Y);
            if (currentDist < closestDistance)
            {
                closestDistance = currentDist;
                closest = a;
            }
        }
        return closest;
    }

    private Animal Spawn(Animal a)
    {
        animalsToSpawn.Push(a);
        return a;
    }

    public Predator SpawnPredator(Tile tile)
    {
        Predator p = new Predator(tile, this);
        Predators.Add(p);
        Spawn(p);
        return p;
    }

    public Prey SpawnPrey(Tile tile)
    {
        Prey p = new Prey(tile, this);
        Prey.Add(p);
        Spawn(p);
        return p;
    }

    private void Despawn(Animal a)
    {
        if (OnAnimalDestroyedCallback != null)
        {
            OnAnimalDestroyedCallback(a);
        }

        animalsToDespawn.Push(a);
    }

    public void DespawnPredator(Predator p)
    {
        Predators.Remove(p);
        Despawn(p);
    }

    public void DespawnPrey(Prey p)
    {
        Prey.Remove(p);
        Despawn(p);
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
