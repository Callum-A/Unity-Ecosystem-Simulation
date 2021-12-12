using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public int Width;
    public int Height;

    public TileSpriteController TileSpriteController;
    public FoodSpriteController FoodSpriteController;
    public AnimalSpriteController AnimalSpriteController;
    public EventLogController EventLogController;
    private Process graphWindow;

    /// <summary>
    /// Helper variables for initial food simulation will be removed later on after prey is fully implemented.
    /// </summary>
    public float time = 5;
    public float Days = 0;

    public float nutritionNeeded = 40f;

    //-------Creation Variables-------------//
    public static int PreyCount = 20;
    public static int PredatorCount = 2;
    public static int WorldType = 0;
    public static int WorldSeed = 207;
    public static float WaterLevel = 0.32f;
    public static float AridityLevel = 0;
    public static float PredatorBreedingRate = 1.0f;
    public static float PreyBreedingRate = 1.0f;
    public static float SpreadRate = 12;
    //---------------------------------------//

    public static WorldController Instance { get; protected set; }
    public World World { get; protected set; }

    private void OnEnable()
    {
        if (Instance != null)
        {
            UnityEngine.Debug.LogError("This shouldnt be reachable");
        }
        Instance = this;

        InitialiseTiles();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        World.Update(TimeController.TimeMultiplier * Time.deltaTime);
    }

    private void InitialiseTiles()
    {
        World = new World(Width, Height);

        World.AnimalManager.RegisterOnAnimalCreatedCallback(AnimalSpriteController.OnAnimalCreated);
        World.AnimalManager.RegisterOnAnimalDestroyedCallback(AnimalSpriteController.OnAnimalDestroyed);
        World.FoodManager.RegisterOnFoodSproutedCallback(FoodSpriteController.OnFoodSpawned);
        World.FoodManager.RegisterOnFoodExhaustedCallback(FoodSpriteController.OnFoodExhausted);
        World.FoodManager.RegisterOnFoodChangedCallback(FoodSpriteController.OnFoodChanged);

        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Tile t = World.GetTileAt(x, y);

                TileSpriteController.OnTileCreated(t);
                t.RegisterOnTileTypeChangedCallback(TileSpriteController.OnTileTypeChanged);
            }
        }

        //207
        World.GenerateTerrain(WorldSeed,WaterLevel, AridityLevel, WorldType);
        World.SproutInitialFood();
        World.SpawnAnimals(PreyCount, PredatorCount);
    }

    /// <summary>
    /// Prints the state of the world to the event log.
    /// </summary>
    public void WorldCountLog() 
    {
        EventLogController.AddLog($"Day {TimeController.Instance.NumberOfDays} started!");
        EventLogController.AddLog("Current Food: " + World.getFoodTiles().Count);
        EventLogController.AddLog("Current Nutrition: " + World.getTotalNutritionOnMap());
        EventLogController.AddLog("Current Prey: " + World.getPrey().Count);
    }

    public void ToggleGraph()
    {
        if (graphWindow == null || graphWindow.HasExited)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && File.Exists(@"Assets\StreamingAssets\EcologyCompanion.exe"))
            {
                graphWindow = Process.Start(@"Assets\StreamingAssets\EcologyCompanion.exe");
            }
        }

        else 
        {
            if (graphWindow.HasExited)
            {
                graphWindow = null;
            }

            else 
            {
                graphWindow.Kill();
                graphWindow = null;
            }  
        }
    }

    private void Start()
    {
        TimeController.Instance.RegisterOnNewDayCallback(o => World.AnimalManager.AgeUpAnimals());
        TimeController.Instance.RegisterOnNewDayCallback(World.FoodManager.OnNewDay);
        TimeController.Instance.RegisterOnNewDayCallback(o => WorldCountLog());
        TimeController.Instance.RegisterOnNewDayCallback(World.EventManager.OnNewDay);
    }

    private void OnApplicationQuit()
    {
        if (graphWindow != null)
        {
            graphWindow.Kill();
        }
    }
}
