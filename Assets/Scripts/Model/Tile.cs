using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType
{
    Empty,
    Ground,
    Water,
    Sand,
    Plant
}

public class Tile
{
    public World World { get; protected set; }
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public Food food { get; protected set; }

    private TileType _type;

    public TileType Type
    {
        get => _type;
        set
        {
            TileType oldValue = _type;
            _type = value;
            if (oldValue != _type)
            {
                OnTileTypeChangedCallback(this);
            }
        }
    }

    private Action<Tile> OnTileTypeChangedCallback;
    private Action<Food> OnFoodSproutedCallback;
    private Action<Food> OnFoodExhaustedCallback;

    public Tile(int x, int y, World world)
    {
        X = x;
        Y = y;
        World = world;
    }

    public Tile North()
    {
        return World.GetTileAt(X, Y + 1);
    }

    public Tile NorthEast() 
    {
        return World.GetTileAt(X + 1, Y + 1);
    }

    public Tile NorthWest() 
    {
        return World.GetTileAt(X - 1, Y + 1);
    }

    public Tile East()
    {
        return World.GetTileAt(X + 1, Y);
    }

    public Tile South()
    {
        return World.GetTileAt(X, Y - 1);
    }

    public Tile SouthEast() 
    {
        return World.GetTileAt(X - 1, Y + 1);
    }

    public Tile SouthWest() 
    {
        return World.GetTileAt(X - 1, Y - 1);
    }

    public Tile West()
    {
        return World.GetTileAt(X - 1, Y);
    }

    public Tile[] GetNeighbours()
    {
        Tile[] ns = new Tile[4];
        ns[0] = North();
        ns[1] = East();
        ns[2] = South();
        ns[3] = West();
        return ns;
    }

    /// <summary>
    /// Find all of the tiles surrounding a given center tile. The array returned is the clockwise
    /// direction of the surrounding tiles.
    /// </summary>
    /// <returns>An array of surrounding tiles</returns>
    public Tile[] GetNeighboursIncludingDiagonal() 
    {
        Tile[] ns = new Tile[8];

        ns[0] = North();
        ns[1] = NorthEast();
        ns[2] = East();
        ns[3] = SouthEast();
        ns[4] = South();
        ns[5] = SouthWest();
        ns[6] = West();
        ns[7] = NorthWest();


        return ns;
    }

    /// <summary>
    /// Rolls to determine whether food will sprout on this tile, called by spread.
    /// </summary>
    public void Sprout() 
    {
        if (this.Type == TileType.Ground && !HasFood())
        {
            if (UnityEngine.Random.Range(0, food.SpreadRate) == 0)
            {
                food = new Food(this);
                food.RegisterOnFoodExhaustedCallback(OnFoodExhaustedCallback);
                OnFoodSproutedCallback(food);
            }
        }
    }

    /// <summary>
    /// Rolls to determine whether food will sprout on this tile, used only once on instantiation.
    /// </summary>
    public void InitialSprout()
    {
        if (this.Type == TileType.Ground && !HasFood())
        {
            if (UnityEngine.Random.Range(0, food.InitialSproutRate) == 0)
            {
                food = new Food(this);
                food.RegisterOnFoodExhaustedCallback(OnFoodExhaustedCallback);
                OnFoodSproutedCallback(food);
            }
        }
    }

    /// <summary>
    /// Checks to see if there is food present on the tile.
    /// </summary>
    /// <returns>True if food is present</returns>
    public bool HasFood()
    {
        if (food != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Consumes food on the tile if there is any.
    /// </summary>
    public void CosumeFood() 
    {
        if (HasFood()) 
        {
            food.Consume();
        }
    }

    /// <summary>
    /// Removes food from a tile, sets the food on the tile to null.
    /// </summary>
    public void RemoveFood() 
    {
        this.food = null;
    }

    public void RegisterOnTileTypeChangedCallback(Action<Tile> cb)
    {
        OnTileTypeChangedCallback += cb;
    }

    public void UnregisterOnTileTypeChangedCallback(Action<Tile> cb)
    {
        OnTileTypeChangedCallback -= cb;
    }

    public void RegisterOnFoodSproutedCallbackCallback(Action<Food> cb)
    {
        OnFoodSproutedCallback += cb;
    }

    public void UnregisterOnFoodSproutedCallbackCallback(Action<Food> cb)
    {
        OnFoodSproutedCallback -= cb;
    }

    public void RegisterOnFoodExhaustedCallbackCallback(Action<Food> cb)
    {
        OnFoodExhaustedCallback += cb;
    }

    public void UnregisterOnFoodExhaustedCallbackCallback(Action<Food> cb)
    {
        OnFoodExhaustedCallback -= cb;
    }

}
