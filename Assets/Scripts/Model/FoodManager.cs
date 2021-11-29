using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FoodManager
{
    public List<Tile> FoodTiles { get; protected set; }
    private List<Tile> newFoodTiles;

    private Action<Food> OnFoodSproutedCallback;
    private Action<Food> OnFoodChangedCallback;
    private Action<Food> OnFoodExhaustedCallback;

    public FoodManager()
    {
        FoodTiles = new List<Tile>();
        newFoodTiles = new List<Tile>();
        RegisterOnFoodExhaustedCallback(FoodExhausted);
    }

    /// <summary>
    /// Registered in Time controller within the world controller.
    /// </summary>
    /// <param name="world"></param>
    public void OnNewDay(World world) 
    {
        SpreadFoodAcrossMap();
    }

    /// <summary>
    /// Spreads Food across the map, should only be called once per day.
    /// </summary>
    private void SpreadFoodAcrossMap()
    {
        int oldCount = FoodTiles.Count;

        foreach (Tile tile in FoodTiles)
        {
            Spread(tile); 
        }

        FoodTiles.AddRange(newFoodTiles);
        newFoodTiles.Clear();
    }

    /// <summary>
    /// Sprouts the initial food onto the tiles, should only be called at the start and not again.
    /// </summary>
    /// <param name="tiles">The tiles that make up the world.</param>
    public void SproutInitialFood(Tile[,] tiles)
    {
        foreach (Tile tile in tiles)
        {
            if (InitialSprout(tile))
            {
                FoodTiles.Add(tile);
            }
        }
    }

    /// <summary>
    /// Occurs when food is exhausted, all nutrition consumed.
    /// </summary>
    /// <param name="food">The food that is exhausted</param>
    public void FoodExhausted(Food food) 
    {
        FoodTiles.Remove(food.Tile);
        food.Tile.RemoveFood();
    }

    /// <summary>
    /// Rolls to determine whether food will sprout on this tile, used only once on instantiation.
    /// </summary>
    /// <param name="tile">The tile to try and sprout food on</param>
    /// <returns></returns>
    private bool InitialSprout(Tile tile)
    {
        if (tile.Type == TileType.Ground && !tile.HasFood())
        {
            if (UnityEngine.Random.Range(0, Food.InitialSproutRate) == 0)
            {
                Food newFood = new Food(tile);
                newFood.RegisterOnFoodExhaustedCallback(OnFoodExhaustedCallback);
                newFood.RegisterOnFoodChangedCallback(OnFoodChangedCallback);
                tile.addFood(newFood);

                OnFoodSproutedCallback(tile.food);

                return true;
            }

            return false;

        }

        return false;
    }

    /// <summary>
    /// Rolls to determine whether food will sprout on this tile, used during spread.
    /// </summary>
    /// <param name="tile">The tile to try and sprout food on</param>
    /// <returns></returns>
    private bool Sprout(Tile tile)
    {
        if (tile.Type == TileType.Ground && !tile.HasFood())
        {
            if (UnityEngine.Random.Range(0, Food.SpreadRate) == 0)
            {
                Food newFood = new Food(tile);
                newFood.RegisterOnFoodExhaustedCallback(OnFoodExhaustedCallback);
                newFood.RegisterOnFoodChangedCallback(OnFoodChangedCallback);
                tile.addFood(newFood);
                OnFoodSproutedCallback(tile.food);
                newFoodTiles.Add(tile);

                return true;
            }

            return false;

        }

        return false;
    }

    public void AddFoodToTile(Tile tile)
    {
        Food newFood = new Food(tile);
        newFood.RegisterOnFoodExhaustedCallback(OnFoodExhaustedCallback);
        newFood.RegisterOnFoodChangedCallback(OnFoodChangedCallback);
        tile.addFood(newFood);
        OnFoodSproutedCallback(tile.food);
        newFoodTiles.Add(tile);
    }

    /// <summary>
    /// Spreads food from tiles adjacent to the initial tile.
    /// </summary>
    /// <param name="tile">The tile to spread from</param>
    private void Spread(Tile tile) 
    {
        foreach (Tile t in tile.GetNeighbours())
        {
            if (t != null)
            {
                Sprout(t);
            }
        }
    }

    /// <summary>
    /// Calculates the remianing nutrition in all food tiles.
    /// </summary>
    /// <returns>int total nutrition</returns>
    public int getTotalNutrition() 
    {
        int totalNutrition = 0;

        foreach (Tile tile in FoodTiles)
        {
            Food food = tile.food;
            totalNutrition += food.nutrition;
        }

        return totalNutrition;
    }

    /// <summary>
    /// Registers the callback for when food sprouts.
    /// </summary>
    /// <param name="cb">Callback to register</param>
    public void RegisterOnFoodSproutedCallback(Action<Food> cb)
    {
        OnFoodSproutedCallback += cb;
    }

    /// <summary>
    /// Registers the callback for when food is exhausted.
    /// </summary>
    /// <param name="cb">Callback to register</param>
    public void RegisterOnFoodExhaustedCallback(Action<Food> cb)
    {
        OnFoodExhaustedCallback += cb;
    }

    /// <summary>
    /// Registers the callback when food is changed to update sprites.
    /// </summary>
    /// <param name="cb">Callback to register.</param>
    public void RegisterOnFoodChangedCallback(Action<Food> cb)
    {
        OnFoodChangedCallback += cb;
    }
}

