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
    public EventManager EventManager { get; protected set; }
    public WorldData Data { get; protected set; }

    public World(int w, int h)
    {
        Width = w;
        Height = h;
        tiles = new Tile[Width, Height];
        TerrainGenerator = new TerrainGenerator();
        AnimalManager = new AnimalManager(this);
        FoodManager = new FoodManager();
        EventManager = new EventManager();
        Data = new WorldData();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(x, y, this);
            }
        }

        tileGraph = null;
    }

    public void Update(float deltaTime)
    {
        AnimalManager.Update(deltaTime);
    }

    // function to force all tiles to update visually.
    public void ForceTileUpdate()
    {
        foreach(Tile tile in tiles)
        {
            WorldController.Instance.TileSpriteController.OnTileTypeChanged(tile);
        }
    }

    public static int ManhattanDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }

    public static double EuclideanDistance(float x1, float y1, float x2, float y2)
    {
        return (Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }

    public void GenerateTerrain(int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, float waterHeight, float sandHeight, float grassHeight, int generationMode)
    {
        Data.TerrainData = TerrainGenerator.GenerateTerrain(tiles, seed, scale, octaves, persistence, lacunarity, offset, waterHeight, sandHeight, grassHeight, generationMode);
        tileGraph = null;
    }

    /// <summary>
    /// custom generate function for usage in the main menu.
    /// </summary>
    /// <param name="seed"> seed of the terrain</param>
    /// <param name="waterHeight"> Sea level of the terrain, from 0 to 1.</param>
    /// <param name="aridity"> How arid the land is, from 0 to 1./param>
    /// <param name="generationType"> The enum value of the type of generation you want. 0 = normal, 1 = archipelago, 2 = island </param>
    public void GenerateTerrain(int seed, float waterHeight, float aridity, int generationType)
    {
        float sandHeight = waterHeight + 0.05f;
        if (sandHeight > 1) { sandHeight = 1; }

        aridity = Mathf.Lerp(sandHeight, 1, aridity);

        Data.TerrainData = TerrainGenerator.GenerateTerrain(tiles, seed, 44, 5, 0.229f, 3, new Vector2(0, 0), waterHeight, aridity, 1, generationType);
        tileGraph = null;
    }

    public void UpdateTerrain()
    {
        Data.TerrainData = TerrainGenerator.UpdateTerrain(tiles, Data.TerrainData);
        tileGraph = null;
    }

    public void ChangeWaterLevel(float change)
    {
        if (Data.WaterHeight + change > 1)
        {
            change = 1 - Data.WaterHeight;
        }
        else if (Data.WaterHeight + change < 0)
        {
            change = 0 - Data.WaterHeight;
        }

        if (change > 0)
        {
            if (Data.WaterHeight + change < 1)
            {

                Mathf.Min(Data.WaterHeight += change, 1);
                if (Data.WaterHeightInitial < Data.WaterHeight)
                {
                    if (Data.SandHeightInitial <= Data.WaterHeight + change)
                    {
                        Mathf.Min(Data.SandHeight = Data.SandHeight + change, 1);
                    }
                }
                else
                {
                    Data.SandHeight = Data.SandHeightInitial;
                }
            }
        }
        else if (change < 0)
        {
            if (Data.WaterHeight + change > 0)
            {
                Data.WaterHeight += change;
                if (Data.WaterHeightInitial > Data.WaterHeight)
                {
                    Data.SandHeight = Data.SandHeightInitial;
                }
                else
                {
                    if (Data.SandHeight > Data.SandHeightInitial)
                    {
                        Mathf.Max(Data.SandHeight = Data.SandHeight + change, 1);
                    }
                }
            }
        }
        UpdateTerrain();
    }

    public void SpawnAnimals(int preyAmount, int predatorAmount)
    {
        int[] highestpoint = TerrainGenerator.GetHighestPoint(Data.TerrainData);

        AnimalManager.SpawnAnimals(preyAmount, predatorAmount, highestpoint[0], highestpoint[1]);

        foreach (Animal a in AnimalManager.AllAnimals) 
        {
            a.setAdult();
            a.Hunger = UnityEngine.Random.Range(0.8f, 1f);
            a.Thirst = UnityEngine.Random.Range(0.8f, 1f);
        }

        Camera.main.transform.position = new Vector3(highestpoint[0], highestpoint[1], Camera.main.transform.position.z);
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
