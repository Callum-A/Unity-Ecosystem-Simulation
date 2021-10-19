using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public int Width;
    public int Height;

    public TileSpriteController TileSpriteController;
    public FoodSpriteController FoodSpriteController;
    public AnimalSpriteController AnimalSpriteController;

    /// <summary>
    /// Helper variables for initial food simulation will be removed later on after prey is fully implemented.
    /// </summary>
    public float time = 5;
    public float Days = 0;
    public float breedingRate = 1.1f;
    public float nutritionNeeded = 40f;
    //---------------------------------------//


    public static WorldController Instance { get; protected set; }
    public World World { get; protected set; }

    private void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("This shouldnt be reachable");
        }
        Instance = this;

        InitialiseTiles();
    }

    // Update is called once per frame
    void Update()
    {
        World.Update(Time.deltaTime);
    }

    private void InitialiseTiles()
    {
        World = new World(Width, Height);

        World.AnimalManager.RegisterOnAnimalCreatedCallback(AnimalSpriteController.OnAnimalCreated);
        World.AnimalManager.RegisterOnAnimalDestroyedCallback(AnimalSpriteController.OnAnimalDestroyed);
        World.FoodManager.RegisterOnFoodSproutedCallback(FoodSpriteController.OnFoodSpawned);
        World.FoodManager.RegisterOnFoodExhaustedCallback(FoodSpriteController.OnFoodExhausted);

        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Tile t = World.GetTileAt(x, y);

                TileSpriteController.OnTileCreated(t);
                t.RegisterOnTileTypeChangedCallback(TileSpriteController.OnTileTypeChanged);
            }
        }

        World.GenerateTerrain();
        World.SproutInitialFood();
        World.SpawnAnimals(10, 0);
    }

}
