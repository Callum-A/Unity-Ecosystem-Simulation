using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class FoodManager
{
    public List<Tile> FoodTiles { get; protected set; }

    private Action<Food> OnFoodSproutedCallback;
    private Action<Food> OnFoodExhaustedCallback;
    private float SpreadCooldown;

    public FoodManager()
    {
        FoodTiles = new List<Tile>();
        SpreadCooldown = TimeController.Instance.SECONDS_IN_A_DAY;
    }

    public void Update(float deltatime)
    {
        SpreadCooldown -= deltatime;
        
        if (SpreadCooldown <= 0)
        {
            SpreadFood();
            SpreadCooldown = TimeController.Instance.SECONDS_IN_A_DAY;
        } 
    }

    private void SpreadFood()
    {
        foreach (Tile tile in FoodTiles)
        {
            Sprout(tile);
        }
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

                return true;
            }

            return false;

        }

        return false;
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

