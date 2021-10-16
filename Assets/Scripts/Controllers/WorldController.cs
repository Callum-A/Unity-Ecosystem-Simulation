using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public int Width;
    public int Height;

    public TileSpriteController TileSpriteController;
    public FoodController FoodController;

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

        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Tile t = World.GetTileAt(x, y);

                TileSpriteController.OnTileCreated(t);
                t.RegisterOnTileTypeChangedCallback(TileSpriteController.OnTileTypeChanged);
                t.RegisterOnFoodSproutedCallbackCallback(FoodController.OnFoodSpawned);
                t.RegisterOnFoodExhaustedCallbackCallback(FoodController.OnFoodExhausted);
            }
        }

        World.GenerateTerrain();
        World.SproutInitialFood();
        World.SpawnAnimals(1, 1);
    }

    /// <summary>
    /// Method purely used for initial testing of the food, once pray is in this will later become redundant and can be removed.
    /// Put this method in update to test
    /// </summary>
    private void PlantGrowthSimulationTest() 
    {
        if (FoodController.FoodCount > 0)
        {
            time -= Time.deltaTime;

            if (time <= 0)
            {
                Debug.Log("Day -" + Days);
                Days++;


                foreach (Tile tile in World.tiles)
                {
                    if (tile.HasFood())
                    {
                        tile.food.Spread();
                    }
                }

                int eaten = 0;

                while (eaten < nutritionNeeded && FoodController.FoodCount > 0)
                {

                    foreach (Tile tile in World.tiles)
                    {
                        if (FoodController.FoodCount <= 0)
                        {
                            Debug.Log("NO FOOD LEFT!");
                            break;
                        }

                        else if (eaten >= nutritionNeeded)
                        {
                            break;
                        }

                        else if (tile.HasFood())
                        {
                            tile.CosumeFood();
                            eaten++;
                        }
                    }

                }

                Debug.Log("Food Count: " + FoodController.FoodCount + "Nutrition Eaten - " + nutritionNeeded);

                nutritionNeeded = (nutritionNeeded * breedingRate);
                time = 5;

            }

        }

        else
        {
            Debug.LogError("NO FOOD!");
            Debug.Break();
        }
    }
}
