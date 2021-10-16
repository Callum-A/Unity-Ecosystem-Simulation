using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldData
{
    private TerrainData terrainData;
    public TerrainData TerrainData
    {
        get { return terrainData; }
        set { terrainData = value; }
    }

    float waterPercent;

    public List<Tile> WaterTiles
    {
        get { return terrainData.waterTiles; }
        protected set { }
    }

    public List<Tile> SandTiles
    {
        get { return terrainData.sandTiles; }
        protected set { }
    }

    public List<Tile> GrassTiles
    {
        get { return terrainData.grassTiles; }
        protected set { }
    }

    public int numTiles
    {
        get { return terrainData.numTiles; }
        protected set { }
    }

    public WorldData()
    {
       
    }
}

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