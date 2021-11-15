using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// World data stores useful data about the world to be easily accessed, such as lists of each tile and total tile percentages. 
/// </summary>
public class WorldData
{
    #region TerrainData

    private TerrainData terrainData;
    public TerrainData TerrainData
    {
        get { return terrainData; }
        set { terrainData = value; }
    }
    public int NumTiles
    {
        get { return terrainData.numTiles; }
        protected set { }
    }

    #region Noise Variables

    public float[,] Noisemap { get{ return terrainData.noisemap; } protected set { } }

    public int Seed { get { return terrainData.seed; } protected set { } }
    public float Scale { get { return terrainData.scale; } protected set { } }
    public int Octaves { get { return terrainData.octaves; } protected set { } }
    public float Persistence { get { return terrainData.persistence; } protected set { } }
    public float Lacunarity { get { return terrainData.lacunarity; } protected set { } }
    public Vector2 Offset { get { return terrainData.offset; } protected set { } }

    #endregion

    #region Tile Regions

    public float WaterHeight { get { return terrainData.waterHeight; } set { terrainData.waterHeight = value; } }
    public float WaterHeightInitial { get { return terrainData.initialWaterHeight; } protected set { } }

    public float SandHeight { get { return terrainData.sandHeight; } set { terrainData.sandHeight = value; } }
    public float SandHeightInitial { get { return terrainData.initialSandHeight; } protected set { } }

    public float GrassHeight { get { return terrainData.grassHeight; } set { terrainData.grassHeight = value; } }
    public float GrassHeightInitial { get { return terrainData.initialGrassHeight; } protected set { } }
    public TileRegion[] Regions { get { return terrainData.regions; } set { terrainData.regions = value; } }

    #endregion

    #region Tile Data

    public List<Tile> WaterTiles{ get { return terrainData.waterTiles; } protected set { } }

    public float WaterPercent { get { return ((float)terrainData.waterTiles.Count / (float)terrainData.numTiles) * 100 ; } protected set { } }

    public List<Tile> SandTiles { get { return terrainData.sandTiles; } protected set { } }

    public float SandPercent{ get { return ((float)terrainData.sandTiles.Count / (float)terrainData.numTiles) * 100; } protected set { } }

    public List<Tile> GrassTiles { get { return terrainData.grassTiles; } protected set { } }

    public float GrassPercent { get { return ((float)terrainData.grassTiles.Count / (float)terrainData.numTiles) * 100; } protected set { } }

    public List<Tile> CoastTiles { get { return terrainData.coastTiles; } protected set { } }

    #endregion

    #endregion
}

// The TerrainData struct is used to easily return multiple objects back to world after each GenerateTerrain call.
public struct TerrainData
{
    public int numTiles;

    //noise data
    public float[,] noisemap;

    public int seed;
    public float scale;
    public int octaves;
    public float persistence;
    public float lacunarity;
    public Vector2 offset;

    //tile region data
    public float initialWaterHeight;
    public float initialSandHeight;
    public float initialGrassHeight;

    public float waterHeight;
    public float sandHeight;
    public float grassHeight;

    public TileRegion[] regions;

    public List<Tile> waterTiles;
    public List<Tile> sandTiles;
    public List<Tile> grassTiles;

    // used for meta gaming where water is, may not stay for long.
    public List<Tile> coastTiles;

    public TerrainData(int numTiles, float[,] noisemap, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, float waterHeight, float sandHeight, float grassHeight)
    {
        this.numTiles = numTiles;

        this.noisemap = noisemap;

        this.seed = seed;
        this.scale = scale;
        this.octaves = octaves;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
        this.offset = offset;

        initialWaterHeight = waterHeight;
        initialSandHeight = sandHeight;
        initialGrassHeight = grassHeight;

        this.waterHeight = initialWaterHeight;
        this.sandHeight = initialSandHeight;
        this.grassHeight = initialGrassHeight;

        regions = new TileRegion[0];

        waterTiles = new List<Tile>();
        sandTiles = new List<Tile>();
        grassTiles = new List<Tile>();
        coastTiles = new List<Tile>();
    }

}

[System.Serializable]
public struct TileRegion
{
    public TileType type;
    public float height;
    public float heightInitial;

    public TileRegion(TileType type, float height)
    {
        this.type = type;

        heightInitial = height;
        this.height = heightInitial;
    }
}