using System;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public Tile[,] tiles { get; protected set; }
    public PathTileGraph tileGraph; // pathfinding graph, for navigating 

    private TerrainGenerator terrainGenerator;
    public AnimalManager AnimalManager { get; protected set; }
    private WorldData worldData;

    // TODO: Move this to the struct that Dylan will add
    public List<Tile> FoodTiles { get; protected set; }

    public World(int w, int h)
    {
        Width = w;
        Height = h;
        tiles = new Tile[Width, Height];
        FoodTiles = new List<Tile>();
        terrainGenerator = new TerrainGenerator();
        AnimalManager = new AnimalManager(this);
        worldData = new WorldData();
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

    public void GenerateTerrain()
    {
        worldData.TerrainData = terrainGenerator.GenerateTerrain(tiles);
    }

    public void SpawnAnimals(int preyAmount, int predatorAmount)
    {
        AnimalManager.SpawnAnimals(preyAmount, predatorAmount);
    }

    public void SproutInitialFood() 
    {
        foreach (Tile tile in tiles)
        {
            tile.InitialSprout();
            if (tile.food != null)
            {
                FoodTiles.Add(tile);
            }
        }
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

    public Tile GetRandomNonWaterTileInRadius(Tile tile, int r)
    {
        List<Tile> tiles = tile.GetRadius(r);
        List<Tile> nonWater = new List<Tile>(); // memory inefficient but idc
        foreach (Tile t in tiles)
        {
            if (t != null)
            {
                if (t.Type != TileType.Water)
                {
                    nonWater.Add(t);
                }
            }
        }
        return nonWater[UnityEngine.Random.Range(0, nonWater.Count)];
    }

    public Tile FindClosestFoodTile(Tile tile)
    {
        Tile closest = null;
        int closestDistance = Int32.MaxValue;
        foreach (Tile t in FoodTiles)
        {
            int currentDist = ManhattanDistance(tile.X, tile.Y, t.X, t.Y);
            if (currentDist < closestDistance)
            {
                closestDistance = currentDist;
                closest = t;
            }
        }
        return closest;
    }
}
