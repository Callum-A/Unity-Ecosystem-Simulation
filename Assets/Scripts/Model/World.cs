using System;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public Tile[,] tiles { get; protected set; }
    public PathTileGraph tileGraph; // pathfinding graph, for navigating 

    public TerrainGenerator TerrainGenerator { get; protected set; }
    public AnimalManager AnimalManager { get; protected set; }
    public FoodManager FoodManager { get; protected set; }
    public WorldData Data { get; protected set; }

    public World(int w, int h)
    {
        Width = w;
        Height = h;
        tiles = new Tile[Width, Height];
        TerrainGenerator = new TerrainGenerator();
        AnimalManager = new AnimalManager(this);
        FoodManager = new FoodManager();
        Data = new WorldData();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(x, y, this);
            }
        }
    }

    public void Update(float deltaTime)
    {
        AnimalManager.Update(deltaTime);
    }

    public static int ManhattanDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }

    public static double EuclideanDistance(float x1, float y1, float x2, float y2)
    {
        return (Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }

    public void GenerateTerrain()
    {
        Data.TerrainData = TerrainGenerator.GenerateTerrain(tiles);
    }

    //TODO: clamp increases to prevent float wackiness
    public void ChangeWaterLevel(float change)
    {
        if (TerrainGenerator.WaterHeight + change > 1)
        {
            change = 1 - TerrainGenerator.WaterHeight;
        }
        else if (TerrainGenerator.WaterHeight + change < 0)
        {
            change = 0 - TerrainGenerator.WaterHeight;
        }

        if (change > 0)
        {
            TerrainGenerator.WaterHeight += change;

            //if (TerrainGenerator.WaterHeight > TerrainGenerator.sandHeightInit) { TerrainGenerator.SandHeight += change; }
        }
        else if (change < 0)
        {
            TerrainGenerator.WaterHeight += change;

           // if (TerrainGenerator.SandHeight > TerrainGenerator.sandHeightInit) { TerrainGenerator.SandHeight += change; }
        }
    }

    public void SpawnAnimals(int preyAmount, int predatorAmount)
    {
        AnimalManager.SpawnAnimals(preyAmount, predatorAmount);
    }

    public void SproutInitialFood()
    {
        FoodManager.SproutInitialFood(tiles);
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return null;
        }

        return tiles[x, y];
    }

    public Tile GetTileAt(Vector3 coord)
    {
        return GetTileAt(Mathf.FloorToInt(coord.x), Mathf.FloorToInt(coord.y));
    }

    /// <summary>
    /// Gets all of the water tiles on the map.
    /// </summary>
    /// <returns>List of water tiles</returns>
    public List<Tile> getWaterTiles()
    {
        return Data.WaterTiles;
    }

    /// <summary>
    /// Gets all the coastal tiles, the water tiles that are adjacent to a sant tile.
    /// </summary>
    /// <returns>List of coastal tiles</returns>
    public List<Tile> getCoastalTiles() 
    {
        return Data.CoastTiles;
    }

    /// <summary>
    /// Gets all the grass tiles on the map.
    /// </summary>
    /// <returns>List of grass tiles</returns>
    public List<Tile> getGrassTiles() 
    {
        return Data.GrassTiles;
    }

    /// <summary>
    /// Gets all the tiles with food on the map.
    /// </summary>
    /// <returns>List of tiles that contain food</returns>
    public List<Tile> getFoodTiles() 
    {
        return FoodManager.FoodTiles;
    }

    /// <summary>
    /// Get all the prey on the map
    /// </summary>
    /// <returns>List of prey.</returns>
    public List<Prey> getPrey() 
    {
        return AnimalManager.Prey;
    }

    /// <summary>
    /// Get all the predators on the map
    /// </summary>
    /// <returns>List of predators.</returns>
    public List<Predator> getPredators()
    {
        return AnimalManager.Predators;
    }

    /// <summary>
    /// Get the total nutrition on the map.
    /// </summary>
    /// <returns>Return the total nutrition</returns>
    public int getTotalNutritionOnMap() 
    {
        return FoodManager.getTotalNutrition();
    }
}
