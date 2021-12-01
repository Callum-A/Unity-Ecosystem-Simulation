using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public enum TerrainType 
    {
        Default,
        Lake,
        Island       // final version of island code, gave the results we actually wanted.
    }

    /// <summary>
    /// Generates and updates terrain with the given parameters. Also creates TerrainData which is stored within world and used to keep track of relevant info.
    /// </summary>
    /// <param name="Tiles">The array of tiles of the world.</param>
    /// <param name="width">Width of the map</param>
    /// <param name="height">Height of the map</param>
    /// <param name="seed">The seed used for the randomised sample offsets</param>
    /// <param name="scale">The scale of the noise.</param>
    /// <param name="octaves">Iterations of samples on the noise, higher values lead to more detailed noise, but increased computation</param>
    /// <param name="persistence">A multiplier for how quickly the amplitude of each successive octave decreases. Higher values lead to rougher noise.</param>
    /// <param name="lacunarity">A multiplier for how quickly the frequency of each successive octave increases. Higher values lead to smoother noise.</param>
    /// <param name="offset">Addtional sample point offset. Shifts each sample by the given cordinates.</param>
    /// <param name="waterHeight">Height of where water stops. value of 0 to 1.</param>
    /// <param name="sandHeight">Height of where sand stops. value of 0 to 1.</param>
    /// <param name="grassHeight">Height of where grass stops. value of 0 to 1.></param>
    /// <returns>A TerrainData struct.</returns>
    public TerrainData GenerateTerrain(Tile[,] Tiles, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, float waterHeight, float sandHeight, float grassHeight, int generationMode)
    {
        TerrainType type = (TerrainType)generationMode;

        float[,] heightmap = GenerateNoiseMap(Tiles.GetLength(0), Tiles.GetLength(1), seed, scale, octaves, persistence, lacunarity, offset, type);

        TerrainData data = new TerrainData(Tiles.Length, heightmap, seed, scale, octaves, persistence, lacunarity, offset, waterHeight, sandHeight, grassHeight);

        data = UpdateTerrain(Tiles, data);

        return data;
    }

    /// <summary>
    ///  Generates a 2D array of normalised values to be used as a noise map.
    /// </summary>
    /// <param name="width">Width of the map</param>
    /// <param name="height">Height of the map</param>
    /// <param name="seed">The seed used for the randomised sample offsets</param>
    /// <param name="scale">The scale of the noise.</param>
    /// <param name="octaves">Iterations of samples on the noise, higher values lead to more detailed noise, but increased computation</param>
    /// <param name="persistence">A multiplier for how quickly the amplitude of each successive octave decreases. Higher values lead to rougher noise.</param>
    /// <param name="lacunarity">A multiplier for how quickly the frequency of each successive octave increases. Higher values lead to smoother noise.</param>
    /// <param name="offset">Addtional sample point offset. Shifts each sample by the given cordinates.</param>
    private float[,] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, TerrainType type)
    {
        float[,] noisemap = new float[width, height];

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

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // range -1 to 1
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                switch (type)
                {
                    case TerrainType.Lake:

                        noiseHeight = noiseHeight + (float)Math.Sqrt((halfWidth - x) * (halfWidth - x) + (halfHeight - y) * (halfHeight - y)) / 50;
                        break;

                    case TerrainType.Island:

                        noiseHeight = noiseHeight - (float)Math.Sqrt((halfWidth - x) * (halfWidth - x) + (halfHeight - y) * (halfHeight - y)) / 25;
                        break;
                }


                // update min and max noise values;
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noisemap[x, y] = noiseHeight;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noisemap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noisemap[x, y]);
            }
        }

        return noisemap;
    }

    /// <summary>
    /// Updates the terrain layers of the map. Doesn't change the height map.
    /// </summary>
    /// <param name="Tiles">The array of tiles in the world.</param>
    /// <param name="data">The TerrainData you wish to update.</param>
    /// <returns>An updated TerrainData struct.</returns>
    public TerrainData UpdateTerrain(Tile[,] Tiles, TerrainData data)
    {
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                Tile t = Tiles[x, y];
                float noise = data.noisemap[x, y];

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
        if (noise <= data.waterHeight)
        {
            tile.Type = TileType.Water;
            data.waterTiles.Add(tile);
        }
        else if (noise <= data.sandHeight)
        {
            tile.Type = TileType.Sand;
            data.sandTiles.Add(tile);
        }
        else if (noise <= data.grassHeight)
        {
            tile.Type = TileType.Ground;
            data.grassTiles.Add(tile);
        }
    }

    //used to find the water adjacent sand tiles.
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
