using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public float[,] noiseMap { get; protected set; }

    //TODO: Noise function varibles shouldn't be hardcorded, should be set to defaults and edited through menus in unity.
    // hardcoded noise variables.
    private int seed = 207;
    public float scale = 44;
    public int octaves = 5;
    public float persistance = 0.229f;
    public float lacunarity = 3;
    public Vector2 offset = new Vector2(0, 0);

    public int Seed { get { return seed; } set { seed = value; } }
    public float Scale { get { return scale; } set { scale = value; } }
    public int Octaves { get { return octaves; } set { octaves = value; } }
    public float Persistance { get { return persistance; } set { persistance = value; } }
    public float Lacunarity { get { return lacunarity; } set { lacunarity = value; } }
    public Vector2 Offset { get { return offset; } set { offset = value; } }

    private bool isIsland = false;
    public bool IsIsland { get { return isIsland;} set { isIsland = value; } }

    //Tile heights
    private float waterHeightInit = 0.3f;
    public float sandHeightInit = 0.35f;
    private float grassHeightInit = 1f;

    private float waterHeight = 0.3f;
    private float sandHeight = 0.35f;
    private float grassHeight = 1f;

    public float WaterHeight { get { return waterHeight; } set { waterHeight = value; } }
    public float SandHeight { get { return sandHeight; } set { sandHeight = value; } }
    public float GrassHeight { get { return grassHeight; } set { grassHeight = value; } }

    /// <summary>
    /// Updates a 2D array of floats of normalised values, used as a heightmap. Uses varibles within terrain generation class to generate said noise (e.g. seed).
    /// </summary>
    /// <param name="width"> The width of the </param>
    /// <param name="height"></param>
    private void UpdateNoiseMap(int width, int height)
    {
        noiseMap = new float[width, height];

        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000, 100000) + offset.x;
            float offsetY = rng.Next(-100000, 100000) + offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);
        }

        //prevents division by 0 errors.
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffset[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffset[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                if (isIsland == true)
                {
                    noiseHeight = noiseHeight - (float)Math.Sqrt((halfWidth - x) * (halfWidth - x) + (halfHeight - y) * (halfHeight - y)) / 100;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
    }

    /// <summary>
    /// Generates terrain for a given tile map, updating the type for each tile.
    /// </summary>
    /// <param name="Tiles">A 2D array of tiles you want to generate terrain for.</param>
    /// <param name="width">Width of </param>
    /// <param name="height"></param>
    public TerrainData GenerateTerrain(Tile[,] Tiles)
    {
        UpdateNoiseMap(Tiles.GetLength(0), Tiles.GetLength(1));

        TerrainData data = new TerrainData(Tiles.Length);

        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                Tile t = Tiles[x, y];
                float noise = noiseMap[x, y];

                setUpTile(t, noise, data);

            }
        }

        data.coastTiles = findCoastTiles(data);

        return data;
    }

    /// <summary>
    /// Updates a tile's type based on the value of noise.
    /// </summary>
    /// <param name="tile">The tile you want to update.</param>
    /// <param name="noise">A normalised value to decide what type to change the tile to.</param>
    /// <param name="data">A terrain data struct, used to pass terrain generation data to the World class</param>param>
    private void setUpTile(Tile tile, float noise, TerrainData data)
    {
        if (noise <= waterHeight)
        {
            tile.Type = TileType.Water;
            data.waterTiles.Add(tile);
        }
        else if (noise <= sandHeight)
        {
            tile.Type = TileType.Sand;
            data.sandTiles.Add(tile);
        }
        else if (noise <= grassHeight)
        {
            tile.Type = TileType.Ground;
            data.grassTiles.Add(tile);
        }
    }

    //used to find the water adjacent tiles. Currently inefficent but an easy implemntaion
    public List<Tile> findCoastTiles(TerrainData data)
    {
        List<Tile> coastTiles = new List<Tile>();

        foreach (Tile tile in data.sandTiles)
        {
            foreach (Tile neighbour in tile.GetNeighbours())
            {
                if (neighbour != null)
                {
                    if (neighbour.Type == TileType.Water)
                    {
                        coastTiles.Add(tile);
                        break;
                    }
                }
            }
        }

        return coastTiles;
    }

}
