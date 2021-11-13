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

    public float WaterHeight { get; protected set; }
    public float WaterHeightInitial { get { return terrainData.waterHeight; } protected set { } }

    public float SandHeight { get; protected set; }
    public float SandHeightInitial { get { return terrainData.sandHeight; } protected set { } }

    public float GrassHeight { get; protected set; }
    public float GrassHeightInitial { get { return terrainData.grassHeight; } protected set { } }

    public List<Tile> WaterTiles
    {
        get { return terrainData.waterTiles; }
        protected set { }
    }

    public float WaterPercent
    {
        get { return ((float)terrainData.waterTiles.Count / (float)terrainData.numTiles) * 100 ; }
        protected set { }
    }

    public List<Tile> SandTiles
    {
        get { return terrainData.sandTiles; }
        protected set { }
    }

    public float SandPercent
    {
        get { return ((float)terrainData.sandTiles.Count / (float)terrainData.numTiles) * 100; }
        protected set { }
    }

    public List<Tile> GrassTiles
    {
        get { return terrainData.grassTiles; }
        protected set { }
    }

    public float GrassPercent
    {
        get { return ((float)terrainData.grassTiles.Count / (float)terrainData.numTiles) * 100; }
        protected set { }
    }

    public List<Tile> CoastTiles
    {
        get { return terrainData.coastTiles; }
        protected set { }
    }

    #endregion


    public void Initialise()
    {
        WaterHeight = terrainData.waterHeight;
        SandHeight = terrainData.sandHeight;
        GrassHeight = terrainData.grassHeight;
    }
}

// The TerrainData struct is used to easily return multiple objects back to world after each GenerateTerrain call.
public struct TerrainData
{
    public int numTiles;
    public List<Tile> waterTiles;
    public List<Tile> sandTiles;
    public List<Tile> grassTiles;

    public float waterHeight;
    public float sandHeight;
    public float grassHeight;

    // used for meta gaming where water is, may not stay for long.
    public List<Tile> coastTiles;

    public TerrainData(int numTiles, float waterHeight, float sandHeight, float grassHeight)
    {
        this.numTiles = numTiles;

        this.waterHeight = waterHeight;
        this.sandHeight = sandHeight;
        this.grassHeight = grassHeight;

        waterTiles = new List<Tile>();
        sandTiles = new List<Tile>();
        grassTiles = new List<Tile>();
        coastTiles = new List<Tile>();
    }
}