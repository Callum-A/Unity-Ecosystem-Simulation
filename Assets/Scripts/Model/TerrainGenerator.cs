using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public float[,] noiseMap { get; protected set; }

    //TODO: Do this correctly :)
    // hardcoded noise Variables.
    public int seed = 207;
    public float scale = 44;
    public int octaves = 5;
    public float persistance = 0.229f;
    public float lacunarity = 3;
    public Vector2 offset = new Vector2(0, 0);

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

    public void GenerateTerrain(Tile[,] Tiles, int width, int height)
    {
        UpdateNoiseMap(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile t = Tiles[x, y];
                float noise = noiseMap[x, y];

                setUpTile(t, noise);

            }
        }
    }

    //TODO: paramatirise the numbers
    private void setUpTile(Tile tile, float noise)
    {

        if (noise <= 0.3)
        {
            tile.Type = TileType.Water;
        }
        else if (noise <= 0.35)
        {
            tile.Type = TileType.Sand;
        }
        else if (noise <= 1)
        {
            tile.Type = TileType.Ground;
        }
    }


}
