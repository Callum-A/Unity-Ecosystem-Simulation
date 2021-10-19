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
    private Action<Food> OnFoodExhaustedCallback;
    private float SpreadCooldown;

    public FoodManager()
    {
        FoodTiles = new List<Tile>();
        newFoodTiles = new List<Tile>();
        SpreadCooldown = TimeController.Instance.SECONDS_IN_A_DAY;
        RegisterOnFoodExhaustedCallback(FoodExhausted);
    }

    public void Update(float deltatime)
    {
        SpreadCooldown -= deltatime;
        
        if (SpreadCooldown <= 0)
        {
            SpreadFoodAcrossMap();
            SpreadCooldown = TimeController.Instance.SECONDS_IN_A_DAY;
        } 
    }

    private void SpreadFoodAcrossMap()
    {
        int oldCount = FoodTiles.Count;

        foreach (Tile tile in FoodTiles)
        {
            Spread(tile); 
        }

        FoodTiles.AddRange(newFoodTiles);
        newFoodTiles.Clear();

        Debug.Log("Spread Occured, Previous Count = " + oldCount + ", Current Count = " + FoodTiles.Count);
    }

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

    public void FoodExhausted(Food food) 
    {
        FoodTiles.Remove(food.Tile);
        food.Tile.RemoveFood();

        Debug.Log("FOOD MANAGER - FOOD REMOVED");
    }

    /// <summary>
    /// Rolls to determine whether food will sprout on this tile, used only once on instantiation.
    /// </summary>
    private bool InitialSprout(Tile tile)
    {
        if (tile.Type == TileType.Ground && !tile.HasFood())
        {
            if (UnityEngine.Random.Range(0, Food.InitialSproutRate) == 0)
            {
                Food newFood = new Food(tile);
                newFood.RegisterOnFoodExhaustedCallback(OnFoodExhaustedCallback);
                tile.addFood(newFood);

                OnFoodSproutedCallback(tile.food);

                return true;
            }

            return false;

        }

        return false;
    }

    private bool Sprout(Tile tile)
    {
        if (tile.Type == TileType.Ground && !tile.HasFood())
        {
            if (UnityEngine.Random.Range(0, Food.SpreadRate) == 0)
            {
                Food newFood = new Food(tile);
                newFood.RegisterOnFoodExhaustedCallback(OnFoodExhaustedCallback);
                tile.addFood(newFood);
                OnFoodSproutedCallback(tile.food);
                newFoodTiles.Add(tile);

                return true;
            }

            return false;

        }

        return false;
    }

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

    public void RegisterOnFoodSproutedCallback(Action<Food> cb)
    {
        OnFoodSproutedCallback += cb;
    }

    public void RegisterOnFoodExhaustedCallback(Action<Food> cb)
    {
        OnFoodExhaustedCallback += cb;
    }
}

