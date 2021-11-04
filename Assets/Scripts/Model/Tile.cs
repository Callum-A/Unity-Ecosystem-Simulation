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

    public float MovementCost
    {
        get
        {
            if (_type == TileType.Water)
            {
                return 5f;
            }
            else
            {
                return 1f;
            }
        }
    }

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
        return World.GetTileAt(X + 1, Y - 1);
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

    //TODO: can be optimised further (currently a neighbourhood search n^2), also could use Euclidean distance over Manhattan for a 'smoother' radius.
    /// <summary>
    /// A simple radius check around a given tile, using Manhattan distance. Parse a float for a smoother radius with
    /// </summary>
    /// <param name="radius">The number of tiles in the radius, not including the root tile</param>
    /// <returns>A List of tiles within the given radius</returns>
    public List<Tile> GetRadius(int radius)
    {

        List<Tile> radiusList = new List<Tile>();

        //check OOB
        int rowCeiling = Math.Min(this.Y + radius + 1, World.Height);
        int rowFloor = Math.Max(this.Y - radius, 0);
        int columnCeiling = Math.Min(this.X + radius + 1, World.Width);
        int columnFloor = Math.Max(this.X - radius, 0);

        for (int y = rowFloor; y < rowCeiling; y++)
        {
            for (int x = columnFloor; x < columnCeiling; x++)
            {
                if (World.ManhattanDistance(this.X, this.Y, x, y) <= radius)
                {
                    radiusList.Add(World.GetTileAt(x, y));
                }
            }
        }

        return radiusList;
    }

    /// <summary>
    /// A simple radius check around a given tile, using Euclidean distance.
    /// </summary>
    /// <param name="radius">The number of tiles in the radius, not including the root tile</param>
    /// <returns>A List of tiles within the given radius</returns>
    public List<Tile> GetRadius(float radius)
    {
        List<Tile> radiusList = new List<Tile>();

        //check OOB
        int rowCeiling = Math.Min(this.Y + (int)Math.Ceiling(radius) + 1, World.Height);
        int rowFloor = Math.Max(this.Y - (int)Math.Floor(radius), 0);
        int columnCeiling = Math.Min(this.X + (int)Math.Ceiling(radius) + 1, World.Width);
        int columnFloor = Math.Max(this.X - (int)Math.Floor(radius), 0);

        for (int y = rowFloor; y < rowCeiling; y++)
        {
            for (int x = columnFloor; x < columnCeiling; x++)
            {
                if (World.EuclideanDistance(this.X, this.Y, x, y) <= radius)
                {
                    radiusList.Add(World.GetTileAt(x, y));
                }
            }
        }

        return radiusList;
    }

    public Tile GetClosestTile(List<Tile> tiles)
    {
        Tile closest = null;
        int closestDistance = Int32.MaxValue;

        foreach (Tile t in tiles)
        {
            int currentDist = World.ManhattanDistance(this.X, this.Y, t.X, t.Y);

            if (currentDist < closestDistance)
            {
                closestDistance = currentDist;
                closest = t;
            }
        }

        return closest;
    }

    public Tile GetClosestTile(List<Tile> tiles, TileType tileType)
    {
        List<Tile> tilesOfType = tiles.FindAll(t => t.Type == tileType);

        if (tilesOfType.Count > 0)
        {
            return GetClosestTile(tilesOfType);
        }

        return null;
    }

    public Tile GetClosestWaterTile()
    {
        return GetClosestTile(World.getCoastalTiles());
    }

    public Tile GetClosestFoodTile()
    {
        List<Tile> UnoccupiedFoodTiles = World.getFoodTiles().FindAll(t => !t.isFoodOccupied());
        return GetClosestTile(UnoccupiedFoodTiles);
    }

    public Tile GetRandomNonWaterTileInRadius(int r)
    {
        List<Tile> tiles = this.GetRadius(r);
        tiles = tiles.FindAll(t => t.Type != TileType.Water);

        if (tiles.Count == 0)
        {
            //Debug.LogError("No water tiles in radius found, from Tile -" + this.X + "," + this.Y);
            return null;
        }

        return tiles[UnityEngine.Random.Range(0, tiles.Count)];
    }

    public void addFood(Food foodToAdd)
    {
        this.food = foodToAdd;
    }

    public bool isFoodOccupied()
    {
        return food.IsOccupied;
    }

    public void setFoodOccupied()
    {
        Debug.Log("Setting Food Occupied");
        food.IsOccupied = true;
    }

    public void setFoodUnoccupied()
    {
        Debug.Log("Setting Food Unoccupied");
        food.IsOccupied = false;
    }

    /// <summary>
    /// Checks to see if there is food present on the tile.
    /// </summary>
    /// <returns>True if food is present</returns>
    public bool HasFood()
    {
        if (this.food != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Consumes food on the tile if there is any.
    /// </summary>
    public bool ConsumeFood()
    {
        if (HasFood())
        {
            food.Consume();
            return true;
        }

        return false;
    }

    public void DrownTile() 
    {
        if (HasFood()) 
        {
            food.Drown();
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

}
