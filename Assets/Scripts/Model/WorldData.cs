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
    public int numTiles
    {
        get { return terrainData.numTiles; }
        protected set { }
    }

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

    #endregion


    public WorldData()
    {
       
    }
}

// The TerrainData struct is used to easily return multiple objects back to world after each GenerateTerrain call.
public struct TerrainData
{
    public int numTiles;
    public List<Tile> waterTiles;
    public List<Tile> sandTiles;
    public List<Tile> grassTiles;

    public TerrainData(int numTiles)
    {
        this.numTiles = numTiles;

        waterTiles = new List<Tile>();
        sandTiles = new List<Tile>();
        grassTiles = new List<Tile>();
    }
}